using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using System.Threading.Tasks;
using Java.Util;
using Android.Bluetooth.LE;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Clipcoin.Smartphone.SignalManagement.Logging;

namespace Clipcoin.Phone.Services.Signals
{
    [Service]
    public class SignalScanner : Service, ISignalScanner
    {
        private BluetoothManager bManager;
        private BluetoothAdapter bAdapter;
        private static BleSignalHolder _rangeNotifier;

        private BleSignalHolder _signalsScanCallback
        {
            get
            {
                if (_rangeNotifier == null)
                    _rangeNotifier = new BleSignalHolder();
                return _rangeNotifier;
            }
        }

        public IObservable<BeaconSignal> RangeNotifier
        {
            get
            {
                return _signalsScanCallback.SignalNotifier;
            }
        }

        public override IBinder OnBind(Intent intent) => new Binder();

        public override void OnCreate()
        {
            base.OnCreate();
            
            bManager = (BluetoothManager)GetSystemService(Context.BluetoothService);
            bAdapter = bManager.Adapter;
            bAdapter.Enable();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Logger.Info("Signal scanner started");
            var scanner = bAdapter.BluetoothLeScanner;

            var settings = new ScanSettings.Builder()
                .SetReportDelay(0)
                //.SetPhy(Android.Bluetooth.BluetoothPhy.Le1m)
                .SetScanMode(Android.Bluetooth.LE.ScanMode.LowLatency)                
                .Build();

            scanner.StartScan(new List<ScanFilter>() , settings,  _signalsScanCallback);
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            //SignalNotifier.Dispose();
            bAdapter.BluetoothLeScanner.StopScan(_signalsScanCallback);
            base.OnDestroy();
        }
    }
}