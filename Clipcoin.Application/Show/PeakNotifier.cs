using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Application.Assets.Enums;
using Clipcoin.Application.Show.Ranger;
using Clipcoin.Phone.Services.Beacons;
using Trigger.Beacons;

namespace Clipcoin.Application.Show
{
    public class PeakNotifier : IObserver<BeaconScanResult>
    {
        Context _ctx;
        IDisposable unsubscriber;
        BeaconInfoGroup _firstLineInfo = new BeaconInfoGroup();
        BeaconInfoGroup _secondLineInfo = new BeaconInfoGroup();
        IBeaconBody _beaconA, _beaconB;
        ICollection<IOnEventListener> _listeners = new List<IOnEventListener>();

        public AppearStatus Status { get; private set; } = AppearStatus.Unknown;


        Timer timer = new Timer
        {
            Interval = 5000,
            Enabled = true
        };



        PeakNotifier(Context ctx, IBeaconBody beaconA, IBeaconBody beaconB)
        {
            _ctx = ctx;
            _beaconA = beaconA;
            _beaconB = beaconB;


            timer.Elapsed += (s, e) => { TimerTask(); };
        }

        private void TimerTask()
        {
            var max1 = _firstLineInfo.RssiPeak;
            var max2 = _secondLineInfo.RssiPeak;

            if((max2 - max1)>= TimeSpan.FromMilliseconds(5000))
            {
                NotifyListeners(EventType.Enter);
            }
            else
            {
                NotifyListeners(EventType.Exit);
            }
        }

        private void NotifyListeners(EventType type)
        {
            foreach(var l in _listeners)
            {
                l.OnEvent(type);
            }
        }

        public void Subscribe(IObservable<BeaconScanResult> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }

        public void OnCompleted()
        {
            unsubscriber.Dispose();
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(BeaconScanResult value)
        {
            //var res = value.Signals.FirstOrDefault

            Action<IBeaconBody, BeaconInfoGroup> CheckSignal = (line, infogroup) =>
            {
                var signal = value.Signals.FirstOrDefault(s => s.Mac.Equals(line.Address));

                if(signal != null)
                {
                    RestartTimer();

                    var res = infogroup.FirstOrDefault(b => b.MacAddress.Equals(signal.Mac, StringComparison.CurrentCultureIgnoreCase));

                    if(res == null)
                    {
                        res = new BeaconInfo(signal.Mac, int.MaxValue);
                        infogroup.Add(res);
                    }
                    res.Add(new BeaconItem { Rssi = signal.Rssi, Time = value.Time });
                }
            };

            CheckSignal.Invoke(_beaconA, _firstLineInfo);
            CheckSignal.Invoke(_beaconB, _secondLineInfo);
        }

        private void RestartTimer()
        {
            timer.Stop();
            timer.Start();
        }
    }
}