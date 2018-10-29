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

namespace Clipcoin.Phone.Services.Classes.Trackers
{
    public class Tracker : ITracker 
    {
        public string Uid { get; set; }

        public ICollection<string> Beacons { get; set; }
        public event EventHandler OnObsolete;
        public bool IsObsolete { get; private set; } = false;
        private IAccessPoint _apoint;
        public IAccessPoint AccessPoint
        {
            get => _apoint;
            set
            {
                if(_apoint != null)
                    _apoint.OnDisAppear -= StartObsolete;

                _apoint = value;
                _apoint.OnDisAppear += StartObsolete;

            }
        }

        

        private Timer timer1 = new Timer { Interval = 5000, Enabled = true };

        public Tracker()
        {
            timer1.Elapsed += (s, e) =>
            {
                IsObsolete = true;
                OnObsolete?.Invoke(this, new EventArgs());
                timer1.Stop();
            };
        }

        public void StartObsolete(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}