using System;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Smartphone.SignalManagement.Trackers
{
    public class TrackerBuilder
    {
        private Tracker tracker = new Tracker();

        private TrackerBuilder Modify(Action a)
        {
            a?.Invoke();
            return this;
        }

        public TrackerBuilder Uid(string uid)
            => Modify(() => tracker.Uid = uid);

        public TrackerBuilder BeaconsUUID(Guid guid)
            => Modify(()=>
            {
                tracker.UUID = guid.ToString();
                tracker.ObserveManager = new GroupObserverManager(guid.ToString());
            });

        public TrackerBuilder AccessPoint(IAccessPoint apoint)
            => Modify(() => tracker.AccessPoint = apoint);

        public Tracker Build() => tracker;

    }
}