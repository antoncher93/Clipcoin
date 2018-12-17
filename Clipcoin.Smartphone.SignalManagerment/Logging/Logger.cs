using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Logging
{
    public static class Logger
    {
        public static event EventHandler<LogEventArgs> OnEvent;
        public static void Info(string message)
        {
            OnEvent?.Invoke(null, new LogEventArgs { Message = message, Time = DateTime.Now });
            var now = DateTime.Now;
            System.Diagnostics.Debug.WriteLine($"LOGGER [{now}.{now.Millisecond}] " + message);
        }
    }
}