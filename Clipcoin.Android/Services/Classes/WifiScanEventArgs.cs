using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clipcoin.Phone.Services.Classes
{
    public class WifiScanEventArgs : EventArgs
    {
        public ICollection<ScanResult> ScanResults { get; set; }
    }
}