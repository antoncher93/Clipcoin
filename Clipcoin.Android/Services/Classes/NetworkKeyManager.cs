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
using Clipcoin.Phone.Runnable;
using Clipcoin.Phone.Services.Interfaces;

namespace Clipcoin.Phone.Services.Classes
{
    public class NetworkKeyManager : ICollection<RequestKeyTask>
    {
        private ICollection<RequestKeyTask> _list = new List<RequestKeyTask>();

        public event EventHandler<IAccessPoint> OnFindTracker;

        public void Add(RequestKeyTask item)
        {
            if(!_list.Any(t => string.Equals(t.AccessPoint.Bssid, item.AccessPoint.Bssid)))
            {
                _list.Add(item);
                
                item.OnComplete += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine(item.AccessPoint.Ssid + " Responce: " + e.Code + " " + e.Status.ToString());

                    switch (e.Status)
                    {
                        case Enums.KeyResponceStatus.Ok:
                            Logger.Info("Found " + item.AccessPoint.Ssid);
                            OnFindTracker?.Invoke(this, e.AccessPoint);
                           
                            break;

                        case Enums.KeyResponceStatus.Fail:
                            break;

                        case Enums.KeyResponceStatus.NotFound:
                            break;

                            
                    }

                    if(_list.Contains(item))
                    {
                        _list.Remove(item);
                    }
                };
                item.Run();
                
            }
        }

        public int Count => _list.Count;
        public bool IsReadOnly => _list.IsReadOnly;
        public void Clear()
        {
            _list.Clear();
        }
        public bool Contains(RequestKeyTask item)
        {
            return _list.Contains(item);
        }
        public void CopyTo(RequestKeyTask[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }
        public IEnumerator<RequestKeyTask> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        public bool Remove(RequestKeyTask item)
        {
            return _list.Remove(item);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}