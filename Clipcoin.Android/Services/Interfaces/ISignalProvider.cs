using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Services.Signals;
using Clipcoin.Smartphone.SignalManagement.Signals;

namespace Clipcoin.Phone.Services.Interfaces
{
    public interface ISignalProvider : IObservable<BeaconSignal>
    {
        void OnBleSignal(BleSignal signal);
    }
}