using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clipcoin.Phone.BroadcastReceivers.Wifi
{
    [BroadcastReceiver]
    public class WifiConnectionReceiver : BroadcastReceiver
    {
        public event EventHandler<WifiConnectionEventArgs> Received;

        public override void OnReceive(Context context, Intent intent)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            if (connectivityManager.ActiveNetworkInfo?.Type == ConnectivityType.Wifi)
            {
                Received?.Invoke(this, new WifiConnectionEventArgs());
            }
        }
    }

    public class WifiConnectionEventArgs : EventArgs
    {

    }
}