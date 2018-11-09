using System;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.OS;
using Clipcoin.Phone.Database;
using Clipcoin.Phone.Services.Beacons;
using Xamarin.Android.NUnitLite;


namespace Test
{
    [Activity(Label = "Test", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : TestSuiteActivity, IObserver<BeaconScanResult>
    {
        BeaconScannerService service;

        public void OnCompleted()
        {
            
        }

        public void OnError(Exception error)
        {throw new NotImplementedException();
        }

        public void OnNext(BeaconScanResult value)
        {
            System.Diagnostics.Debug.Write("=========================");
            foreach(var s in value.Signals)
            {
                System.Diagnostics.Debug.Write($"{s.Mac} {s.Rssi} {s.UUID}");
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            AddTest(Assembly.GetExecutingAssembly());
            // or in any reference assemblies
            // AddTest (typeof (Your.Library.TestClass).Assembly);

            // Once you called base.OnCreate(), you cannot add more assemblies.
            base.OnCreate(bundle);

            service = new BeaconScannerService()
            {
                ForegroundScanPeriod = 500,
                ForegroundBetweenScanPeriod = 500,
                BackgroundScanPeriod = 500,
                BackgroundBetweenScanPeriod = 500
            };

            BeaconScannerService.RangeNotifier.Subscribe(this);

            System.Diagnostics.Debug.WriteLine($"START SCAN: " + DateTime.Now.ToString());



            StartService(new Intent(this, service.Class));


        }

        
    }
}

