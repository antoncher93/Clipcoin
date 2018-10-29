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
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Services.Classes;

namespace Clipcoin.Application.Activities
{
    [Activity(Label = "MainLoopActivity")]
    public class MainLoopActivity : Activity
    {
        TextView tvLog;
        UserSettings settings;
        AccessPointService service;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Loop);

            // Create your application here

            settings = UserSettings.GetInstanceForApp(this);
            tvLog = FindViewById<TextView>(Resource.Id.textView_Log);
            tvLog.MovementMethod = new ScrollingMovementMethod();

            Logger.OnEvent += OnLoggerEvent;

            service = new AccessPointService();
            service.Token = settings.Token;


            StartService(new Intent(this, service.Class));
        }

        protected override void OnResume()
        {
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
            StopService(new Intent(this, service.Class));

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