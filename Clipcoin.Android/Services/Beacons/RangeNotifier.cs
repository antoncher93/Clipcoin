using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Database;

namespace Clipcoin.Phone.Services.Beacons
{
    public class RangeNotifier : Java.Lang.Object, IRangeNotifier, IObservable<BeaconScanResult>
    {
        private IList<IObserver<BeaconScanResult>> _observers = new List<IObserver<BeaconScanResult>>();

        public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            var now = System.DateTime.Now;
            System.Diagnostics.Debug.WriteLine($"[{now}.{now.Millisecond}] Beacons: {beacons.Count}");
            foreach (var obs in _observers)
            {
                Task.Factory.StartNew(() =>
                {
                    obs.OnNext(new BeaconScanResult
                    {
                        Signals = beacons
                    .Select(b => new BeaconSignal { Mac = b.BluetoothAddress, Rssi = b.Rssi, UUID = b.Id1.ToString(), Distance = b.Distance })
                    .ToList(),
                        Time = DateTime.Now
                    });
                });
                
            }
        }

        public IDisposable Subscribe(IObserver<BeaconScanResult> observer)
        {

            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Subscriber(_observers, observer);

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);


        }

        private class Subscriber : IDisposable
        {
            IList<IObserver<BeaconScanResult>> _observers;
            IObserver<BeaconScanResult> _observer;

            public Subscriber(IList<IObserver<BeaconScanResult>> observers, IObserver<BeaconScanResult> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}