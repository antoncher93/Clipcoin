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
using Clipcoin.Phone.Services.Classes.Trackers;

namespace Clipcoin.Phone.Services.Interfaces
{
    public interface ITracker
    {
        IAccessPoint AccessPoint { get; set; }

        string Uid { get; }
        ICollection<string> Beacons { get; }
        bool IsObsolete { get; }
        event EventHandler OnObsolete;
    }
}