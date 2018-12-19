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
        BluetoothManager bManager;
        BluetoothAdapter bAdapter;
        private static SignalNotifier _rangeNotifier;

        private SignalNotifier SignalNotifier
        {
            get
            {
                if (_rangeNotifier == null)
                    _rangeNotifier = new SignalNotifier();
                return _rangeNotifier;
            }
        }

        public IObservable<BeaconScanResult> RangeNotifier
        {
            get
            {
                if (_rangeNotifier == null)
                    _rangeNotifier = new SignalNotifier();
                return _rangeNotifier;
            }
            
        }

        public override IBinder OnBind(Intent intent) => new Binder();

        public override void OnCreate()
        {
            base.OnCreate();

            bManager = (BluetoothManager)GetSystemService(Context.BluetoothService);
            bAdapter = bManager.Adapter;


        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Logger.Info("Signal scanner started");

            bAdapter.BluetoothLeScanner.StartScan(SignalNotifier);
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            bAdapter.BluetoothLeScanner.StopScan(SignalNotifier);
            base.OnDestroy();
        }
    }
}