using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Clipcoin.Application.Assets.Enums;
using Clipcoin.Application.Settings;
using Clipcoin.Application.Show;
using Clipcoin.Phone.Help;
using Clipcoin.Phone.Services.Beacons;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Enums;
using Clipcoin.Phone.Services.TrackerScanner;
using Square.OkHttp;
using Clipcoin.Phone.Runnable;
using Java.Lang;
using Java.IO;
using Clipcoin.Phone.Services.Main;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Clipcoin.Smartphone.SignalManagement.Logging;
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Phone.Services;
using Clipcoin.Smartphone.SignalManagement.Preferences;

namespace Clipcoin.Application.Activities
{
    [Activity(Label = "MainLoopActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleInstance, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainLoopActivity :
        Activity,
        IObserver<TrackerScanInfo>,
        IObserver<BeaconScanResult>,
        ISensorEventListener,
        ICallback

    {

        private const int RssiBoost = 5;

        TextView tvLog;
        UserSettings settings;
        CommonSettings commonSettings;
        MainService service;

        TextView tvBeacScanStatus;
        TextView tvSignalCount;
        TextView tvTrackerValue;
        TextView tvStatus;

        Button btIEnter;
        Button btIExit;


        TimeTriggerNotificator ttn;

        int count;
        AppearStatus _status;

        SensorManager sManager;
        ProgressBar prBar;
        Handler _handler;

        int Signal_count
        {
            get => count;
            set
            {
                RunOnUiThread(() =>
                {
                    tvSignalCount.Text = value.ToString();
                    count = value;
                });
            }

        }
        private void Initial()
        {
            BeaconScannerService.OnStateChanged += (s, e) =>
            {
                switch (e.State)
                {
                    case ServiceState.Started:
                        RunOnUiThread(() => tvBeacScanStatus.Text = "Scanning");
                        break;
                    case ServiceState.Destroyed:
                        RunOnUiThread(() => tvBeacScanStatus.Text = "Disable");
                        break;
                }
            };
        }

        private void InitializeComponents()
        {
            tvBeacScanStatus = FindViewById<TextView>(Resource.Id.textView_BeaconScannerStatus);

            tvLog = FindViewById<TextView>(Resource.Id.textView_Log);
            tvLog.MovementMethod = new ScrollingMovementMethod();

            tvSignalCount = FindViewById<TextView>(Resource.Id.textView_SignalsCount);

            tvTrackerValue = FindViewById<TextView>(Resource.Id.textView_TrackerScannerValue);

            tvStatus = FindViewById<TextView>(Resource.Id.textView_status_position);

            btIEnter = FindViewById<Button>(Resource.Id.button_IENTER);
            btIEnter.Click += (s, e) =>
            {
                prBar.Visibility = ViewStates.Visible;
                new Thread(new ConfirmTask("EnterStore", settings.Token, this)).Start();
            };


            btIExit = FindViewById<Button>(Resource.Id.button_IExit);
            btIExit.Click += (s, e) =>
            {
                prBar.Visibility = ViewStates.Visible;
                new Thread(new ConfirmTask("ExitStore", settings.Token, this)).Start();
            };

            prBar = new ProgressBar(this);
            prBar.Visibility = ViewStates.Invisible;

            _handler = new Handler();
        }

        private void TuneService()
        {
            service.Settings.NotifyAboutNewTracker = commonSettings.NotifyAboutNewTrackers;
        }

        private void TuneInfrastructure()
        {
            SetvicePreferences.RssiTreshold = commonSettings.RssiTreshold;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Loop);

            // Create your application here

            count = 0;

            sManager = (SensorManager)GetSystemService(Context.SensorService);
            var sensor = sManager.GetDefaultSensor(SensorType.Proximity);
            sManager.RegisterListener(this, sensor, SensorDelay.Game);

            Initial();
            InitializeComponents();

            settings = UserSettings.GetInstanceForApp(this);
            commonSettings = new CommonSettings(this);
            
            Signal_count = 0;

            Logger.OnEvent += OnLoggerEvent;

            service = new MainService();

            ttn = new TimeTriggerNotificator(this);

            Packager.Instance.AddExecutor(ttn);

            ChangeStatus(AppearStatus.Unknown);

            TuneService();
            TuneInfrastructure();
        }

        private void TriggerEventHandler(object s, Trigger.Signal.TriggerEventArgs e)
        {
            if (e.Type == Trigger.Enums.TriggerEventType.Enter)
            {
                ChangeStatus(AppearStatus.Inside);
            }
            else if (e.Type == Trigger.Enums.TriggerEventType.Exit)
            {
                ChangeStatus(AppearStatus.Outside);
            }
        }

        private void ChangeStatus(AppearStatus value)
        {
            if (_status != value)
            {
                switch (value)
                {
                    case AppearStatus.Inside:
                        RunOnUiThread(() =>
                        {
                            tvStatus.Text = "INSIDE";
                            tvStatus.SetTextColor(Color.Green);
                        });
                        break;
                    case AppearStatus.Outside:
                        RunOnUiThread(() =>
                        {
                            tvStatus.Text = "OUT";
                            tvStatus.SetTextColor(Color.Red);
                        });
                        break;
                    case AppearStatus.Unknown:
                        RunOnUiThread(() =>
                        {
                            tvStatus.Text = "UNKNOWN";
                            tvStatus.SetTextColor(Color.Silver);
                        });
                        break;
                }
                _status = value;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            //Finish();
        }

        protected override void OnResume()
        {
            StartService(new Intent(this, service.Class));

            base.OnResume();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            if (Tools.IsServiceRunning(this, service.Class))
            {
                StopService(new Intent(this, service.Class));
            }

            settings.Token = "";

            StartActivity(new Intent(this, new LoginActivity().Class));
            Finish();
        }

        protected override void OnDestroy()
        {

            base.OnDestroy();
        }

        private void OnLoggerEvent(object sender, LogEventArgs e)
        {
            RunOnUiThread(() =>
            {
                tvLog.Append($"{System.Environment.NewLine}[{e.Time.TimeOfDay}] {e.Message}");
            });
        }

        public void OnNext(TrackerScanInfo value)
        {
            RunOnUiThread(() =>
            {
                tvTrackerValue.Text = value.Trackers.Count.ToString();
            });
        }

        public void OnError(System.Exception error)
        {
            Logger.Info(error.Message);
        }

        public void OnCompleted()
        {
            
        }

        public void OnNext(BeaconScanResult value)
        {
            Signal_count = Signal_count + value.Signals.Count;
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {

        }

        public void OnSensorChanged(SensorEvent e)
        {
           
        }

        public void OnFailure(Request request, IOException iOException)
        {


            _handler.Post(() =>
            {
                prBar.Visibility = ViewStates.Invisible;
                Toast.MakeText(this, "Fail", ToastLength.Short).Show();
            });
        }

        public void OnResponse(Response response)
        {
            _handler.Post(() =>
            {
                prBar.Visibility = ViewStates.Invisible;
                Toast.MakeText(this, response.Message(), ToastLength.Short).Show();
            });
        }
    }
}