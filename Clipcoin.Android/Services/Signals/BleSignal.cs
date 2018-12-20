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

namespace Clipcoin.Phone.Services.Signals
{
    public struct BleSignal
    {
        public BleSignal(string address, int rssi, byte[] record, DateTime time)
        {
            Address = address;
            Rssi = rssi;
            Record = record;
            Time = time;
        }

        public byte[] Record { get; private set; }
        public string Address { get; private set; }
        public int Rssi { get; private set; }
        public DateTime Time { get; private set; }
    }
}