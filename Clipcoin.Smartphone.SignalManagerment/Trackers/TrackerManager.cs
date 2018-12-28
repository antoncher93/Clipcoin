using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using Clipcoin.Smartphone.SignalManagement.Interfaces;

namespace Clipcoin.Smartphone.SignalManagement.Trackers
{
    public class TrackerManager : BaseObservable<TrackerEventArgs>
    {
        private readonly IDictionary<string, ITracker> _trackers = new Dictionary<string, ITracker>();
        public IEnumerable<ITracker> Trackers => _trackers.Values;

        public string guid = Guid.NewGuid().ToString();

        private Guid Guid = Guid.NewGuid();

        public void OnFindTrackers(IEnumerable<ITracker> items)
        {
           foreach(var tracker in items.Where(i => !_trackers.ContainsKey(i.UUID)))
            {
                _trackers.Add(tracker.UUID, tracker);

                NotifyAll(new TrackerEventArgs { NewTracker = tracker, Count = _trackers.Count });
            }
        }

        public void Dispose()
        {
            foreach(var t in _trackers.Values)
            {
                t.ObserveManager.OnCompleted();
            }
            _trackers.Clear();
        }

        
    }
}