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
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Interfaces;

namespace Clipcoin.Phone.Services.TrackerScanner
{
    public class TrackerScanInfo
    {
        public ICollection<ITracker> Trackers { get; set; }


    }
}