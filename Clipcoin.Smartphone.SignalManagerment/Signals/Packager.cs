using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Clipcoin.Smartphone.SignalManagement.Interfaces;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public class Packager : IPackager
    {
        internal ICollection<IPackageExecutor> _executors = new List<IPackageExecutor>();

        private Packager() { }

        private static Packager instance;

        public static Packager Instance
        {
            get
            {
                if (instance == null)
                    instance = new Packager();
                return instance;
            }
        }

        public void PushPackage(IEnumerable<BeaconSignal> package)
        {
            foreach(var ex in _executors)
            {
                ex.Execute(package);
            }
        }

        public bool AddExecutor(IPackageExecutor executor)
        {
            bool result = false;
            if(!_executors.Any(ex => ex == executor))
            {
                _executors.Add(executor);
                result = true;
            }
            return result;
        }

        public void RemoveExecutor(IPackageExecutor executor)
        {
            if(_executors.Contains(executor))
            {
                _executors.Remove(executor);
            }
        }

        public void Flush()
        {
            _executors.Clear();
        }
    }
}
