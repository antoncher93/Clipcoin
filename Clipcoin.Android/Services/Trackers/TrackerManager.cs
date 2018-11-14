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
using Clipcoin.Phone.Services.Beacons;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Trackers;
using Clipcoin.Phone.Settings;
using Java.Lang;

namespace Clipcoin.Phone.Services.Trackers
{
    public class TrackerManager : ISearchTrackerTaskCallback
    {
        
        public IList<ITracker> Trakers { get; private set; } = new List<ITracker>();
        public IDictionary<string, int> CheckList { get; private set; } = new Dictionary<string, int>();

        public event EventHandler<TrackerEventArgs> OnStateChanged;

        private Guid Guid = Guid.NewGuid();

        public void CheckAccessPoints(ICollection<IAccessPoint> items)
        {
            var data = items.Where(i => !CheckList.Any(k => k.Key.Equals(i.Bssid))).ToList();

            foreach(var item in items)
            {
                if(!CheckList.ContainsKey(item.Bssid))
                {
                    CheckList.Add(item.Bssid, 0);
                }
            }

            if(data.Any())
            {
                var task = new SearchTrackerTask(this, data, UserSettings.Token);
                Java.Lang.Thread thread = new Thread(task);
                thread.Start();
            }
        }

        public void CheckAccessPoint(IAccessPoint item)
        {
            if(!CheckList.ContainsKey(item.Bssid) || 
                (CheckList.ContainsKey(item.Bssid) && CheckList[item.Bssid].Equals(500)))
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
                OnStateChanged?.Invoke(this, new TrackerEventArgs { Trackers = this.Trakers});
            }
        }

        public void OnFindTrackers(IEnumerable<ITracker> items)
        {
            foreach(var tracker in items)
            {
                if (!Trakers.Any(t => t.AccessPoint.Bssid.Equals(tracker.AccessPoint.Bssid, 
                    StringComparison.CurrentCultureIgnoreCase)))
                {
                    Trakers.Add(tracker);
                    Logger.Info("Find " + tracker.AccessPoint.Ssid);
                    OnStateChanged?.Invoke(this, new TrackerEventArgs { Trackers = this.Trakers });
                }
            }
        }
    }
}