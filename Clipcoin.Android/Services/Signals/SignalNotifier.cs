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
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Newtonsoft.Json;

namespace Clipcoin.Phone.Services.Signals
{
    public class SignalNotifier : BaseObservable<BeaconSignal>, ISignalProvider
    {
        //private ConcurrentStack<BleSignal> _signalStack = new ConcurrentStack<BleSignal>();

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

            if (result)
            {
                var s = JsonConvert.SerializeObject(record);
            }
                

            return result;
        }

        private string GetUuidFromRecord(byte[] record)
        {
            return string.Empty;
        }

        //private IEnumerable<BleSignal> PopAll()
        //{
        //    var buffer = new BleSignal[_signalStack.Count];
        //    _signalStack.TryPopRange(buffer, 0, buffer.Length);
        //    return buffer;
        //}

        public void OnBleSignal(BleSignal signal)
        {
            var beaconSignal = GetBeaconsFromSignals(signal);

            if(!beaconSignal.Equals(BeaconSignal.Default))
            {
                NotifyAllAsync(beaconSignal);
            }
        }
       
    }
}