using System;
using System.Collections.Generic;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface ISignalScanner
    {
        IObservable<BeaconSignal> RangeNotifier { get; }
    }
}
