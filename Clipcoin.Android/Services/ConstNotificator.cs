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
using Clipcoin.Phone.Services.Confirnation;
using Clipcoin.Phone.Settings;

namespace Clipcoin.Phone.Services
{
    public static class ConstNotificator
    {
        public static void CreateNotification(Service context, Intent activityIntent)
        {
            string channelId = CreateChannel(context);
            Notification n = BuildNotification(context, channelId, activityIntent);
            context.StartForeground(new Random().Next(1, 10), n);
        }

        private static Notification BuildNotification(Service ctx, string id, Intent activityIntent)
        {
            Notification.Action enterAct = new Notification.Action.Builder(
                Resource.Drawable.enter, 
                "ENTER", PendingIntent.GetBroadcast(ctx, 0, new Intent(Actions.ConfirmEnter), PendingIntentFlags.Immutable))
                .Build();

            Notification.Action exitAct = new Notification.Action.Builder(
              Resource.Drawable.enter,
              "EXIT", PendingIntent.GetBroadcast(ctx, 0, new Intent(Actions.ConfirmExit), PendingIntentFlags.Immutable))
              .Build();

            

            return new Notification.Builder(ctx, id)
                .SetContentTitle("ClipCoin")
                .SetContentText("Service is running")
                .SetSmallIcon(Resource.Drawable.main_icon)
                .SetStyle(new Notification.BigTextStyle())
                .AddAction(enterAct)
                .AddAction(exitAct)
                //.SetContentIntent(PendingIntent.GetActivity(ctx, 0, activityIntent, PendingIntentFlags.UpdateCurrent))
                .Build();
        }

        private static string CreateChannel(Service ctx)
        {
            string id = Guid.NewGuid().ToString();
            NotificationManager m = (NotificationManager)ctx.GetSystemService(Context.NotificationService);
            NotificationChannel channel = new NotificationChannel(id, "Tracker Channel", NotificationImportance.Default);
            m.CreateNotificationChannel(channel);
            return id;
        }
    }
}