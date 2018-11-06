using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SignalTest.Services
{
    class Ranger : Java.Lang.Object, IRangeNotifier
    {
        public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            System.Diagnostics.Debug.WriteLine($"[{System.DateTime.Now.ToString()}] Beacons: {beacons.Count} Region Id {region.UniqueId}");
        }
    }
}