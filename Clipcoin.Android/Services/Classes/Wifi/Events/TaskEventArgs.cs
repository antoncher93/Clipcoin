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

namespace Clipcoin.Phone.Services.Classes.Wifi.Events
{
    public class TaskEventArgs : EventArgs
    {
        public bool Success { get; set; }
    }
}