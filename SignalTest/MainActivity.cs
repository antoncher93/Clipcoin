using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using SignalTest.Services;

namespace SignalTest
{
    [Activity(Label = "SignalTest", MainLauncher = true)]
    public class MainActivity : Activity
    {
        //SignalScanner scanner;
        BeaconScanerService service;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            service = new BeaconScanerService();
        }

        protected override void OnResume()
        {
            base.OnResume();
            //service.ForegroundScanPeriod = 500;
            //service.ForegroundBetweenScanPeriod = 0;
            StartService(new Intent(this, service.Class));

            
        }
    }
}

