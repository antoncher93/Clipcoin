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

namespace Clipcoin.Phone.Services.Classes
{
    public class APointInfo : IAccessPoint
    {
        public bool IsObsolete { get; private set; }
        public TimeSpan _lifePeriod = TimeSpan.FromSeconds(5);

        public string Ssid { get; set; }
        public string Bssid { get; set; }
        public string Password { get; set; }
        public bool Visible { get; private set; } = false;
        public event EventHandler OnDisAppear;

        public TimeSpan LifePeriod
        {
            get
            {
                _timer.Interval = LifePeriod.TotalMilliseconds;
                return _lifePeriod;
            }
            set
            {
                _lifePeriod = value;
            }
        }

        private Timer _timer;

        public APointInfo()
        {
            _timer = new Timer
            {
                Interval = _lifePeriod.TotalMilliseconds,
                Enabled = true
            };

            _timer.Elapsed += (s, e) =>
            {
                _timer.Stop();
                OnDisAppear?.Invoke(this, new EventArgs());
            };
        }

        public void Update()
        {
            Visible = true;
            _timer.Stop();
            _timer.Start();
        }
    }
}