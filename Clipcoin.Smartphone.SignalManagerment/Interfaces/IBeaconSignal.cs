using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface IBeaconSignal
    {
        string Mac { get; }
        int Rssi { get; }
        int Minor { get; }
        int Major { get; }
        string UUID { get; }
    }
}