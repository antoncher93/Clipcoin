using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Services.Interfaces;
using Newtonsoft.Json;

namespace Clipcoin.Phone.Services.Trackers
{
    public class Tracker : ITracker 
    {
        private const string EmptyKey = "";

        public string Uid { get; set; }
        public string Key { get; set; } = EmptyKey;
        public bool IsObsolete { get; private set; } = false;
        private IAccessPoint _apoint;
        public IAccessPoint AccessPoint
        {
            get => _apoint;
            set
            {
                _apoint = value;
            }
        }

        

        private Timer timer1 = new Timer { Interval = 3600000, Enabled = true };

        public Tracker()
        {
            timer1.Elapsed += (s, e) =>
            {
                IsObsolete = true;
                timer1.Stop();
            };
        }

        public void StartObsolete(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}