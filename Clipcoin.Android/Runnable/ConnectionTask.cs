using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Clipcoin.Phone.BroadcastReceivers.Wifi;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Logging;

namespace Clipcoin.Phone.Runnable
{
    public class ConnectionTask : Java.Lang.Object, IRunnable
    {
        private WifiManager _manager;
        public IAccessPoint AccessPoint { get; private set; }

        public ConnectionTask(WifiManager manager, IAccessPoint point)
        {
            _manager = manager;
            AccessPoint = point;
        }

        public void Run()
        {

            ConnectToNetwork();
        }


        private void ConnectToNetwork()
        {
            Logger.Info("Connecting to " + AccessPoint.Ssid);

            var formatSSid = $"\"{AccessPoint.Ssid}\"";
            var formatPassword = $"\"{AccessPoint.Password}\"";

            var wifiConfig = new WifiConfiguration
            {
                Ssid = formatSSid,
                PreSharedKey = formatPassword
            };


            var network = _manager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid == formatSSid);

            if (network == null)
            {
                var networkDesc = _manager.AddNetwork(wifiConfig);
                network = _manager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid == formatSSid);
            }

            if (network == null)
            {
                throw new System.Exception("Could not to connect:-(");
            }
            else
            {

                if (_manager.Disconnect())// просто отключаемся от текущей сети
                {
                   
                }

                //System.Diagnostics.Debug.WriteLine("TRYING TO CONNECT " + formatSSid);

                if (!_manager.EnableNetwork(network.NetworkId, true))
                {
                    Logger.Info("Fail to enable " + AccessPoint.Ssid);
                }



            }
        }

        public void ConnectionHandler(object sender, WifiConnectionEventArgs e)
        {
            if(_manager.ConnectionInfo.SupplicantState.Equals(SupplicantState.Completed)) // подключение завершено
            {
                if(_manager.ConnectionInfo.BSSID.Equals(AccessPoint.Bssid))
                {
                    Logger.Info("Connected to " + AccessPoint.Ssid);
                }
                else
                {
                }
            }
        }

    }
}