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

namespace Clipcoin.Phone.Services.Classes.Wifi.Events
{
    public class ExchangeEventArgs : TaskEventArgs
    {
        public IAccessPoint AccessPoint { get; set; }
        public int Code { get; set; }
        public string Body { get; set; }
    }
}