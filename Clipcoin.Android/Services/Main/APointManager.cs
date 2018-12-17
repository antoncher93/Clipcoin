using System;
using System.Collections.Generic;
using System.Linq;
using Android.Net.Wifi;
using Clipcoin.Phone.Runnable;
using Clipcoin.Phone.Services.Classes;
using Clipcoin.Phone.Services.Http;
using Clipcoin.Phone.Settings;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Trackers;
using Java.Lang;

namespace Clipcoin.Phone.Services.TrackerScanner
{
    public class APointManager : ISearchTrackerTaskCallback
    {
        public const int MAX_AP_INVISIBLE_TIME_SEC = 5;
        private readonly IDictionary<string, IAccessPoint> _accessPoints = new Dictionary<string, IAccessPoint>();

        public TrackerManager TrackerManager { get; } = new TrackerManager();

        public void Add(ICollection<ScanResult> networks)
        {
            var freshNetworks = new List<IAccessPoint>();

            foreach (var network in networks)
            {
                var macAddress = network.Bssid;
                if (_accessPoints.ContainsKey(macAddress))
                    _accessPoints[macAddress].LastTime = DateTime.Now;
                else
                    freshNetworks.Add(new APointInfo { Bssid = network.Bssid, Ssid = network.Ssid });
            }
            
            foreach (var fN in freshNetworks)
            {                
                _accessPoints.Add(fN.Bssid, fN);
            }

            //new Thread(new SearchTrackerTask(this, freshNetworks)).Start();
            if (freshNetworks.Any())
            {
                new ApiClient().FindTrackers(UserSettings.Token, freshNetworks, this);
            }

        }

        public void OnFindTrackers(IEnumerable<ITracker> items)
        {
            TrackerManager.OnFindTrackers(items);
        }

        public void Update()
        {
            _accessPoints
                .Where(ap => (ap.Value.LastTime != null)
                        && ((DateTime.Now - ap.Value.LastTime) > TimeSpan.FromSeconds(MAX_AP_INVISIBLE_TIME_SEC)))
                        .Select(oAp => _accessPoints.Remove(oAp.Key));
        }

        public void Dispose()
        {
            _accessPoints.Clear();
            TrackerManager.Dispose();
        }


    }
}