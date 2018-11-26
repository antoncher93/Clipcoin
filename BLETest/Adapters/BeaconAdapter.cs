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
using Clipcoin.Phone.Services.Beacons;

namespace BLETest.Adapters
{
    public class BeaconAdapter : BaseAdapter<BeaconSignal>
    {
        private IList<BeaconSignal> _beacons = new List<BeaconSignal>();
        private Context _ctx;

        public BeaconAdapter(Context ctx)
        {
            _ctx = ctx;
         
        }

        public void AppendItems(ICollection<BeaconSignal> scanResult)
        {
            foreach (var s in scanResult)
            {
                var res = _beacons.FirstOrDefault(b => b.Mac.Equals(s.Mac, StringComparison.CurrentCultureIgnoreCase));
                if (res == null)
                {
                    res = s;
                    _beacons.Add(s);
                }
                else res.Rssi = s.Rssi;
            }
        }

        public override BeaconSignal this[int position] => _beacons[position];

        public override int Count => _beacons.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ViewHolder holder = null;
            if (view != null)
                holder = view.Tag as ViewHolder;
            if(view == null)
            {
                holder = new ViewHolder();
                var inflater = _ctx.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.beacon, parent, false);
                holder.Mac = view.FindViewById<TextView>(Resource.Id.textView_beaconAddress);
                holder.Mac.Text = _beacons[position].Mac;

                holder.Rssi = view.FindViewById<TextView>(Resource.Id.textView_beaconRssi);
                holder.Rssi.Text = _beacons[position].Rssi.ToString();

                holder.Time = view.FindViewById<TextView>(Resource.Id.textView_lastTime);
                holder.Time.Text = DateTime.Now.ToString();
            }

            return view;
        }

        private class ViewHolder : Java.Lang.Object
        {
            public TextView Mac { get; set; }
            public TextView Rssi { get; set; }
            public TextView Time { get; set; }
        }
    }
}