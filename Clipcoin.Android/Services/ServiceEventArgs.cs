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

namespace Clipcoin.Phone.Services
{
    public class ServiceEventArgs : EventArgs
    {
        public ServiceState State { get; set; }
    }
}