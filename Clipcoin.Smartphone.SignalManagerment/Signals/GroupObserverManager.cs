﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using System.Timers;
using Clipcoin.Smartphone.SignalManagement.Logging;
using Clipcoin.Smartphone.SignalManagement.Preferences;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public class GroupObserverManager : BaseObserver<BeaconSignal>
    {
        private int treshold = SetvicePreferences.RssiTreshold;
        private string _uuid;

        private readonly string _guid = Guid.NewGuid().ToString();

        public GroupObserverManager(string uuid)
        {
            _uuid = uuid;
        }

        public IEnumerable<IGroupObserver> GroupObservers => _groupObservers.Values;

        private IDictionary<GroupID, IGroupObserver> _groupObservers = new Dictionary<GroupID, IGroupObserver>();

        public override void OnNext(BeaconSignal value)
        {
            base.OnNext(value);
            if(value.Rssi >= treshold)
            {
                PushToObservers(value);
            }
        }        

        private void PushToObservers(BeaconSignal signal)
        {
            if(signal.UUID.Equals(_uuid, StringComparison.CurrentCultureIgnoreCase))
            {
                IGroupObserver observer;
                GroupID id = GroupID.FromValues(signal.Minor, signal.Major);
                if (_groupObservers.ContainsKey(id))
                {
                    observer = _groupObservers[id];
                }
                else
                {
                    observer = GroupObserverFactory.GetInstance(id);
                    observer.Timer.Elapsed += (s, e) =>
                    {
                        OnTimerElapsed(observer);
                    };
                    _groupObservers.Add(observer.GroupID, observer);
                    Logger.Info($"GOM {_guid} add new GO {observer.GroupID.ToString()}");

                }
                observer.SetSignal(signal);
            }
        }

        private void OnTimerElapsed(IGroupObserver observer)
        {            
            _groupObservers.Remove(observer.GroupID);
        }
    }
}
