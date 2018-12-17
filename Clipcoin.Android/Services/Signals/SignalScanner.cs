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

namespace Clipcoin.Phone.Services.Signals
{
    [Service]
    public class SignalScanner : Service
    {
        BluetoothAdapter bAdapter;

        public override IBinder OnBind(Intent intent) => new Binder();

        public override void OnCreate()
        {
            base.OnCreate();

            bAdapter = (BluetoothAdapter)GetSystemService(Context.BluetoothService);


        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            bAdapter.BluetoothLeScanner.StartScan(new LeScanCallback());

            return base.OnStartCommand(intent, flags, startId);
        }
    }
}