using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface ISearchTrackerTaskCallback
    {
        void OnFindTrackers(IEnumerable<ITracker> items);
    }
}