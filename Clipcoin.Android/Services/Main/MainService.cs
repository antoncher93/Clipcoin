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
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Trackers;

namespace Clipcoin.Phone.Services.Main
{
    [Service]
    public class MainService : Service,
        IObserver<TrackerEventArgs>
    {
        TrackerScannerService trackerScanner;
        BeaconScannerService beaconScanner;

        public override IBinder OnBind(Intent intent) => new Binder();

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            beaconScanner = new BeaconScannerService();

            trackerScanner = new TrackerScannerService();
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(TrackerEventArgs value)
        {
            if(value.Trackers.Any())
            {
                StartService(new Intent(this, beaconScanner.Class));
            }

            //beaconScanner.RangeNotifier.Subscribe()
            //value.Tracker.Observer.Subscibe(RangeNotifier);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            StartService(new Intent(this, trackerScanner.Class));
            ConstNotificator.CreateNotification(this, null);

            // subscribe to new tracker
            trackerScanner.ApManager.TrackerManager.Subscribe(this);



            return base.OnStartCommand(intent, flags, startId);
        }
    }
}