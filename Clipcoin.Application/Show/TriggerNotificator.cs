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
using Trigger;
using Trigger.Beacons;
using Trigger.Classes.Beacons;
using Trigger.Enums;
using Trigger.Interfaces;
using Trigger.Signal;

namespace Clipcoin.Application.Show
{
    public class TriggerNotificator : IObserver<BeaconScanResult>
    {
        private const int RssiTreshold = -70;
        Context _ctx;
        IDisposable subscriber;
        Telemetry telemetry;
        IRanger ranger;
        NotificationManager notif_manager;
        IDictionary<DateTime, TriggerEventType> events = new Dictionary<DateTime, TriggerEventType>();

        public event EventHandler<TriggerEventArgs> OnEvent;

        public TriggerNotificator(Context ctx)
        {
            _ctx = ctx;

            telemetry = Telemetry.EmptyForUser("123");

            notif_manager = (NotificationManager)ctx.GetSystemService(Context.NotificationService);

            ranger = new RangerBuilder()
               //.AddFirstLineBeacon(BeaconBody.FromUUID(new Guid("EBEFD083-70A2-47C8-9837-E7B5634DF524")))
               .AddFirstLineBeacon(BeaconBody.FromUUID(new Guid("A07C5CA8-59EB-4EA8-9956-30B776E0FEDC")))
               //.AddFirstLineBeacon(BeaconBody.FromMac("c9:18:b1:cf:9b:50"))
               //.AddSecondLineBeacon(BeaconBody.FromUUID(new Guid("613037A8-D200-0400-FFFF-FFFFFFFFFFFF")))
               //.AddSecondLineBeacon(BeaconBody.FromUUID(new Guid("EBEFD083-70A2-47C8-9837-E7B5634DF599")))
               .AddSecondLineBeacon(BeaconBody.FromUUID(new Guid("613037a8-d200-0400-ffff-ffffffffffff")))
               //.AddSecondLineBeacon(BeaconBody.FromMac("DE:A6:78:08:52:A2"))
               //.SetCalcSlideAverageCount(3)
               //.SetAPointUid("B4B1DDB2-6941-40BE-AC8C-29F4E5043A8A")
               .Build();


            ranger.OnEvent += RangerOnEventHandler;
        }

        private void  RangerOnEventHandler(object o, TriggerEventArgs e)
        {
            if(!events.ContainsKey(e.Timespan))
            {
                events.Add(e.Timespan, e.Type);
                OnEvent?.Invoke(this, e);
            }
        }

        public IRanger Ranger => ranger;

        public void OnCompleted()
        {
            subscriber.Dispose();
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(BeaconScanResult value)
        {
            var list = value.Signals.Where(s => s.Rssi>=RssiTreshold)
                .Select(b => BeaconData.FromAddress(b.UUID)
              .Add(new BeaconItem[] { new BeaconItem { Rssi = b.Rssi, Time = value.Time } }));

            foreach (var s in list)
            {
                telemetry.Append(s);
            }

            ranger.OnNext(telemetry);
        }

        public void Subscribe(IObservable<BeaconScanResult> provider)
        {
            subscriber = provider?.Subscribe(this);
        }
    }
}