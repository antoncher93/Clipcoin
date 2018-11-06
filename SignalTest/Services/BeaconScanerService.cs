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

namespace SignalTest.Services
{
    [Service]
    public class BeaconScanerService : Service, IBeaconConsumer
    {
        Context IBeaconConsumer.ApplicationContext => this;
        static BeaconManager beaconManager;
        static bool bManagerIsStarted = false;
        static Region region;

        //RangeNotifier rangeNotifier;



        static string UUID = "A07C5CA8-59EB-4EA8-9956-30B776E0FEDC";

        public static int ForegroundScanPeriod { get; set; }
        static int ForegroundBetweenScanPeriod { get; set; }
        public static int BackgroundScanPeriod { get; set; }
        static int BackgroundBetweenScanPeriod { get; set; }

        //public static event Action<object, RangeEventArgs> BeaconsInRegion;

        private Notification notif;
        private NotificationManager notifManager;
        private BluetoothManager bleManager;

        //private DataBaseWriter writer;
        //private DataBaseReader reader;

        //private DataBaseService service;
        private System.Timers.Timer timer;

        public override IBinder OnBind(Intent intent)
        {
            return new Binder();
        }



        private void SendStartNotification()
        {
            notif = new Notification.Builder(this)
                //.SetSmallIcon(Resource.Drawable.beacon)
                .SetContentTitle("SHOPPERCOIN")
                .SetContentText("Beacon Scanner has been started.")
                .Build();

            notifManager.Notify(1, notif);
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //string filebase = "/data/user/0/SHOPPERCOIN.SHOPPERCOIN/databases/SCDataBase";
            //if (File.Exists(filebase))
            //{
            //    File.Delete(filebase);
            //}




            notifManager = (NotificationManager)GetSystemService(Context.NotificationService);

            beaconManager = BeaconManager.GetInstanceForApplication(this);
            beaconManager.BeaconParsers.Add(new BeaconParser()
                .SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:25-25"));

            //writer = new DataBaseWriter(this);

            beaconManager.Bind(this);

            SetScanPeriods(200, 0, 2000, 0);

            bleManager = (BluetoothManager)GetSystemService(Context.BluetoothService);

            bleManager.Adapter.Enable();

            //service = new DataBaseService();



        }



        public static void SetScanPeriods(
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

        private static void ChangeManagerScanPeriods()
        {
            beaconManager.SetForegroundScanPeriod(ForegroundScanPeriod);
            beaconManager.SetForegroundBetweenScanPeriod(ForegroundBetweenScanPeriod);
            beaconManager.SetBackgroundScanPeriod(BackgroundScanPeriod);
            beaconManager.SetForegroundBetweenScanPeriod(BackgroundBetweenScanPeriod);

            if (bManagerIsStarted)
            {
                //beaconManager.StartMonitoringBeaconsInRegion(region);
                beaconManager.StartRangingBeaconsInRegion(region);
            }
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

            //RangeNotifier.Instance.DidRangeBeaconsComplete += NewBeaconsInfoHandler;
            //RangeNotifier.Instance.DidRangeBeaconsComplete += writer.NewBeaconRangeHandler;

            //MonitorNotifier monitor = new MonitorNotifier();

            region = new Region("Server", null, null, null);

            beaconManager.RangingNotifiers.Add(new Ranger());

            //beaconManager.MonitoringNotifiers.Add(monitor);

            //beaconManager.StartMonitoringBeaconsInRegion(region);

            beaconManager.StartRangingBeaconsInRegion(region);

            //StartService(new Intent(this, service.Class));

            bManagerIsStarted = true;

            Toast.MakeText(this, "Beacon Scanner started.", ToastLength.Short);
        }

        public override void OnDestroy()
        {
            //StopService(new Intent(this, service.Class));

            base.OnDestroy();


            beaconManager.StopRangingBeaconsInRegion(region);
            bManagerIsStarted = false;
            beaconManager.Unbind(this);

            //beaconManager.RangingNotifiers.Remove(rangeNotifier);

            bleManager?.Adapter.Disable();

            bleManager.Adapter.Disable();

            Toast.MakeText(this.ApplicationContext, "Beacon Service done.", ToastLength.Short).Show();
        }






        void IBeaconConsumer.UnbindService(IServiceConnection serviceConnection)
        {
            this.ApplicationContext.UnbindService(serviceConnection);
        }





       


    }
}