using System;
using Clipcoin.Phone.Services.Interfaces;
using NO.Nordicsemi.Android.Support.V18.Scanner;

namespace Clipcoin.Phone.Services.Signals
{
    class NordicBleSignalHolder : ScanCallback
    {
        public ISignalProvider SignalNotifier { get; private set; } = new SignalNotifier();

        public override void OnScanResult(int p0, ScanResult result)
        {
            base.OnScanResult(p0, result);

            System.Diagnostics.Debug.WriteLine($"*** {DateTime.Now.TimeOfDay} {result.Device.Address}");

            var time = DateTime.Now;

            var bleSignal = new BleSignal(
                       result.Device.Address,
                       result.Rssi,
                       result.ScanRecord.GetBytes(),
                       time
                       );

            SignalNotifier.OnBleSignal(bleSignal);
        }
    }
}