﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clipcoin.Phone.Services.Interfaces
{
    public interface IAccessPoint
    {
        string Ssid { get; }
        string Bssid { get; }
        string Password { get; set; }
        void Update();
        bool Visible { get; }
        event EventHandler OnDisAppear;
    }
}