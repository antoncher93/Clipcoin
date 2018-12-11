using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Interfaces;

namespace Clipcoin.Smartphone.SignalManagement.Trackers
{
    public class TrackerEventArgs : EventArgs
    {
        public ICollection<ITracker> Trackers { get; set; }
    }
}