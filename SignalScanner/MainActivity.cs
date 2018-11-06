using Android.App;
using Android.Widget;
using Android.OS;
using Clipcoin.Phone.Help;
using Android.Content;
using SignalScanner.Adapters;
using System;
using Clipcoin.Phone.Services.Beacons;

namespace SignalScanner
{
    [Activity(Label = "SignalScanner", MainLauncher = true)]
    public class MainActivity : Activity, IObserver<BeaconScanResult>
    {
        ListView lvBeacons;
        BeaconAdapter adapter;
        BeaconScannerService service;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            lvBeacons = FindViewById<ListView>(Resource.Id.listView_beacons);

            service = new BeaconScannerService
            {
                ForegroundScanPeriod = 200
            };
            service.Subscribe(this);
        }

        protected override void OnResume()
        {
            base.OnResume();

            StartService(service);
        }

        protected override void OnPause()
        {
            base.OnPause();

            StopService(service);
        }

        private void StartService(Service serv)
        {
            if(!Tools.IsServiceRunning(this, serv.Class))
            {
                StartService(new Intent(this, serv.Class));
            }
        }

        private void StopService(Service serv)
        {
            if (Tools.IsServiceRunning(this, serv.Class))
            {
                StopService(new Intent(this, serv.Class));
            }
        }

        public void OnNext(BeaconScanResult value)
        {
            RunOnUiThread(() =>
            {
                adapter = new BeaconAdapter(this, value.Signals);
                lvBeacons.Adapter = adapter;
            });
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}

