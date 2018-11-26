using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Application.Settings;
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
        public int RssiTreshold { get; set; } = -70;
        Context _ctx;
        IDisposable subscriber;
        Telemetry telemetry;
        IRanger ranger;
        NotificationManager notif_manager;
        IDictionary<DateTime, TriggerEventType> events = new Dictionary<DateTime, TriggerEventType>();
        CommonSettings settings;

        public TriggerEventHolder Holder { get; private set; }
        public int Interval { get; set; } = 2000;

        public TriggerNotificator(Context ctx)
        {
            _ctx = ctx;

            settings = new CommonSettings(ctx);

            telemetry = Telemetry.EmptyForUser("123");

            notif_manager = (NotificationManager)ctx.GetSystemService(Context.NotificationService);

            ranger = new RangerBuilder()
               .AddFirstLineBeacon(BeaconBody.FromMac(settings.FirstBeaconAddress))
               .AddSecondLineBeacon(BeaconBody.FromMac(settings.SecondBeaconAddress))
               .Build();

            Holder = new TriggerEventHolder(Interval);

            ranger.OnEvent += RangerOnEventHandler;

            Holder.OnEvent += (s, e) =>
            {
                SendNotification(e);
            };

            
        }

        private void  RangerOnEventHandler(object o, TriggerEventArgs e)
        {
            if(!events.ContainsKey(e.Timespan))
            {
                events.Add(e.Timespan, e.Type);
                Holder.HoldEvent(e);
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
                .Select(b => BeaconData.FromAddress(b.Mac)
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

        public void SendNotification(TriggerEventArgs args)
        {
            Notification notif = new Notification.Builder(_ctx)
                .SetContentText($"{args.Type.ToString()} at {args.Timespan.ToString()}")
                .SetContentTitle(args.Type.ToString())
                .SetSmallIcon(args.Type == TriggerEventType.Enter ? Resource.Drawable.enter : Resource.Drawable.exit)
                .Build();
            //Android.Net.Uri.Parse($"{ContentResolver.SchemeAndroidResource}{_ctx.PackageName}{}" )
            notif.Sound = RingtoneManager.GetDefaultUri(args.Type == TriggerEventType.Enter ? 
                RingtoneType.Notification : RingtoneType.Notification);
            notif.Vibrate = new long[] { 500, 1000 };
            notif_manager.Notify(1, notif);

            
        }
    }
}