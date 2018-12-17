using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Bluetooth.BluetoothAdapter;

namespace Clipcoin.Phone.Services.Signals
{
    public class LeScanCallback : ScanCallback
    {
        public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);

            System.Diagnostics.Debug.WriteLine($"{result.Device.Address}, {result.Rssi}");
        }

        public override void OnBatchScanResults(IList<ScanResult> results)
        {
            base.OnBatchScanResults(results);


        }
    }
}