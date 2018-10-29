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
using Clipcoin.Phone.Services.Interfaces;

namespace Clipcoin.Phone.Services.Classes.Trackers
{
    public class TrackerManager : ICollection<ITracker>
    {
        private ICollection<ITracker> trakers = new List<ITracker>();

        public event EventHandler OnNewTracker;
        public event EventHandler OnTrackerRemove;

        public void Update(IAccessPoint item)
        {
            trakers.FirstOrDefault(t => t.Bssid.Equals(item.Bssid, StringComparison.CurrentCultureIgnoreCase))?.Update();
        }

        public void Add(ITracker item)
        {
            if(!trakers.Any(t => t.Uid.Equals(item.Uid, StringComparison.CurrentCultureIgnoreCase)))
            {
                trakers.Add(item);

                // удалить трекер из списка, если он устарел
                item.OnObsolete += (s, e) =>
                {
                    trakers.Remove(item);
                };

                OnNewTracker?.Invoke(this, new EventArgs());
            }
        }

        


#region Collection
        
        public int Count => trakers.Count;
        public bool IsReadOnly => trakers.IsReadOnly;
        public void Clear()
        {
            trakers.Clear();
        }
        public bool Contains(ITracker item)
        {
            return trakers.Contains(item);
        }
        public void CopyTo(ITracker[] array, int arrayIndex)
        {
            trakers.CopyTo(array, arrayIndex);
        }
        public IEnumerator<ITracker> GetEnumerator()
        {
            return trakers.GetEnumerator();
        }
        public bool Remove(ITracker item)
        {
            OnTrackerRemove?.Invoke(this, new EventArgs());
            return trakers.Remove(item);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return trakers.GetEnumerator();
        }
        
#endregion
    }
}