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
using Clipcoin.Phone.Logging;

namespace Clipcoin.Phone.Services.Signals
{
    public class SignalsCallback : ScanCallback
    {
        public override void OnBatchScanResults(IList<ScanResult> results)
        {
            base.OnBatchScanResults(results);


        }

        public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);
            Logger.Info($"Device: {result.Device.Address}, RSSI: {result.Rssi}");
        }

        public override void OnScanFailed([GeneratedEnum] ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);
        }
    }
}