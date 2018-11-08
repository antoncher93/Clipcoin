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
    public class MainActivity : TestSuiteActivity
    {
        BeaconScannerService service = new BeaconScannerService
        {
            ForegroundScanPeriod = 500,
            ForegroundBetweenScanPeriod = 5000,
            BackgroundScanPeriod = 500,
            BackgroundBetweenScanPeriod = 10000
        };

        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            AddTest(Assembly.GetExecutingAssembly());
            // or in any reference assemblies
            // AddTest (typeof (Your.Library.TestClass).Assembly);

            // Once you called base.OnCreate(), you cannot add more assemblies.
            base.OnCreate(bundle);

            System.Diagnostics.Debug.WriteLine($"START SCAN: " + DateTime.Now.ToString());
            StartService(new Intent(this, service.Class));


        }

        
    }
}

