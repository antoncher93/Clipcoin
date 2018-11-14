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
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Services.Beacons;
using Trigger;
using Trigger.Beacons;
using Trigger.Classes.Beacons;
using Trigger.Enums;
using Trigger.Interfaces;
using Trigger.Signal;

namespace Clipcoin.Phone.Services.Demonstration
{
    [Service]
    public class RangerService : IObserver<BeaconScanResult>
    {
        Context _ctx;
        IDisposable subscriber;
        Telemetry telemetry;
        IRanger ranger;
        NotificationManager notif_manager;
        IDictionary<DateTime, TriggerEventType> events = new Dictionary<DateTime, TriggerEventType>();

        private void RangereventHandler(object sender, TriggerEventArgs e)
        {
            if (events.ContainsKey(e.Timespan))
                return;

            events.Add(e.Timespan, e.Type);

            Notification n;
            switch (e.Type)
            {
                case Trigger.Enums.TriggerEventType.Enter:
                    Logger.Info("ENTER The Region");
                    n = new Notification.Builder(_ctx)
                        .SetContentTitle("Enter the EMail Office")
                        .SetContentText($"Enter the EMail Office at {e.Timespan.ToString()}")
                        .SetSmallIcon(Resource.Drawable.enter)
                        .Build();
                    notif_manager.Notify(1, n);
                    break;

                case Trigger.Enums.TriggerEventType.Exit:
                    Logger.Info("EXIT The Region");
                    n = new Notification.Builder(_ctx)
                        .SetContentTitle("Exit the EMail Office")
                        .SetContentText($"Exit the EMail Office at {e.Timespan.ToString()}")
                        .SetSmallIcon(Resource.Drawable.exit)
                        .Build();
                    notif_manager.Notify(1, n);
                    break;
            }
        }


        public void OnCompleted()
        {
            subscriber.Dispose();
        }

        public void Subscribe(IObservable<BeaconScanResult> provider)
        {
            subscriber = provider?.Subscribe(this);
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(BeaconScanResult value)
        {
            var list = value.Signals.Select(
                b => BeaconData.FromAddress(b.UUID)
                .Add(new BeaconItem[] { new BeaconItem { Rssi = b.Rssi, Time = value.Time } }));

            foreach(var s in list)
            {
                telemetry.Append(s);
            }

            ranger.OnNext(telemetry);
        }


        public RangerService(Context ctx)
        {
            _ctx = ctx;
            telemetry = Telemetry.EmptyForUser("123");

            notif_manager = (NotificationManager)ctx.GetSystemService(Context.NotificationService);

            ranger = new RangerBuilder()
               .AddFirstLineBeacon(BeaconBody.FromUUID(new Guid("EBEFD083-70A2-47C8-9837-E7B5634DF524")))
               //.AddFirstLineBeacon(BeaconBody.FromUUID(new Guid("A07C5CA8-59EB-4EA8-9956-30B776E0FEDC")))
               //.AddFirstLineBeacon(BeaconBody.FromMac("c9:18:b1:cf:9b:50"))
               //.AddSecondLineBeacon(BeaconBody.FromUUID(new Guid("613037A8-D200-0400-FFFF-FFFFFFFFFFFF")))
               .AddSecondLineBeacon(BeaconBody.FromUUID(new Guid("EBEFD083-70A2-47C8-9837-E7B5634DF599")))
               //.AddSecondLineBeacon(BeaconBody.FromMac("DE:A6:78:08:52:A2"))
               //.SetCalcSlideAverageCount(3)
               //.SetAPointUid("B4B1DDB2-6941-40BE-AC8C-29F4E5043A8A")
               .Build();

            ranger.OnEvent += RangereventHandler;
        }


    }
}