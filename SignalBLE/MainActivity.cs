using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Clipcoin.Phone.Services.Signals;

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

            StartService(new Intent(this, new SignalScanner().Class));


        }
    }
}

