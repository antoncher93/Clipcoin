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
            foreach(var ap in _items)
            {
                points += ap.Bssid + ";";
            }

            Request request = new Request.Builder()
               .Url(HostAddress + points)
               .AddHeader("Authorization", "Bearer " + _token)
               .Build();

            client.NewCall(request).Enqueue(this);
        }

        public void OnFailure(Request request, IOException iOException)
        {
            
        }

        public void OnResponse(Response response)
        {
            int code = response.Code();

            switch(code)
            {
                case 200:
                    string body = response.Body().String();

                    var data = new[] 
                    { new {
                        macAddressEth = "",
                        macAddressWlan0 = "",
                        macAddressWlan1 = "",
                        password = "",
                        uid = "",
                        token = ""
                    } };
            
                    data = JsonConvert.DeserializeAnonymousType(body, data);

                    var trackers = new List<ITracker>();

                    foreach(var d in data)
                    {
                        var tracker = new TrackerBuilder()
                            .AccessPoint(_items.FirstOrDefault(
                                i => i.Bssid.Equals(d.macAddressEth, StringComparison.CurrentCultureIgnoreCase) ||
                                i.Bssid.Equals(d.macAddressWlan0, StringComparison.CurrentCultureIgnoreCase) ||
                                i.Bssid.Equals(d.macAddressWlan1, StringComparison.CurrentCultureIgnoreCase)))
                            .Uid(d.uid)
                            .Build();

                        trackers.Add(tracker);
                    }

                    if (trackers.Any())
                        _callback?.OnFindTrackers(trackers);

                    break;
            }
            
        }
    }
}