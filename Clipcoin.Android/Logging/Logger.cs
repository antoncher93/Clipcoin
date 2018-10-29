using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clipcoin.Phone.Logging
{
    public static class Logger
    {
        public static event EventHandler<LogEventArgs> OnEvent;
        public static void Info(string message)
        {
            OnEvent?.Invoke(null, new LogEventArgs { Message = message, Time = DateTime.Now });
            System.Diagnostics.Debug.WriteLine($"LOGGER [{DateTime.Now}] " + message);
        }
    }
}