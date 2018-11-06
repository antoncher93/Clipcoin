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

namespace Clipcoin.Phone.Services.Signals
{
    [Service]
    public class SignalScanner : Service
    {
        BluetoothManager manager;

        public override IBinder OnBind(Intent intent) => new Binder();

        public override void OnCreate()
        {
            base.OnCreate();

            manager = (BluetoothManager)GetSystemService(Context.BluetoothService);

            
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var callback = new SignalsCallback();
            manager.Adapter.BluetoothLeScanner.StartScan(callback);
            manager.Adapter.BluetoothLeScanner.StopScan(callback);
            return base.OnStartCommand(intent, flags, startId);
        }
    }
}