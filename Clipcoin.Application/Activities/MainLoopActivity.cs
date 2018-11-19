using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
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
        TextView tvStatus;

        TriggerNotificator tn;

        IDisposable unsubscriber;
        int count;
        AppearStatus _status;


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
            tn = new TriggerNotificator(this);

            tvLog = FindViewById<TextView>(Resource.Id.textView_Log);
            tvSignalCount = FindViewById<TextView>(Resource.Id.textView_SignalsCount);
            tvTrackerValue = FindViewById<TextView>(Resource.Id.textView_TrackerScannerValue);
            tvStatus = FindViewById<TextView>(Resource.Id.textView_status_position);

            Signal_count = 0;
            tvLog.MovementMethod = new ScrollingMovementMethod();

            Logger.OnEvent += OnLoggerEvent;

            BeaconScannerService.RangeNotifier.Subscribe(this);
            //BeaconScannerService.RangeNotifier.Subscribe(tn);
            tn.Subscribe(BeaconScannerService.RangeNotifier);


            service = new TrackerScannerService
            {
                Token = settings.Token,
                RssiTreshold = new CommonSettings(this).RssiTreshold
            };

            unsubscriber = service.Subscribe(this);

            tn.Holder.OnEvent += TriggerEventHandler;

            ChangeStatus(AppearStatus.Unknown);
        }

        private void TriggerEventHandler(object s, Trigger.Signal.TriggerEventArgs e)
        {
            if(e.Type == Trigger.Enums.TriggerEventType.Enter)
            {
                ChangeStatus(AppearStatus.Inside);
            }
            else if(e.Type == Trigger.Enums.TriggerEventType.Exit)
            {
                ChangeStatus(AppearStatus.Outside);
            }
        }

        private void ChangeStatus(AppearStatus value)
        {
            if(_status != value)
            {
                switch(value)
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
            Logger.Info(error.Message);
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