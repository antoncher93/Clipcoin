using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using System;

namespace SignalBLE
{
    [Activity(Label = "SignalBLE", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var service = new SignalScanner();

            //service.RangeNotifier.Subscribe(new Observer());

            StartService(new Intent(this, service.Class));
           

        }

        class Observer : BaseObserver<BeaconSignal>
        {
            public override void OnNext(BeaconSignal value)
            {
                base.OnNext(value);

                //System.Diagnostics.Debug.WriteLine($"{DateTime.Now.TimeOfDay} {value.Mac} {value.Rssi}");


            }
        }
    }
}

