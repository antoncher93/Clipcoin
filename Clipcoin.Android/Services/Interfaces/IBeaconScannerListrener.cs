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

namespace Clipcoin.Phone.Services.Interfaces
{
    public interface IBeaconScannerListrener
    {
        void DidRangeBeacons(ICollection<Beacon> beacons, DateTime time);
    }
}