using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Help;
using Clipcoin.Phone.Logging;
using Java.IO;
using SCAppLibrary.Android.Services.Telemetry;
using Square.OkHttp;


namespace Clipcoin.Phone.Services.Signals
{
    public class Callback : Java.Lang.Object, ICallback
    {
        public TelemetryDatabaseWriter Writer { get; set; }
        public ICollection<int> Ids { get; set; }

        public void OnFailure(Request request, IOException iOException)
        {
            Logger.Info("Fail");
        }

        public void OnResponse(Response response)
        {
            switch (response.Code())
            {
                case 200:
                    Logger.Info("Success");
                    foreach(var id in Ids)
                    {
                        Writer.DeleteById(id);
                    }
                    break;
                default:
                    Logger.Info("Fail Responce");
                    break;
            }
        }
    }

    [Service]
    public class TelemetrySendService : Service
    {
        private const string Url = "http://192.168.0.9:5000";
        TelemetryDatabaseWriter dbWriter;
        static string _token;
        public string Token
        {
            get => _token;
            set => _token = value;
        }

        ICollection<int> ids;


        Timer timer;
        string userId;

        public override IBinder OnBind(Intent intent)
        {
            return new Binder();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            userId = JwtHelper.GetAspNetUserId(_token);

            dbWriter = new TelemetryDatabaseWriter(this);
            timer = new Timer
            {
                Interval = 30000,
                Enabled = true
            };

            timer.Elapsed += (s, e) =>
            {
                var type = MediaType.Parse("application/json; charset=utf-8");
                var telemetry = dbWriter.ReadTelemetry(userId, out ids);

                if(telemetry?.Count > 0)
                {
                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(telemetry, new Trigger.Classes.TelemetryJsonConverter());
                    var body = RequestBody.Create(type, str);
                    OkHttpClient client = new OkHttpClient();
                    Request request = new Request.Builder()
                    .Url(Url)
                    .Header("scn-dev-msg", "")
                    .Post(body)
                    .Build();

                    Callback c = new Callback { Writer = dbWriter, Ids = ids };

                    Logger.Info("Send Telemetry");
                    client.NewCall(request).Enqueue(c);
                }
               
            };

            
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            timer.Start();

            Logger.Info("Sender Service Started");
            
            //заглушка
            dbWriter.ClearData();

            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            timer.Stop();

            Logger.Info("Sender Service Destroyed");
            base.OnDestroy();
        }

        
    }
}