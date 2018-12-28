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
        [JsonProperty(PropertyName = "macAddressEth")]
        public string MacAddressEth { get; set; }

        [JsonProperty(PropertyName = "macAddressWlan0")]
        public string MacAddressWlan0 { get; set; }

        [JsonProperty(PropertyName = "macAddressWlan1")]
        public string MacAddressWlan1 { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        [JsonProperty(PropertyName = "spaceUid")]
        public string SpaceUid { get; set; }
    }
}