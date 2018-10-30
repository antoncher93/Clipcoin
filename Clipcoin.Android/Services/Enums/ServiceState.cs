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

namespace Clipcoin.Phone.Services.Enums
{
    public enum ServiceState
    {
        Created = 0,
        Started = 1,
        Destroyed = -1
    }
}