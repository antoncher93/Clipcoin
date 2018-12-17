using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Interfaces;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public struct BeaconSignal : IBeaconSignal
    {
        public string Mac { get; set; }
        public string UUID { get; set; }
        public int Rssi { get; set; }
        public double Distance { get; set; }
        public int Minor { get; set; }
        public int Major { get; set; }        
    }

    
}