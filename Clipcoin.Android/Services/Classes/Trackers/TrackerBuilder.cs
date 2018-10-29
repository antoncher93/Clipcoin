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
    public class TrackerBuilder
    {
        private Tracker tracker = new Tracker();

        private TrackerBuilder Modify(Action a)
        {
            a?.Invoke();
            return this;
        }

        public TrackerBuilder Uid(string uid)
            => Modify(() => tracker.Uid = uid);

        public TrackerBuilder AccessPoint(IAccessPoint apoint)
            => Modify(() => tracker.AccessPoint = apoint);

        public TrackerBuilder Beacon(string mac)
            => Modify(() => tracker.Beacons.Add(mac));

        public Tracker Build() => tracker;

    }
}