using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text;
using Java.Lang;
using Clipcoin.Phone.Services.Classes.Wifi.Events;
using Clipcoin.Phone.BroadcastReceivers.Wifi;
using static Android.Provider.Settings;
using Clipcoin.Phone.Runnable;

namespace Clipcoin.Phone.Services.Classes.Wifi
{
    public class ConnectTaskManager : ICollection<ConnectionTask>
    {
        private Context _ctx;
        private WifiManager _manager;
        private ICollection<ConnectionTask> _queue = new List<ConnectionTask>();
        private WifiConnectionReceiver _receiver = new WifiConnectionReceiver();
        

        public event EventHandler<ExchangeEventArgs> TrackerConnectionComplete;

        public ConnectTaskManager(Context context, WifiManager manager)
        {
            _manager = manager;
            IntentFilter filter = new IntentFilter(ConnectivityManager.ConnectivityAction);
            context.RegisterReceiver(_receiver, filter);
            _ctx = context;
        }

        private string TrackerIpAddress
        {

            get
            {
                string IpAdress = "";
                int serverHost = _manager.DhcpInfo.ServerAddress;
                IpAdress = Android.Text.Format.Formatter.FormatIpAddress(serverHost);
                return IpAdress;
            }
        }

        private string DeviceId
        {
            get
            {
                string result = "";
                result = Secure.GetString(_ctx.ContentResolver, Secure.AndroidId);
                return result;
            }
        }

        public int Count => _queue.Count;

        public bool IsReadOnly => _queue.IsReadOnly;

        public void Add(ConnectionTask item)
        {
            if(!_queue.Any(t => string.Equals(t.AccessPoint.Bssid, item.AccessPoint.Bssid)))
            {
                _queue.Add(item); // добавляем задачу в очередь
                _receiver.Received += item.ConnectionHandler; // подписываем задачу на системное сообщение о подключении

                // подписываемся на завершение подключения
                item.OnComplete += (s, e) =>
                {
                    if(e.Success)
                    {
                        var task = new CallTrackerTask(TrackerIpAddress, DeviceId);
                        task.OnComplete += (sender, args) =>
                        {
                            if (args.Success)
                            {
                                //событие о получении данных о биконах
                                args.AccessPoint = item.AccessPoint;
                                TrackerConnectionComplete?.Invoke(sender, args);
                            }
                        };

                        task.Run();
                    }
                    _queue.Remove(item); 
                };
                
                item.Run();
            }
        }


    #region Collection

        public void Clear()
        {
            _queue.Clear();
        }

        public bool Contains(ConnectionTask item)
        {
            return _queue.Contains(item);
        }

        public void CopyTo(ConnectionTask[] array, int arrayIndex)
        {
            _queue.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ConnectionTask> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public bool Remove(ConnectionTask item)
        {
            return _queue.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

#endregion
    }
}