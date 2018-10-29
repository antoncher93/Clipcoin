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
    public class WifiStateEventArgs : EventArgs
    {
        public WifiState WifiState { get; set; }
    }
}