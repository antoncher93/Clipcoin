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
using Java.Util;

namespace Clipcoin.Phone.Services.Beacons
{
    public class BeaconScannerEventArgs : EventArgs
    {
        public ICollection<Beacon> Beacons { get; set; }
        public DateTime Time { get; set; }
    }

    [Service]
    public class BeaconScannerService : Service, IBeaconConsumer
    {
        Context IBeaconConsumer.ApplicationContext => this;
        BeaconManager beaconManager;
        public static event EventHandler<BeaconScannerEventArgs> OnStaticRanginBeacons;
        public static event EventHandler<ServiceEventArgs> OnStateChanged;

        private static Region region; //=  new Region("Common", null, null, null);
        
        private static RangeNotifier _rangeNotifier;
        

        private static int fsp = 500;
        private static int fbsp = 1000;
        private static int bsp = 0;
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

        public static RangeNotifier RangeNotifier
        {
            get
            {
                if(_rangeNotifier == null)
                {
                    _rangeNotifier = new RangeNotifier();
                }
                return _rangeNotifier;
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();

            beaconManager = BeaconManager.GetInstanceForApplication(this);
         
            beaconManager.BeaconParsers.Add(new BeaconParser().SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:25-25"));
            beaconManager.BeaconParsers.Add(new BeaconParser().SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));
            beaconManager.BeaconParsers.Add(new BeaconParser().SetBeaconLayout("m:0-3=4c000215,i:4-19,i:20-21,i:22-23,p:24-24"));

            beaconManager.BeaconParsers.Add(new BeaconParser().SetBeaconLayout(BeaconParser.AltbeaconLayout));
            beaconManager.BeaconParsers.Add(new BeaconParser().SetBeaconLayout(BeaconParser.EddystoneTlmLayout));
            beaconManager.BeaconParsers.Add(new BeaconParser().SetBeaconLayout(BeaconParser.EddystoneUidLayout));
            beaconManager.BeaconParsers.Add(new BeaconParser().SetBeaconLayout(BeaconParser.EddystoneUrlLayout));

            region = new Region("Common", null, null, null);
            //BeaconManager.SetDebug(true);
            BeaconManager.AndroidLScanningDisabled = true;

            ChangeManagerScanPeriods();

            beaconManager.Bind(this);


            

            bleManager = (BluetoothManager)GetSystemService(Context.BluetoothService);
            bleManager.Adapter.Enable();
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
            
            beaconManager.RangingNotifiers.Add(RangeNotifier);
            beaconManager.StartRangingBeaconsInRegion(region);

            Toast.MakeText(this.ApplicationContext, "Beacon Scanner started.", ToastLength.Short).Show();
        }



        public override void OnDestroy()
        {
            base.OnDestroy();
            _rangeNotifier = null;
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

        //public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        //{

        //    if(beacons.Count > 0)
        //    {

        //    }

        //    System.Diagnostics.Debug.WriteLine("BEACONS COUNT: " + beacons.Count);

        //    OnStaticRanginBeacons?.Invoke(this, new BeaconScannerEventArgs {Beacons = beacons, Time = DateTime.Now });
           
        //}

        

       

    }
}