using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace SignalTest.Services
{
    public class SignalScanner : Service, IBeaconConsumer
    {
        public void OnBeaconServiceConnect()
        {
            
        }

        public override IBinder OnBind(Intent intent) => new Binder();

        public override void OnCreate()
        {
            base.OnCreate();

            
        }
    }

    
}