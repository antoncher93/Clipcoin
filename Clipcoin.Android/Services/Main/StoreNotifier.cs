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
using Clipcoin.Phone.Services.Http;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Stores;
using Clipcoin.Phone.Settings;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using Clipcoin.Smartphone.SignalManagement.Trackers;

namespace Clipcoin.Phone.Services.Main
{
    public class StoreNotifier : BaseObserver<TrackerEventArgs>, IStoreRequestCallback
    {
        private Context _context;
        private NotificationManager nm;
        private IApiClient _apiClient;
        private int _count = 1;

        public StoreNotifier(Context ctx, IApiClient apiClient)
        {
            _context = ctx;
            nm = (NotificationManager)ctx.GetSystemService(Context.NotificationService);
            _apiClient = apiClient;

        }

        public override void OnNext(TrackerEventArgs value)
        {
            base.OnNext(value);

            if(value.Count > 0)
            {
                _apiClient.RequestStore(value.NewTracker.SpaceUid, UserSettings.Token, this);
            }
        }

        public void OnResponce(StoreData storeData)
        {
            Notification notif = new Notification.Builder(_context)
                .SetContentTitle(storeData.Number)
                .SetContentText(storeData.Message)
                .SetSmallIcon(Resource.Drawable.sun)
                .Build();

            nm.Notify(_count, notif);

            _count++;
        }
    }
}