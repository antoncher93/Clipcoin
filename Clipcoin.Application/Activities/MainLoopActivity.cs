using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Clipcoin.Application.Settings;
using Clipcoin.Phone.Help;
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Services.Beacons;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Enums;
using Clipcoin.Phone.Services.TrackerScanner;

namespace Clipcoin.Application.Activities
{
    [Activity(Label = "MainLoopActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainLoopActivity : Activity, IObserver<TrackerScanInfo>, IObserver<BeaconScanResult>
    {
        TextView tvLog;
        UserSettings settings;
        TrackerScannerService service;

        TextView tvBeacScanStatus;
        TextView tvSignalCount;
        TextView tvTrackerValue;

        

        IDisposable unsubscriber;
        int count;

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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Loop);

            // Create your application here

            count = 0;

            Initial();

            tvBeacScanStatus = FindViewById<TextView>(Resource.Id.textView_BeaconScannerStatus);
           
            settings = UserSettings.GetInstanceForApp(this);


            tvLog = FindViewById<TextView>(Resource.Id.textView_Log);
            tvSignalCount = FindViewById<TextView>(Resource.Id.textView_SignalsCount);
            tvTrackerValue = FindViewById<TextView>(Resource.Id.textView_TrackerScannerValue);
            Signal_count = 0;
            tvLog.MovementMethod = new ScrollingMovementMethod();

            Logger.OnEvent += OnLoggerEvent;

            service = new TrackerScannerService
            {
                Token = settings.Token
            };
            
            unsubscriber = service.Subscribe(this);

            BeaconScannerService.RangeNotifier.Subscribe(this);
        }

        protected override void OnResume()
        {
            if(!Tools.IsServiceRunning(this, service.Class))
            {
                StartService(new Intent(this, service.Class));
            }

            base.OnResume();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            settings.Token = "";
            Finish();
        }

        protected override void OnDestroy()
        {
            if (Tools.IsServiceRunning(this, service.Class))
            {
                StopService(new Intent(this, service.Class));
            }

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

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            unsubscriber.Dispose();
        }

        public void OnNext(BeaconScanResult value)
        {
            Signal_count = Signal_count + value.Signals.Count;
        }
    }
}