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
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Runnable;
using Clipcoin.Phone.Services.Beacons;
using Clipcoin.Phone.Services.Demonstration;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Phone.Services.TrackerScanner;
using Clipcoin.Phone.Settings;
using Clipcoin.Smartphone.SignalManagement.Trackers;
using Java.Lang;
using Newtonsoft.Json;
using Square.OkHttp;

namespace Clipcoin.Phone.Services.Classes.Trackers
{
    [Service]
    public class TrackerScannerService : 
        Service, 
        //IObservable<TrackerScanInfo>, 
        //IObserver<BeaconScanResult>, 
        IObserver<TrackerEventArgs>,
        ITrackerScanner
    {
        private class Unsubscriber : IDisposable
        {
            private IObserver<TrackerScanInfo> _observer;
            private IList<IObserver<TrackerScanInfo>> _observers;
            public Unsubscriber(IObserver<TrackerScanInfo> observer, IList<IObserver<TrackerScanInfo>> observers)
            {
                _observer = observer;
                _observers = observers;
            }
            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }
        
        private static int _rssiTreshold = -100;
        public const string ActionStarted = "TrackerScannerServiceStarted";


        private WifiManager _wifiManager;
        private Timer _timer;
        private static int _interval = 500;
        private static string token;
        private static int beacon_scan_interval = 500;

        private bool _scanComplete;

        //static private IList<IObserver<TrackerScanInfo>> _observers = new List<IObserver<TrackerScanInfo>>();

        private BeaconScannerService beaconServ;
        private TelemetrySendService sendService;
        private RangerService rangerServ;

        public APointManager ApManager { get; private set; } // список активных сетей

        private SignalsDBWriter dbWriter;

        public int RssiTreshold { get => _rssiTreshold; set => _rssiTreshold = value; }

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
           
            _wifiManager = (WifiManager)GetSystemService(Context.WifiService);
            ApManager = new APointManager();
            beaconServ = new BeaconScannerService();
            sendService = new TelemetrySendService();
            rangerServ = new RangerService(this);
            dbWriter = new SignalsDBWriter(this);

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

            ApManager.TrackerManager.Subscribe(this);
            //BeaconScannerService.RangeNotifier.Subscribe(this);
        }

        private void StartService(Service service)
        {
            if(!Tools.IsServiceRunning(this, service?.Class))
            {
                StartService(new Intent(this, service.Class));
            }
        }
        private void StopService(Service service)
        {
            if (Tools.IsServiceRunning(this, service?.Class))
            {
                StopService(new Intent(this, service.Class));
            }
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _timer.Start();
            Logger.Info("Main Service Started");
            
            //PendingIntent.GetActivity()
            //PendingIntent.GetBroadcast(this, 0, new Intent(ActionStarted), PendingIntentFlags.UpdateCurrent).Send();
            //var ActivityIntent = new Intent(this, _activity.Class);
            //ConstNotificator.CreateNotification(this, ActivityIntent);
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            _timer.Stop();
            Logger.Info("Main Service Destroyed");

            StopService(beaconServ);
            StopService(sendService);

            base.OnDestroy();
        }

        public void TimerTask(object sender, EventArgs e)
        {
            _wifiManager.StartScan();


            ApManager.Add(_wifiManager.ScanResults);

            ApManager.Update();
            _scanComplete = true;
        }

        //public IDisposable Subscribe(IObserver<TrackerScanInfo> observer)
        //{
        //    if(!_observers.Contains(observer))
        //    {
        //        _observers.Add(observer);
        //    }
        //    return new Unsubscriber(observer, _observers);
        //}

        //public void OnNext(BeaconScanResult value)
        //{
        //    foreach (var beacon in value.Signals.Where(s => s.Rssi>=_rssiTreshold))
        //    {
        //        if (ApManager.TrackerManager..Any())
        //        {
        //            dbWriter.NewBeaconSignal(beacon, ApManager.TrackerManager.Trakers.FirstOrDefault().Uid, value.Time);
        //        }
        //    }
        //}

        public void OnError(System.Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }

        public void OnNext(TrackerEventArgs eventArgs)
        {
            if (eventArgs.Trackers.Any())
            {
                StartService(beaconServ);
                StartService(sendService);
            }
            //TODO: else stop Service


        }
    }
}