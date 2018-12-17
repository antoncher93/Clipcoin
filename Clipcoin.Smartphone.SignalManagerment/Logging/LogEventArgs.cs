using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Logging
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}