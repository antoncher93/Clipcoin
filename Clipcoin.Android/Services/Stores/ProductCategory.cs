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

namespace Clipcoin.Phone.Services.Stores
{
    public struct ProductCategory
    {
        public int Id { get; private set; }
        public string Title { get; private set; }

        public ProductCategory(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}