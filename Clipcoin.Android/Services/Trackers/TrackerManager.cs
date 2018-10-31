using System;
using System.Collections;
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
using Clipcoin.Phone.Runnable;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Trackers;
using Clipcoin.Phone.Settings;
using Java.Lang;

namespace Clipcoin.Phone.Services.Trackers
{
    public class TrackerManager
    {
        
        public IList<ITracker> Trakers { get; private set; } = new List<ITracker>();
        public IDictionary<string, int> CheckList { get; private set; } = new Dictionary<string, int>();

        public event EventHandler<TrackerEventArgs> OnNewTracker;
        public event EventHandler<TrackerEventArgs> OnTrackerRemove;

        private Guid Guid = Guid.NewGuid();

        public void CheckAccessPoint(IAccessPoint item)
        {
            if(!CheckList.ContainsKey(item.Bssid))
            {
                CheckList.Add(item.Bssid, 0);
                var task = new RequestKeyTask(item, UserSettings.Token);
                task.OnComplete += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine(
                        task.AccessPoint.Ssid + 
                        $" ({task.AccessPoint.Bssid}) " + 
                        " Responce: " + 
                        e.Code + " " + e.Status.ToString());

                    if (CheckList.ContainsKey(e.AccessPoint.Bssid))
                    {
                        CheckList[e.AccessPoint.Bssid] = e.Code;
                    }

                    switch (e.Status)
                    {
                        case Enums.KeyResponceStatus.Ok:
                            Logger.Info("Found " + task.AccessPoint.Ssid);
                            ITracker tracker = new TrackerBuilder()
                            .Uid(e.Uid)
                            .AccessPoint(e.AccessPoint)
                            .Build();

                            Add(tracker);
                            break;

                        case Enums.KeyResponceStatus.Fail:
                            break;

                        case Enums.KeyResponceStatus.NotFound:
                            break;
                    }
                };
                task.Run();

            }
          
        }

        private void Add(ITracker item)
        {

            if(!Trakers.Any(t => t.Uid.Equals(item.Uid, StringComparison.CurrentCultureIgnoreCase)))
            {
                Trakers.Add(item);

                // удалить трекер из списка, если он устарел
                item.OnObsolete += (s, e) =>
                {
                    Trakers.Remove(item);
                    OnTrackerRemove?.Invoke(this, null);
                };

                OnNewTracker?.Invoke(this, new TrackerEventArgs { Tracker = item});
            }
        }
    }
}