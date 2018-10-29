using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Logging;
using Clipcoin.Phone.Services.Classes.Wifi.Events;
using Clipcoin.Phone.Services.Interfaces;

namespace Clipcoin.Phone.Services.Classes
{
    public class APointManager// : ICollection<IAccessPoint>
    {
        public ICollection<IAccessPoint> AccessPoints { get; private set; } = new List<IAccessPoint>();
        public event EventHandler<NetworkEventArgs> NewAccessPoint;
        public event EventHandler<NetworkEventArgs> OnNetworDisappear;
        

        public void Add(IAccessPoint item)
        {
            var ap = AccessPoints.FirstOrDefault(p => string.Equals(p.Bssid, item.Bssid, StringComparison.CurrentCultureIgnoreCase));

            if(ap == null)
            {
                AccessPoints.Add(item);

                //Logger.Info("New Access Point: " + item.Ssid + " " + item.Bssid);

                item.Update();
                item.OnDisAppear += (s, e) =>
                {
                    AccessPoints.Remove((APointInfo)s);
                };

                NewAccessPoint?.Invoke(this, new NetworkEventArgs { AccessPoint = item });
            }
            else
            {
                ap.Update();
            }
        }

     
    }
}