using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Logging;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public class GroupObserver : IGroupObserver
    {
        public readonly string _guid = Guid.NewGuid().ToString();
        private readonly IPackager _packager;
        private ICollection<BeaconSignal> _signals = new List<BeaconSignal>();
        public GroupID GroupID { get; private set;}
        public Timer Timer { get; private set; } = new Timer() { Interval = 3000, Enabled = false };

        public GroupObserver(GroupID groupId, IPackager packager)
        {
            _packager = packager;

            GroupID = groupId;

            Timer.Elapsed += (s, e) =>
            {
                Timer.Stop();
                Logger.Info($"Observer {_guid} push pack");
                _packager?.PushPackage(_signals);
            };
        }


        public void SetSignal(BeaconSignal signal)
        {
            Timer.Stop();
            Timer.Start();
            _signals.Add(signal);
        }
    }
}
