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
using Clipcoin.Phone.Services.Enums;
using Clipcoin.Phone.Services.Interfaces;

namespace Clipcoin.Phone.Services.TrackerScanner
{
    public class KeyRequestEventArgs : EventArgs
    {
        public KeyResponceStatus Status { get; set; }
        public int Code { get; set; }
        public IAccessPoint AccessPoint { get; set; }
        public string Uid { get; set; }
    }
}