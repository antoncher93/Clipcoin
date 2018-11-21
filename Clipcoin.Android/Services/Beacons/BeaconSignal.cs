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

namespace Clipcoin.Phone.Services.Beacons
{
    public class BeaconSignal : IBeaconSignal
    {
        public string Mac { get; set; }
        public string UUID { get; set; }
        public int Rssi { get; set; }
        public double Distance { get; set; }
    }
}