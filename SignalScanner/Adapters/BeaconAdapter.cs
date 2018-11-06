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

namespace SignalScanner.Adapters
{
    public class BeaconAdapter : BaseAdapter<BeaconSignal>
    {
        private IList<BeaconSignal> _signals = new List<BeaconSignal>();
        private Context _context;

        public BeaconAdapter(Context context, IList<BeaconSignal> signals)
        {
            _context = context;
            _signals = signals;
        }

        public override BeaconSignal this[int position] => _signals[position];

        public override int Count => _signals.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            BeaconViewHolder holder = null;
            if (view != null)
                holder = view.Tag as BeaconViewHolder;

            if(holder == null)
            {
                holder = new BeaconViewHolder();
                var inflater = _context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.beacon, parent, false);

                holder.Mac = view.FindViewById<TextView>(Resource.Id.textView_beaconMac);
                holder.Rssi = view.FindViewById<TextView>(Resource.Id.textView_beaconRssi);

                holder.Mac.Text = _signals[position].Mac;
                holder.Rssi.Text = _signals[position].Rssi.ToString();
            }

            return view;
        }

        private class BeaconViewHolder : Java.Lang.Object
        {
            public TextView Mac { get; set; }
            public TextView Rssi { get; set; }
        }
    }
}