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
using Clipcoin.Phone.Settings;
using Java.IO;
using Square.OkHttp;

namespace Clipcoin.Phone.BroadcastReceivers
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Actions.ConfirmEnter, Actions.ConfirmExit })]
    public class ConfirmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            new ConfirmTask(intent.Action, UserSettings.Token, new Callback(context)).Run();
        }

        private class Callback : Java.Lang.Object, ICallback
        {
            Context _ctx;
            Handler _handler = new Handler(Looper.MainLooper);
            public Callback(Context ctx)
            {
                _ctx = ctx;
            }
            public void OnFailure(Request request, IOException iOException)
            {
                _handler.Post(() => Toast.MakeText(_ctx, "Fail", ToastLength.Short).Show());
                
            }

            public void OnResponse(Response response)
            {
                _handler.Post(() => Toast.MakeText(_ctx, response.Message(), ToastLength.Short).Show());
                
            }
        }
    }
    
}