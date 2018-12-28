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

namespace Clipcoin.Phone.Services.Stores
{
    public class StoreData
    {
        [JsonProperty(PropertyName ="uid")]
        public string UID { get; set; }

        [JsonProperty(PropertyName ="number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "brand")]
        public Brand Brand { get; set; }

        [JsonProperty(PropertyName = "spaceUid")]
        public string SpaceUid { get; set; }

        [JsonProperty(PropertyName = "mallUid")]
        public string MallUid { get; set; }

        [JsonProperty(PropertyName = "mallTitle")]
        public string MallTitle { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

    }
}