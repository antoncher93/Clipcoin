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
using Clipcoin.Phone.Services.Beacons;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Http;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Clipcoin.Smartphone.SignalManagement.Trackers;

namespace Clipcoin.Phone.Services.Main
{
    [Service]
    public class MainService : Service,
        IObserver<TrackerEventArgs>
    {
        TrackerScannerService trackerScanner;
        BeaconScannerService beaconScanner;

        IEnumerable<Service> serviceList;

        public override IBinder OnBind(Intent intent) => new Binder();

        public void OnCompleted()
        {
            
        }

        public override void OnCreate()
        {
            base.OnCreate();
            beaconScanner = new BeaconScannerService();
            trackerScanner = new TrackerScannerService();
            serviceList = new List<Service>()
            {
                beaconScanner,
                trackerScanner
            };

            

            GroupObserverFactory.SetPackager(Packager.Instance);
            Packager.Instance.AddExecutor(new SignalSender(new ApiClient()));

        }

        public override void OnDestroy()
        {
            foreach (var serv in serviceList)
            {
                ServiceTools.StopFinally(this, serv.Class);
            }

            trackerScanner.AccessPointManager.TrackerManager.Unsubscribe(this);
            Packager.Instance.Flush();

            base.OnDestroy();
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(TrackerEventArgs value)
        {
            if(value.Count > 0)
            {
                value.NewTracker.ObserveManager.Subscribe(beaconScanner.RangeNotifier);
                ServiceTools.StartFinally(this, beaconScanner.Class);
            }
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            ConstNotificator.CreateNotification(this, null);
            StartService(new Intent(this, trackerScanner.Class));

            // subscribe to new tracker
            trackerScanner.AccessPointManager.TrackerManager.Subscribe(this);
            return base.OnStartCommand(intent, flags, startId);
        }
    }
}