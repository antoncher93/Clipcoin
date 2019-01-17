using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Smartphone.SignalManagement.Infrastructure;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Clipcoin.Phone.Tests
{
    [TestFixture]
    public class SignalTests
    {
        private class Obs : BaseObserver<BeaconSignal>
        {
            public ICollection<BeaconSignal> _signals = new List<BeaconSignal>();

            public override void OnNext(BeaconSignal value)
            {
                base.OnNext(value);

                _signals.Add(value);
            }
        }

        [Test]
        public void HardSignals_Test()
        {
            ISignalProvider signalProvider = new SignalNotifier();
            string s = "AgEGGv9MAAIV6+/Qg3CiR8iYN+e1Y031KAAKAAoyDQlOaWtlIFNwYWNlIOIQFmTP2SufjQAKAAqI4AAAAAA=";

            byte[] record = JsonConvert.DeserializeObject<byte[]>($"\"{s}\"");

            BleSignal signal = new BleSignal("00:00:00:00:00:00", -70, record, DateTime.Now);

           Obs _observer = new Obs();


            signalProvider.Subscribe(_observer);


            int count = 0;

            Task.Factory.StartNew(() =>
            {
                while (count < 100)
                {
                    Task.Factory.StartNew(() =>
                    {
                        signalProvider.OnBleSignal(signal);
                    });

                    count++;
                    Task.Delay(1).Wait();
                }

                int i = _observer._signals.Count;
            });

          
        }
    }
}