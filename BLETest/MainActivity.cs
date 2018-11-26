using Android.App;
using Android.Widget;
using Android.OS;
using Clipcoin.Phone.Services.Beacons;
using System;
using BLETest.Adapters;

namespace BLETest
{
    [Activity(Label = "BLETest", MainLauncher = true)]
    public class MainActivity : Activity, IObserver<BeaconScanResult>
    {
        ListView lv;
        BeaconScannerService service;
        IDisposable unsubscriber;
        BeaconAdapter adapter;
        Handler handler;

        public void OnCompleted()
        {
            unsubscriber?.Dispose();
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(BeaconScanResult value)
        {
            RunOnUiThread(() =>
            {
                adapter.AppendItems(value.Signals);
                lv.Adapter = adapter;
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            handler = new Handler(Looper.MainLooper);
            lv = FindViewById<ListView>(Resource.Id.listView_beacons);
            service = new BeaconScannerService();
            unsubscriber = BeaconScannerService.RangeNotifier.Subscribe(this);
            adapter = new BeaconAdapter(this);
            
        }

        protected override void OnResume()
        {
            base.OnResume();

            StartService(new Android.Content.Intent(this, service.Class));
        }

        protected override void OnPause()
        {
            base.OnPause();

            StopService(new Android.Content.Intent(this, service.Class));
        }





    }
}

