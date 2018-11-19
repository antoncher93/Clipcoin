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
using Clipcoin.Phone.Services.Classes.Trackers;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Trackers;
using Java.IO;
using Java.Lang;
using Newtonsoft.Json;
using Square.OkHttp;

namespace Clipcoin.Phone.Runnable
{
    public class SearchTrackerTask : Java.Lang.Object, IRunnable, ICallback
    {
        public string HostAddress { get; private set; } = "http://technobee.elementstore.ru/api/Customer/points/";
        private string _token;
        private OkHttpClient client = new OkHttpClient();
        private IEnumerable<IAccessPoint> _items;
        private ISearchTrackerTaskCallback _callback;
        public SearchTrackerTask(ISearchTrackerTaskCallback callback, IEnumerable<IAccessPoint> items, string token)
        {
            _callback = callback;
            _items = items;
            _token = token;
        }
        public void Run()
        {
            var points = "";

            System.Diagnostics.Debug.WriteLine("REQUEST POINTS:");

            foreach(var ap in _items)
            {
                points += ap.Bssid + ";";
                System.Diagnostics.Debug.WriteLine(ap.Bssid + " " + ap.Ssid);
            }

            Request request = new Request.Builder()
               .Url(HostAddress)
               .AddHeader("Authorization", "Bearer " + _token)
               .Post(RequestBody.Create(MediaType.Parse("application/json; charset=utf-8"), $"\"{points}\""))
               .Build();

            client.NewCall(request).Enqueue(this);
        }

        public void OnFailure(Request request, IOException iOException)
        {
            Dispose();
        }

        public new void Dispose()
        {
            _callback = null;
            base.Dispose();
        }

        public void OnResponse(Response response)
        {
            int code = response.Code();

            switch(code)
            {
                case 200:
                    string body = response.Body().String();
            
                    var data = JsonConvert.DeserializeObject<TrackerData[]>(body);

                    var trackers = new List<ITracker>();

                    foreach(var d in data)
                    {
                        var tracker = new TrackerBuilder()
                            .AccessPoint(_items.FirstOrDefault(
                                i => i.Bssid.Equals(d.MacAddressEth, StringComparison.CurrentCultureIgnoreCase) ||
                                i.Bssid.Equals(d.MacAddressWlan0, StringComparison.CurrentCultureIgnoreCase) ||
                                i.Bssid.Equals(d.MacAddressWlan1, StringComparison.CurrentCultureIgnoreCase)))
                            .Uid(d.Uid)
                            .Build();

                        trackers.Add(tracker);
                    }

                    if (trackers.Any())
                        _callback?.OnFindTrackers(trackers);

                    break;
            }

            Dispose();
            
        }
    }
}