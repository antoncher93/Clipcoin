using System;
using System.Collections.Generic;
using System.Linq;
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Stores;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Logging;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Clipcoin.Smartphone.SignalManagement.Trackers;
using Java.IO;
using Newtonsoft.Json;
using Square.OkHttp;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Beacons;
using Trigger.Signal;

namespace Clipcoin.Phone.Services.Http
{
    public class ApiClient : IApiClient
    {
        private const string apiUrl = "http://technobee.elementstore.ru";
        private const string apiCustomerPoints = "/api/Customer/points/";
        private const string apiStore = "/api/Contract/store/";
        private const string apiSeparator = ";";
        private const string apiTelemetryReceivePort = ":5000";

        private readonly MediaType _type = MediaType.Parse("application/json; charset=utf-8");

        public void FindTrackers(string token, IEnumerable<IAccessPoint> networks, ISearchTrackerTaskCallback callback = null)
        {
            var body = string.Join(apiSeparator, networks.Select(n => n.Bssid));

            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + token }
            };
            Post(apiUrl + apiCustomerPoints, $"\"{body}\"", headers, new SearchTrackerCallback(callback, networks));
        }

        public void RequestStore(string storeUid, string token, IStoreRequestCallback callback)
        {
            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + token }
            };

            Get(apiUrl + apiStore + storeUid, headers, new StoreCallback(callback));
        }

        private void Get(string uri, IDictionary<string, string> headers = null, ICallback callback = null)
        {
            OkHttpClient client = new OkHttpClient();
            var requestBuilder = new Request.Builder()
            .Url(uri);

            if (headers != null)
                foreach (var key in headers.Keys)
                {
                    requestBuilder.AddHeader(key, headers[key]);
                }

            Request request = requestBuilder
                .Build();

            if (callback == null)
                callback = new EmptyCallback();

            client.NewCall(request).Enqueue(callback);
        }

        public void Post(string uri, string body, IDictionary<string, string> headers = null, ICallback callback = null)
        {
            var requestBody = RequestBody.Create(_type, body);
            OkHttpClient client = new OkHttpClient();
            var requestBuilder = new Request.Builder()
            .Url(uri)
            .Post(requestBody);

            if (headers != null)
                foreach (var key in headers.Keys)
                {
                    requestBuilder.AddHeader(key, headers[key]);
                }

            Request request = requestBuilder
                .Build();

            if (callback == null)
                callback = new EmptyCallback();  
            
            client.NewCall(request).Enqueue(callback);
        }

        public void SendTelemetry(string userID, IEnumerable<BeaconSignal> signals, ISignalSendingCallback callback = null)
        {
            Logger.Info("Trying to send telemetry..");

            Telemetry telemetry = Telemetry.EmptyForUser(userID);

            foreach(var s in signals)
            {
                var data = new BeaconData { Address = s.Mac };
                data.Add(new BeaconItem { Rssi = s.Rssi, Time = s.Time });
                telemetry.Append(data);
            }

            if(telemetry.Count > 0)
            {
                string url = apiUrl + apiTelemetryReceivePort;
                var headers = new Dictionary<string, string>
                {
                    {"scn-dev-msg", "" }
                };

                var body = JsonConvert.SerializeObject(telemetry, new TelemetryJsonConverter());
                Post(url, body, headers, new SendingSignalCallback(callback));
            }
        }

     
        private class SearchTrackerCallback  : Java.Lang.Object, ICallback
        {
            private readonly ISearchTrackerTaskCallback _callback;
            private readonly IEnumerable<IAccessPoint> _items;

            public SearchTrackerCallback(ISearchTrackerTaskCallback callback, IEnumerable<IAccessPoint> items)
            {
                _callback = callback;
                _items = items;
            }

            public void OnFailure(Request request, IOException iOException)
            { }

            public void OnResponse(Response response)
            {
                if(response.Code().Equals(200))
                {
                    string body = response.Body().String();
                    var data = JsonConvert.DeserializeObject<TrackerData[]>(body);
                    var trackers = new List<ITracker>();

                    foreach (var d in data)
                    {
                        var tracker = new TrackerBuilder()
                            .AccessPoint(_items.FirstOrDefault(
                                i => i.Bssid.Equals(d.MacAddressEth, StringComparison.CurrentCultureIgnoreCase) ||
                                i.Bssid.Equals(d.MacAddressWlan0, StringComparison.CurrentCultureIgnoreCase) ||
                                i.Bssid.Equals(d.MacAddressWlan1, StringComparison.CurrentCultureIgnoreCase)))
                            .Uid(d.Uid)
                            .BeaconsUUID(Guid.Parse(d.UUID))
                            .SpaceUid(d.SpaceUid)
                            .Build();

                        trackers.Add(tracker);
                    }

                    if (trackers.Any())
                        _callback?.OnFindTrackers(trackers);
                }
            }
        }

        private class EmptyCallback : Java.Lang.Object, ICallback
        {
            public void OnFailure(Request request, IOException iOException)
            {
                Logger.Info($"Request failed. Request info: {request.ToString()}");
            }

            public void OnResponse(Response response)
            {
                Logger.Info($"Request complete. Response info: {response.ToString()}");
            }
        }

        private class SendingSignalCallback : Java.Lang.Object, ICallback
        {
            private readonly ISignalSendingCallback _callback;
            public SendingSignalCallback(ISignalSendingCallback callback)
            {
                _callback = callback;
            }
            public void OnFailure(Request request, IOException iOException)
            {
                _callback?.OnFail(iOException.Message);
            }

            public void OnResponse(Response response)
            {
                var body = response.Body().String();
                var msg = response.Message();

                if(response.Code() == 200)
                {
                    Logger.Info("Success");
                    _callback?.OnOK();
                }
                else
                {
                    Logger.Info("Fail");
                    _callback?.OnFail(response.Message());
                }
            }
        }

        private class StoreCallback : Java.Lang.Object, ICallback
        {
            private readonly IStoreRequestCallback _callback;

            public StoreCallback(IStoreRequestCallback callback)
            {
                _callback = callback;
            }

            public void OnFailure(Request request, IOException iOException)
            {
                
            }

            public void OnResponse(Response response)
            {
                if(response.Code() == 200)
                {
                    StoreData storeData = JsonConvert.DeserializeObject<StoreData>(response.Body().String());
                    _callback?.OnResponce(storeData);
                }
            }
        }
    }
}