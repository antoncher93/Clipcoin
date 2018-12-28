using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Smartphone.SignalManagement.Trackers
{
    public class Tracker : ITracker
    {
        private const string EmptyKey = "";

        public string Uid { get; set; }
        public string Key { get; set; } = EmptyKey;
        public bool IsObsolete { get; private set; } = false;
        public IAccessPoint AccessPoint { get; internal set; }
        public string UUID { get; internal set; } = "";
        public string SpaceUid { get; internal set; }
        public BaseObserver<BeaconSignal> ObserveManager { get; internal set; }

        private Timer timer1 = new Timer { Interval = 3600000, Enabled = true };

        public Tracker()
        {
            timer1.Elapsed += (s, e) =>
            {
                IsObsolete = true;
                timer1.Stop();
            };
        }

        public void StartObsolete(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}