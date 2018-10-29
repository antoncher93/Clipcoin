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

namespace Clipcoin.Phone.Runnable
{
    public class UserData
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
        [JsonProperty(PropertyName = "client_id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "device_id")]
        public string DeviceId { get; set; }
        [JsonProperty(PropertyName = "time_phone_start")]
        public DateTime TimeStart { get; set; }
        
    }
}