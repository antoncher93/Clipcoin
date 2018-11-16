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
using Clipcoin.Phone.Database;
using Clipcoin.Phone.Help;
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Settings;
using Java.IO;
using Square.OkHttp;


namespace Clipcoin.Phone.Services.Signals
{
    public class Callback : Java.Lang.Object, ICallback
    {
        public SignalsDBWriter Writer { get; set; }
        public ICollection<int> Ids { get; set; }

        public void OnFailure(Request request, IOException iOException)
        {
            Logger.Info("Fail");
        }

        public void OnResponse(Response response)
        {
            int code = response.Code();
            switch (code)
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
        //private const string Url = "http://192.168.0.9:5000";
        private const string Url = "http://technobee.elementstore.ru:5000";
        SignalsDBWriter dbWriter;
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

            userId = JwtHelper.GetAspNetUserId(UserSettings.Token);

            dbWriter = new SignalsDBWriter(this);
            timer = new Timer
            {
                Interval = 30000,
                Enabled = true
            };

            timer.Elapsed += (s, e) =>
            {
                Logger.Info("trying to sent telemetry...");

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

                    var all_signals = telemetry.SelectMany(b => b.Select(i => i)).ToList();

                    Logger.Info("Send Telemetry: " + all_signals.Count.ToString());
                    client.NewCall(request).Enqueue(c);
                }
                else
                {
                    Logger.Info("No signals to send");
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