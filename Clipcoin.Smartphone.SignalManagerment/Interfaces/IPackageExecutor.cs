using System;
using System.Collections.Generic;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface IPackageExecutor
    {
        void Execute(IEnumerable<(IBeaconSignal, DateTime)> package);
    }
}
