using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clipcoin.Phone.BroadcastReceivers.Wifi
{
    [BroadcastReceiver]
    public class WifiStateReceiver : BroadcastReceiver
    {
        public event EventHandler<WifiStateEventArgs> OnStateChanged;

        public override void OnReceive(Context context, Intent intent)
        {
            var state = intent.GetIntExtra(WifiManager.ExtraWifiState, (int)WifiState.Unknown);
            OnStateChanged?.Invoke(this, new WifiStateEventArgs { WifiState = (WifiState)state });
        }

    }
}