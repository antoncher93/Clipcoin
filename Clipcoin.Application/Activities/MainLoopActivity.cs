using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Clipcoin.Application.Settings;
using Clipcoin.Phone.Help;
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Services.Classes;
using Clipcoin.Phone.Services.Classes.Beacons;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Enums;

namespace Clipcoin.Application.Activities
{
    [Activity(Label = "MainLoopActivity")]
    public class MainLoopActivity : Activity
    {
        TextView tvLog;
        UserSettings settings;
        TrackerScannerService service;
        Handler handler;

        TextView tvBeacScanStatus;

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

            Initial();

            tvBeacScanStatus = FindViewById<TextView>(Resource.Id.textView_BeaconScannerStatus);

            settings = UserSettings.GetInstanceForApp(this);
            tvLog = FindViewById<TextView>(Resource.Id.textView_Log);
            tvLog.MovementMethod = new ScrollingMovementMethod();

            Logger.OnEvent += OnLoggerEvent;

            service = new TrackerScannerService
            {
                Token = settings.Token
            };


            //StartService(new Intent(this, service.Class));
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
    }
}