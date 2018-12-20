using System;
using System.Collections.Generic;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface IPackager
    {
        void PushPackage(IEnumerable<BeaconSignal> package);
        void Flush();
    }
}
