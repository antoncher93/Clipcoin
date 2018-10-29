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
using Clipcoin.Phone.Help;
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Runnable;
using Clipcoin.Phone.Services.Classes.Beacons;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Classes.Wifi;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Signals;
using Java.Lang;
using Newtonsoft.Json;
using SCAppLibrary.Android.Services.Telemetry;
using Square.OkHttp;

namespace Clipcoin.Phone.Services.Classes  
{
    [Service]
    public class AccessPointService : Service
    {
        private WifiManager _wifiManager;
        private Timer _timer;
        private static int _interval = 500;
        private static string token;
        private bool _scanComplete;

        public string Token { get => token; set => token = value; }

        private BeaconScannerService beaconServ;
        private TelemetrySendService sendService;

        private APointManager APoints; // список активных сетей
        private NetworkKeyManager nwKeyManager; // очередь задач на запрос пароля
        private TrackerManager trackerManager;

        private ConnectTaskManager _apManager;

        private TelemetryDatabaseWriter dbWriter;


        public override IBinder OnBind(Intent intent)
        {
            return new Binder();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _scanComplete = true;
           
            _wifiManager = (WifiManager)GetSystemService(Context.WifiService);

            nwKeyManager = new NetworkKeyManager();
            APoints = new APointManager();
            trackerManager = new TrackerManager();
            beaconServ = new BeaconScannerService();


            sendService = new TelemetrySendService
            {
                Token = token
            };

            _apManager = new ConnectTaskManager(this, _wifiManager);
            dbWriter = new TelemetryDatabaseWriter(this);


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

            // подписались на появление новый сетей
            APoints.NewAccessPoint += (s, e) =>
            {
                nwKeyManager.Add(new RequestKeyTask(e.AccessPoint, token));
            };

            // подписались на получение пароля нового трекера
            nwKeyManager.OnFindTracker += (s, e) =>
            {
                var task = new ConnectionTask(_wifiManager, e);
                _apManager.Add(task);

               
            };

            _apManager.TrackerConnectionComplete += (s, e) =>
            {
                if(e.Code.Equals(200))
                {
                    if (!Tools.IsServiceRunning(this, beaconServ.Class))
                    {
                        StartService(new Intent(this, beaconServ.Class));
                    }

                    if (!Tools.IsServiceRunning(this, sendService.Class))
                    {
                        StartService(new Intent(this, sendService.Class));
                    }

                    var data = JsonConvert.DeserializeObject<TrackerData>(e.Body);

                    TrackerBuilder tb = new TrackerBuilder();
                    tb.Uid(data.Uid);
                    foreach(var b in data.Beacons)
                    {
                        tb.Beacon(b);
                    }
                    tb.AccessPoint(e.AccessPoint);

                    ITracker t = tb.Build();

                    trackerManager.Add(t);
                }
            };

            APoints.OnNetworDisappear += (s, e) =>
            {
                trackerManager.FirstOrDefault(t => t.AccessPoint.Bssid == e.AccessPoint.Bssid);
            };


            BeaconScannerService.OnStaticRanginBeacons += (s, e) =>
            {
                foreach(var beacon in e.Beacons)
                {
                    if( trackerManager.Any())
                    {
                        dbWriter.NewBeaconSignal(beacon, trackerManager.FirstOrDefault().Uid, e.Time);
                    }
                }
            };

            trackerManager.OnNewTracker += (s, e) =>
            {
                if (!Tools.IsServiceRunning(this, beaconServ.Class))
                {
                    StartService(new Intent(this, beaconServ.Class));
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
            _timer.Stop();
            Logger.Info("Main Service Destroyed");
            if (Tools.IsServiceRunning(this, beaconServ.Class))
            {
                StopService(new Intent(this, beaconServ.Class));
            }

            if (Tools.IsServiceRunning(this, sendService.Class))
            {
                StopService(new Intent(this, sendService.Class));
            }

            base.OnDestroy();
        }

        public void TimerTask(object sender, EventArgs e)
        {
            _wifiManager.StartScan();

            foreach(var res in _wifiManager.ScanResults)
            {
                Thread.Sleep(50);

                APoints.Add(new APointInfo { Bssid = res.Bssid, Ssid = res.Ssid });
            }

            _scanComplete = true;
        }
    }
}