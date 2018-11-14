﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Services.Classes;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Trackers;
using Clipcoin.Phone.Settings;

namespace Clipcoin.Phone.Services.TrackerScanner
{
    public class APointManager// : ICollection<IAccessPoint>
    {
        public const int MAX_AP_INVISIBLE_TIME_SEC = 5;
        public ICollection<IAccessPoint> AccessPoints { get; private set; } = new List<IAccessPoint>();

        public TrackerManager TrackerManager { get; private set; } = new TrackerManager();

        public void Add(ICollection<ScanResult> networks)
        {
            var old_networks = AccessPoints.Where(
                a => networks.Any(n => n.Bssid.Equals(a.Bssid, StringComparison.CurrentCultureIgnoreCase)));

            foreach(var ap in old_networks)
            {
                ap.LastTime = DateTime.Now;
            }


            var fresh_networks = networks.Where(t => 
                AccessPoints.Any(a => !a.Bssid.Equals(t.Bssid, StringComparison.CurrentCultureIgnoreCase)));

            foreach(var n in fresh_networks)
            {

            }
        }

        public void Add(IAccessPoint item)
        {
            var ap = AccessPoints.FirstOrDefault(p => string.Equals(p.Bssid, item.Bssid, StringComparison.CurrentCultureIgnoreCase));

            if(ap == null)
            {
                AccessPoints.Add(item);

                TrackerManager.CheckAccessPoint(item);
            }
            else
            {
                ap.LastTime = item.LastTime;
            }
        }

        public void Update()
        {
            AccessPoints = AccessPoints.Where(
                ap => (ap.LastTime != null) && ((DateTime.Now - ap.LastTime) < TimeSpan.FromSeconds(MAX_AP_INVISIBLE_TIME_SEC))).ToList();
        }

     
    }
}