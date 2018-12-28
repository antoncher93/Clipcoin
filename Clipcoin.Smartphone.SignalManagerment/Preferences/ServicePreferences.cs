using System;
using System.Collections.Generic;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Preferences
{
    public static class SetvicePreferences
    {
        public static int RssiTreshold { get; set; } = -70;
        public static string UserToken { get; set; } = "";
    }
}
