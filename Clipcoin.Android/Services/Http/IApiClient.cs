using System;
using System.Collections.Generic;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Square.OkHttp;

namespace Clipcoin.Phone.Services.Http
{
    public interface IApiClient
    {
        void Post(string uri, string body, IDictionary<string, string> headers = null, ICallback callback = null);
        void SendTelemetry(string userID, IEnumerable<(IBeaconSignal, DateTime)> signals, ISignalSendingCallback callback = null);
    }


}