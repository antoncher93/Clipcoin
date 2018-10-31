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
using Clipcoin.Phone.Services.Interfaces;

namespace Clipcoin.Phone.Services.Classes
{
    public class APointInfo : IAccessPoint
    {
        public string Ssid { get; set; }
        public string Bssid { get; set; }
        public string Password { get; set; }
        public DateTime LastTime { get; set; }
    }
}