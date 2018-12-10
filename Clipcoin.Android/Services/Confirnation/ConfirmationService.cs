using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Runnable;
using Java.IO;
using Square.OkHttp;

namespace Clipcoin.Phone.Services.Confirnation
{
    [Service]
    public class ConfirmationService : Service, ICallback
    {
        private const string ConfirmUrl = "http://technobee.elementstore.ru/api/Contract/activities/confirm";
        public const string ConfirmEvent = "Event";
        public const string ConfirmEnter = "EnterStore";
        public const string ConfirmExit = "ExitStore";
        public const string Token = "token";
        private Handler _handler;


        public override IBinder OnBind(Intent intent) => new Binder();

        public override void OnCreate()
        {
            base.OnCreate();
            _handler = new Handler(Looper.MainLooper);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var confirm = intent.GetStringExtra(ConfirmEvent);
            var token = intent.GetStringExtra(Token);
            new ConfirmTask(confirm, token, this).Run();

            return base.OnStartCommand(intent, flags, startId);
        }

        public void OnFailure(Request request, IOException iOException)
        {
            _handler.Post(() => Toast.MakeText(this, "Fail", ToastLength.Short).Show());
            
            StopSelf();
        }

        public void OnResponse(Response response)
        {
            _handler.Post(() => Toast.MakeText(this, response.Message(), ToastLength.Short).Show());
            
            StopSelf();
        }
    }

    
}