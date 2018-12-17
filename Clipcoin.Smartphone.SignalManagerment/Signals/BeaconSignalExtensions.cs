using System.Collections.Generic;
using System.Linq;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public static class BeaconSignalExtensions
    {
        public static IEnumerable<BeaconSignal> SetTreshold(this IEnumerable<BeaconSignal> signals, int treshold)
        {
            return signals?.Where(s => s.Rssi >= treshold);
        }
    }
}
