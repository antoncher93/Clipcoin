using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Beacons;
using Trigger.Enums;
using Trigger.Interfaces;
using Trigger.Rangers;
using Trigger.Signal;

namespace Clipcoin.Application.Show
{
    public class TimeTriggerNotificator : IPackageExecutor
    {
        private IRanger ranger;
        private TriggerEventArgs _lastEvent;
        private Context _ctx;
        NotificationManager notif_manager;

        public TimeTriggerNotificator(Context ctx)
        {
            _ctx = ctx;
            notif_manager = (NotificationManager)ctx.GetSystemService(Context.NotificationService);

            ranger = new TimeRangerBuilder()
                .AddFirstLineBeacon(BeaconBody.FromMac("64:cf:d9:2b:9f:8d"))
                .AddSecondLineBeacon(BeaconBody.FromMac("58:7a:62:26:c2:e4"))
                .Build();

            ranger.OnEvent += (s, e) => _lastEvent = e;
        }
        public void Execute(IEnumerable<BeaconSignal> package)
        {
            Telemetry telemetry = Telemetry.EmptyForUser("Me");

            foreach (var s in package)
            {
                var data = new BeaconData { Address = s.Mac };
                data.Add(new BeaconItem { Rssi = s.Rssi, Time = s.Time });
                telemetry.Append(data);
            }

            if(telemetry.Count > 0)
            {
                ranger.OnNext(telemetry);

                if(_lastEvent != null)
                {
                    SendNotification(_lastEvent);
                    _lastEvent = null;
                }
            }


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