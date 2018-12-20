using System;
using System.Collections.Generic;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface IPackageExecutor
    {
        void Execute(IEnumerable<BeaconSignal> package);
    }
}
