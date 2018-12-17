using System;
using System.Collections.Generic;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Interfaces;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public static class GroupObserverFactory
    {
        private static IPackager _packager = new BasePackager();

        public static IGroupObserver GetInstance(GroupID groupID)
        {
            return new GroupObserver(groupID, _packager);
        }

        public static void SetPackager(IPackager packager)
        {
            _packager = packager;
        }

        private class BasePackager : IPackager
        {

            public void PushPackage(IEnumerable<(IBeaconSignal signal, DateTime dateTime)> package)
            {
                
            }

            public void Flush() { }
        }
    }
}
