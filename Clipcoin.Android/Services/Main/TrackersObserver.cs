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
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using Clipcoin.Smartphone.SignalManagement.Trackers;

namespace Clipcoin.Phone.Services.Main
{
    internal class TrackersObserver : BaseObserver<TrackerEventArgs>
    {
        private readonly Context _ctx;
        private readonly SignalScanner signalScanner;
        private int _count = 1;

        public TrackersObserver(Context ctx)
        {
            _ctx = ctx;
            signalScanner = new SignalScanner();
        }

        public override void OnNext(TrackerEventArgs value)
        {
            base.OnNext(value);

            if(value.Count > 0)
            {
                value.NewTracker.ObserveManager.Subscribe(signalScanner.RangeNotifier);
                ServiceTools.StartFinally(_ctx, signalScanner.Class);
            }
            else
            {
                ServiceTools.StopFinally(_ctx, signalScanner.Class);
            }
        }
    }
}