using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{ 
    public interface IAccessPoint
    {
        string Ssid { get; }
        string Bssid { get; }
        string Password { get; set; }
        DateTime LastTime { get; set; }
    }
}