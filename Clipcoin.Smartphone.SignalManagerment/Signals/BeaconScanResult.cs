using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public struct BeaconScanResult
    {
        public IList<BeaconSignal> Signals { get; set; }
        public DateTime Time { get; set; }
    }
}