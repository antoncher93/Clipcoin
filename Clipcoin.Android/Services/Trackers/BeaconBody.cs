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
    public class BeaconBody
    {
        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        [JsonProperty(PropertyName = "serial")]
        public string Serial { get; set; }

        [JsonProperty(PropertyName = "lineType")]
        public string LineType { get; set; }

        [JsonProperty(PropertyName = "macAddress")]
        public string Mac { get; set; }
    }
}