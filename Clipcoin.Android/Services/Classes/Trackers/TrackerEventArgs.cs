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

namespace Clipcoin.Phone.Services.Classes.Trackers
{
    public class TrackerEventArgs : EventArgs
    {
        public ITracker Tracker { get; set; }
    }
}