using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Phone.Services.Signals
{
    public class BleSignalHolder : ScanCallback
    {
        public ISignalProvider SignalNotifier { get; private set; } = new SignalNotifier();

        public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);

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