using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Logging;

namespace Clipcoin.Phone.Services.Classes.Beacons
{
    public class BeaconScannerEventArgs : EventArgs
    {
        public ICollection<Beacon> Beacons { get; set; }
        public DateTime Time { get; set; }
    }

    [Service]
    public class BeaconScannerService : Service, IBeaconConsumer, IRangeNotifier
    {
        Context IBeaconConsumer.ApplicationContext => this;
        BeaconManager beaconManager;
        private static List<Region> regions;
        public static event EventHandler<BeaconScannerEventArgs> OnStaticRanginBeacons;
        public static event EventHandler<ServiceEventArgs> OnStateChanged;

        private static Region region = new Region("Common", null, null, null);

        private static int fsp = 1000;
        private static int fbsp = 0;
        private static int bsp = 5000;
        private static int bbsp = 0;

        public int ForegroundScanPeriod { get => fsp; set => fsp = value; }
        public int ForegroundBetweenScanPeriod { get => fbsp; set => fbsp = value; }
        public int BackgroundScanPeriod { get => bsp; set => bsp = value; }
        public int BackgroundBetweenScanPeriod { get => bbsp; set => bbsp = value; }

        private BluetoothManager bleManager;

        public override IBinder OnBind(Intent intent)
        {
            return new Binder();
        }

        public override void OnCreate()
        {
            base.OnCreate();
            beaconManager = BeaconManager.GetInstanceForApplication(this);
            beaconManager = BeaconManager.GetInstanceForApplication(this);
            beaconManager.BeaconParsers.Add(new BeaconParser()//.SetBeaconLayout(BeaconParser.AltbeaconLayout));
                .SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:25-25"));

            beaconManager.Bind(this);

            SetScanPeriods(ForegroundScanPeriod, ForegroundBetweenScanPeriod, BackgroundScanPeriod, BackgroundBetweenScanPeriod);
            bleManager = (BluetoothManager)GetSystemService(Context.BluetoothService);
            bleManager.Adapter.Enable();
        }

        public void SetScanPeriods(
            int foregroundScanPeriod,
            int foregroundBetweenScanPeriod,
            int backgroundScanPeriod,
            int backgroundBetweenScanPeriod)
        {
            ForegroundScanPeriod = foregroundScanPeriod;
            ForegroundBetweenScanPeriod = 0;
            BackgroundScanPeriod = backgroundScanPeriod;
            BackgroundBetweenScanPeriod = 0;

            ChangeManagerScanPeriods();
        }

        private void ChangeManagerScanPeriods()
        {
            beaconManager.SetForegroundScanPeriod(ForegroundScanPeriod);
            beaconManager.SetForegroundBetweenScanPeriod(ForegroundBetweenScanPeriod);
            beaconManager.SetBackgroundScanPeriod(BackgroundScanPeriod);
            beaconManager.SetForegroundBetweenScanPeriod(BackgroundBetweenScanPeriod);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            beaconManager.Bind(this);
            Logger.Info("Start Beacon Scanner");
            OnStateChanged?.Invoke(this, new ServiceEventArgs { State = Enums.ServiceState.Started });
            return base.OnStartCommand(intent, flags, startId);
        }

        bool IBeaconConsumer.BindService(Intent intent, IServiceConnection serviceConnection, Bind flags)
        {
            return this.ApplicationContext.BindService(intent, serviceConnection, flags);
        }

        public void OnBeaconServiceConnect()
        {
            if (beaconManager.IsBound(this))
            {
                beaconManager.UpdateScanPeriods();
            }

            beaconManager.SetRangeNotifier(this);
            beaconManager.StartRangingBeaconsInRegion(region);
            Toast.MakeText(this.ApplicationContext, "Beacon Scanner started.", ToastLength.Short).Show();
        }



        public override void OnDestroy()
        {
            base.OnDestroy();
            beaconManager.StopRangingBeaconsInRegion(region);
            beaconManager.Unbind(this);
            bleManager?.Adapter.Disable();
            bleManager.Adapter.Disable();
            OnStateChanged?.Invoke(this, new ServiceEventArgs { State = Enums.ServiceState.Destroyed });
            Toast.MakeText(this.ApplicationContext, "Beacon Service done.", ToastLength.Short).Show();
        }
        void IBeaconConsumer.UnbindService(IServiceConnection serviceConnection)
        {
            this.ApplicationContext.UnbindService(serviceConnection);
        }

        public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            OnStaticRanginBeacons?.Invoke(this, new BeaconScannerEventArgs {Beacons = beacons, Time = DateTime.Now });
        }
    }
}