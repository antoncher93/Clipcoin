﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Database;
using Clipcoin.Phone.Help;
using Clipcoin.Phone.Runnable;
using Clipcoin.Phone.Services.Beacons;
using Clipcoin.Phone.Services.Demonstration;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Phone.Services.TrackerScanner;
using Clipcoin.Phone.Settings;
using Clipcoin.Smartphone.SignalManagement.Logging;
using Clipcoin.Smartphone.SignalManagement.Trackers;
using Java.Lang;
using Newtonsoft.Json;
using Square.OkHttp;

namespace Clipcoin.Phone.Services.Classes.Trackers
{
    [Service]
    public class TrackerScannerService : Service, ITrackerScanner
    {
        private WifiManager _wifiManager;
        private Timer _timer;
        private static int _interval = 500;
        private static string token;
        private static int beacon_scan_interval = 500;

        private bool _scanComplete;

        private static APointManager acessPointManager = new APointManager();
        public APointManager AccessPointManager => acessPointManager;

        public string Token
        {
            get => token;
            set
            {
                UserSettings.Token = value;
                token = value;
            }
        }

        private static Activity _activity;

        public Activity Activity { get => _activity; set => _activity = value; }

        public int BeaconScanInterval
        {
            get => beacon_scan_interval;
            set => beacon_scan_interval = value;
        }

        public override IBinder OnBind(Intent intent)
        {
            return new Binder();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _scanComplete = true;
           
            _wifiManager = (WifiManager)GetSystemService(WifiService);

            _timer = new Timer
            {
                Interval = _interval,
                Enabled = true
            };

            // задача сканирования
            _timer.Elapsed += (s, e) =>
            {
                if(_scanComplete)
                {
                    _scanComplete = false;
                    TimerTask(s, e);
                }
            };
        }

    

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _timer.Start();
            Logger.Info("Main Service Started");
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            acessPointManager.Dispose();
            _timer.Stop();
            
            Logger.Info("Main Service Destroyed");
            base.OnDestroy();
        }

        private void TimerTask(object sender, EventArgs e)
        {
            _wifiManager.StartScan();
            acessPointManager.Add(_wifiManager.ScanResults);
            acessPointManager.Update();
            _scanComplete = true;
        }
    }
}