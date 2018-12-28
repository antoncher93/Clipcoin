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
    public class Brand
    {
        [JsonProperty(PropertyName ="uid")]
        public string Uid { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "productCategories")]
        public ICollection<ProductCategory> ProductCatecories { get; set; }
    }
}