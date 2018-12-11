using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface ITracker
    {
        IAccessPoint AccessPoint { get; set; }

        string Uid { get; }
        string Key { get; set; }
        string UUID { get; }
        bool IsObsolete { get; }
    }
}