using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Content;
using Android.Runtime;
using System.Linq;

namespace SensorsTest
{
    [Activity(Label = "SensorsTest", MainLauncher = true)]
    public class MainActivity : Activity, ISensorEventListener
    {
        SensorManager sManager;

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //throw new System.NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            //throw new System.NotImplementedException();

            System.Diagnostics.Debug.WriteLine(e.Values[0].ToString());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            sManager = (SensorManager)GetSystemService(Context.SensorService);
            var sensors = sManager.GetSensorList(SensorType.All);
            var sensor = sManager.GetDefaultSensor(SensorType.Proximity);
            var res = sManager.RegisterListener(this, sensor, SensorDelay.Normal);
        }
    }
}

