using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Phone.Services.Signals
{
    public class SignalNotifier : ScanCallback, IObservable<BeaconScanResult>
    {
        private ConcurrentStack<BleSignal> _signalStack = new ConcurrentStack<BleSignal>();

        private Timer _timer = new Timer { Enabled = false, Interval = 500 };

        ICollection<IObserver<BeaconScanResult>> _observers = new List<IObserver<BeaconScanResult>>();

        public SignalNotifier()
        {
            _timer.Elapsed += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("=====================");

                var signals = PopAll();

                IList<BeaconSignal> beacons = GetBeaconsFromSignals(signals).ToList();

                NotifyAllAsync(new BeaconScanResult { Signals = beacons, Time = DateTime.Now });
            };
        }

        private IEnumerable<BeaconSignal> GetBeaconsFromSignals(IEnumerable<BleSignal> signals)
        {
            foreach(var signal in signals)
            {
                int startByte = 2;
                if(IsBeaconRecord(signal.Record, ref startByte))
                {
                    string uuid = GetUuidFromRecord(signal.Record, startByte);
                    int minor = GetMinorFromRecord(signal.Record, startByte);
                    int major = GetMajorFromRecord(signal.Record, startByte);

                    System.Diagnostics.Debug.WriteLine($"{signal.Address} {signal.Rssi} {uuid} {minor} {major}");

                    yield return new BeaconSignal
                    {
                        Mac = signal.Address,
                        Rssi = signal.Rssi,
                        Minor = minor,
                        UUID = uuid,
                        Major = major,
                        Time = signal.Time
                    };
                }
            }
        }

        private void NotifyAllAsync(BeaconScanResult signals)
        {
            foreach(var obs in _observers)
            {
                Task.Factory.StartNew(() =>
                {
                    obs.OnNext(signals);
                });
            }
        }

        private string GetUuidFromRecord(byte[] record, int startByte)
        {
            byte[] uuidBytes = new byte[16];
            Array.Copy(record, startByte + 4, uuidBytes, 0, uuidBytes.Length);
            var pref = BitConverter.ToString(uuidBytes).Replace("-", "");
            var result = Guid.Parse(pref).ToString();
            return result;
        }

        private int GetMinorFromRecord(byte[] record, int startByte)
        {
            int minor = (int)(record[startByte + 20] * 0x100 + record[startByte + 21]);
            return minor;
        }

        private int GetMajorFromRecord(byte[] record, int startByte)
        {
            return (int)(record[startByte + 22] * 0x100 + record[startByte + 23]);
        }



        private bool IsBeaconRecord(byte[] record, ref int startByte)
        {
            bool result = false;

            try
            {
                while (startByte <= 5)
                {
                    if (record[startByte + 2] == 0x02 &&
                        record[startByte + 3] == 0x15)
                    {
                        result = true;
                        break;
                    }
                    startByte++;
                }
            }
            catch (IndexOutOfRangeException e) { }

            return result;
        }

        private string GetUuidFromRecord(byte[] record)
        {
            return string.Empty;
        }

        private IEnumerable<BleSignal> PopAll()
        {
            var buffer = new BleSignal[_signalStack.Count];
            _signalStack.TryPopRange(buffer, 0, buffer.Length);
            return buffer;
        }

        public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);

            _timer.Start();

            var time = DateTime.Now;

            _signalStack.Push(
                   new BleSignal(
                       result.Device.Address,
                       result.Rssi,
                       result.ScanRecord.GetBytes(),
                       time
                       ));

            //if (!_signalStack.Any(s => s.Address.Equals(result.Device.Address)))
            //{
               
            //}
        }



        public IDisposable Subscribe(IObserver<BeaconScanResult> observer)
        {
            if(!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Subscriber(_observers, observer);
        }

        private class Subscriber : IDisposable
        {
            ICollection<IObserver<BeaconScanResult>> _observers;
            IObserver<BeaconScanResult> _observer;

            public Subscriber(ICollection<IObserver<BeaconScanResult>> observers, IObserver<BeaconScanResult> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if(_observers != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}