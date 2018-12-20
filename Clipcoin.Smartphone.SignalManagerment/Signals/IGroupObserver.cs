using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Clipcoin.Smartphone.SignalManagement.Interfaces;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public interface IGroupObserver
    {
        GroupID GroupID { get; }
        Timer Timer { get; }

        void SetSignal(BeaconSignal signal);
    }
}
