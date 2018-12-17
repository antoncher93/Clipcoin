﻿using System;
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
        private ICollection<(IBeaconSignal, DateTime)> _signals = new List<(IBeaconSignal, DateTime)>();
        public GroupID GroupID { get; private set;}
        public Timer Timer { get; private set; } = new Timer() { Interval = 5000, Enabled = false };

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


        public void SetSignal(IBeaconSignal signal, DateTime time)
        {
            Timer.Stop();
            Timer.Start();
            _signals.Add((signal, time));
        }
    }
}
