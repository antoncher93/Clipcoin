using System.Reflection;
using Android.App;
using Android.Bluetooth;
using Android.OS;
using Clipcoin.Phone.Services.Signals;
using Xamarin.Android.NUnitLite;

namespace Clipcoin.Phone.Tests
{
    [Activity(Label = "Clipcoin.Phone.Tests", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : TestSuiteActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            AddTest(Assembly.GetExecutingAssembly());
            // or in any reference assemblies
            // AddTest (typeof (Your.Library.TestClass).Assembly);

            // Once you called base.OnCreate(), you cannot add more assemblies.
            base.OnCreate(bundle);

            new SignalTests().HardSignals_Test();
        }
    }
}

