using System;
using System.Collections.Generic;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface IPackager
    {
        void PushPackage(IEnumerable<(IBeaconSignal signal, DateTime dateTime)> package);
        void Flush();
    }
}
