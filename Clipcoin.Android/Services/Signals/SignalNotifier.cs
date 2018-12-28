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
    public class SignalNotifier : ScanCallback, IObservable<BeaconSignal>
    {
        private ConcurrentStack<BleSignal> _signalStack = new ConcurrentStack<BleSignal>();

        private BluetoothLeScanner _scanner;

        private Timer _timer = new Timer { Enabled = false, Interval = 2000 };

        ICollection<IObserver<BeaconSignal>> _observers = new List<IObserver<BeaconSignal>>();



        public SignalNotifier()
        {
            _timer.Elapsed += (s, e) =>
            {
                _scanner.StopScan(this);
                _scanner.StartScan(this);
                _timer.Stop();
            };
        }

        public void SetBleScanner(BluetoothLeScanner scanner)
        {
            _scanner = scanner;
        }

        internal void Complete()
        {
            _observers.Clear();
        }

        private BeaconSignal GetBeaconsFromSignals(BleSignal signal)
        {
            int startByte = 2;
            if (IsBeaconRecord(signal.Record, ref startByte))
            {
                string uuid = GetUuidFromRecord(signal.Record, startByte);
                int minor = GetMinorFromRecord(signal.Record, startByte);
                int major = GetMajorFromRecord(signal.Record, startByte);

                System.Diagnostics.Debug.WriteLine($"{signal.Address} {signal.Rssi} {uuid} {minor} {major}");

                return new BeaconSignal
                {
                    Mac = signal.Address,
                    Rssi = signal.Rssi,
                    Minor = minor,
                    UUID = uuid,
                    Major = major,
                    Time = signal.Time
                };
            }
            else return BeaconSignal.Default;


        }

        private void NotifyAllAsync(BeaconSignal signal)
        {
            System.Diagnostics.Debug.WriteLine($"{signal.Time.TimeOfDay} {signal.Mac}  {DateTime.Now.TimeOfDay}");

            foreach (var obs in _observers)
            {
                Task.Factory.StartNew(() =>
                {
                    obs.OnNext(signal);
                    
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

            System.Diagnostics.Debug.WriteLine($"*** {DateTime.Now.TimeOfDay} {result.Device.Address}");

            var time = DateTime.Now;

            var bleSignal = new BleSignal(
                       result.Device.Address,
                       result.Rssi,
                       result.ScanRecord.GetBytes(),
                       time
                       );

            var beaconSignal = GetBeaconsFromSignals(bleSignal);
            if(!beaconSignal.Equals(BeaconSignal.Default))
            {
                NotifyAllAsync(beaconSignal);
            }
        }



        public IDisposable Subscribe(IObserver<BeaconSignal> observer)
        {
            if(!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Subscriber(_observers, observer);
        }

        private class Subscriber : IDisposable
        {
            ICollection<IObserver<BeaconSignal>> _observers;
            IObserver<BeaconSignal> _observer;

            public Subscriber(ICollection<IObserver<BeaconSignal>> observers, IObserver<BeaconSignal> observer)
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