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
    public class MainService : Service
    {
        private TrackerScannerService trackerScanner;
        private SignalScanner signalScanner;
        private IEnumerable<Service> serviceList;
        private TrackersObserver trackerObserver;
        private StoreNotifier storeNotifier;

        private static IMainServiceSettings _settings = new ServiceSettings();

        public override IBinder OnBind(Intent intent) => new Binder();

        public IMainServiceSettings Settings => _settings;

        public override void OnCreate()
        {
            base.OnCreate();
            signalScanner = new SignalScanner();
            trackerScanner = new TrackerScannerService();
            serviceList = new List<Service>()
            {
                signalScanner,
                trackerScanner
            };


            trackerObserver = new TrackersObserver(this);
            trackerObserver.Subscribe(trackerScanner.AccessPointManager.TrackerManager);

            GroupObserverFactory.SetPackager(Packager.Instance);
            Packager.Instance.AddExecutor(new SignalSender(new ApiClient()));


            if(_settings.NotifyAboutNewTracker)
            {
                storeNotifier = new StoreNotifier(this, new ApiClient());
                storeNotifier.Subscribe(trackerScanner.AccessPointManager.TrackerManager);
            }
            


        }

        public override void OnDestroy()
        {
            foreach (var serv in serviceList)
            {
                ServiceTools.StopFinally(this, serv.Class);
            }

            trackerObserver.OnCompleted();
            storeNotifier?.OnCompleted();
            Packager.Instance.Flush();

            base.OnDestroy();
        }
      

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            ConstNotificator.CreateNotification(this, null);
            StartService(new Intent(this, trackerScanner.Class));
            return base.OnStartCommand(intent, flags, startId);
        }

        private class ServiceSettings : IMainServiceSettings
        {
            public bool NotifyAboutNewTracker { get; set; }
        }
    }
}