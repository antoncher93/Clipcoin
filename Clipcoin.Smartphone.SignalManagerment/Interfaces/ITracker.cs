using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface ITracker
    {
        IAccessPoint AccessPoint { get;}
        BaseObserver<BeaconSignal> ObserveManager { get; }
        string Uid { get; }
        string Key { get; set; }
        string UUID { get; }
        string SpaceUid { get; }
        bool IsObsolete { get; }
    }
}