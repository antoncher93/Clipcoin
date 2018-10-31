using System;
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
using Clipcoin.Phone.Services.Classes.Beacons;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Phone.Services.TrackerScanner;
using Clipcoin.Phone.Settings;
using Java.Lang;
using Newtonsoft.Json;
using Square.OkHttp;

namespace Clipcoin.Phone.Services.Classes.Trackers
{
    [Service]
    public class TrackerScannerService : Service, IObservable<TrackerScanInfo>
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

        private WifiManager _wifiManager;
        private Timer _timer;
        private static int _interval = 500;
        private static string token;
        private bool _scanComplete;

        static private IList<IObserver<TrackerScanInfo>> _observers = new List<IObserver<TrackerScanInfo>>();

        

        public string Token
        {
            get => token;
            set
            {
                UserSettings.Token = value;
                token = value;
            }
        }

        private BeaconScannerService beaconServ;
        private TelemetrySendService sendService;

        public APointManager ApManager { get; private set; } // список активных сетей
        public static event EventHandler OnTrackerScanLoop;

        private SignalsDBWriter dbWriter;


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

            ApManager.TrackerManager.OnNewTracker += (s, e) =>
            {
                StartService(beaconServ);
                StartService(sendService);
            };
            

            BeaconScannerService.OnStaticRanginBeacons += (s, e) =>
            {
                foreach(var beacon in e.Beacons)
                {
                    if( ApManager.TrackerManager.Trakers.Any())
                    {
                        dbWriter.NewBeaconSignal(beacon, ApManager.TrackerManager.Trakers.FirstOrDefault().Uid, e.Time);
                    }
                }
            };
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

            OnTrackerScanLoop?.Invoke(this, null);

            foreach(var res in _wifiManager.ScanResults)
            {
                Thread.Sleep(50);

                ApManager.Add(new APointInfo { Bssid = res.Bssid, Ssid = res.Ssid, });
            }
            ApManager.Update();
            _scanComplete = true;
        }

        public IDisposable Subscribe(IObserver<TrackerScanInfo> observer)
        {
            if(!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber(observer, _observers);
        }
    }
}