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
using Newtonsoft.Json;

namespace Clipcoin.Phone.Services.Classes.Trackers
{
    public class TrackerData
    {
        [JsonProperty(PropertyName = "beacons")]
        public ICollection<string> Beacons { get; set; }

        [JsonProperty(PropertyName = "ap_uid")]
        public string Uid { get; set; }
    }
}