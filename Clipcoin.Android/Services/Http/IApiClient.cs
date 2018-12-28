using System;
using System.Collections.Generic;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Square.OkHttp;

namespace Clipcoin.Phone.Services.Http
{
    public interface IApiClient
    {
        void Post(string uri, string body, IDictionary<string, string> headers = null, ICallback callback = null);
        void SendTelemetry(string userID, IEnumerable<BeaconSignal> signals, ISignalSendingCallback callback = null);
        void RequestStore(string storeUid, string token, IStoreRequestCallback callback);
    }


}