using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clipcoin.Application.Settings
{
    public class CommonSettings
    {
        private const string DefaultRssiTreshholdKey = "rssi_treshold";
        private const string DefaultRssiTreshholdValue = "-70";

        public const string NotifyNearStorePrefKey = "NotifyNearStore";

        private Context _ctx;
        private ISharedPreferences pm;

        public CommonSettings(Context ctx)
        {
            _ctx = ctx;
            pm = PreferenceManager.GetDefaultSharedPreferences(ctx);
        }

        public bool NotifyAboutNewTrackers
        {
            get
            {
                var res = false;
                if(pm.Contains(NotifyNearStorePrefKey))
                {
                    res = pm.GetBoolean(NotifyNearStorePrefKey, false);
                }
                return res;
            }
        }

        public string FirstBeaconAddress
        {
            get
            {
                var res = _ctx.GetString(Resource.String.first_beacon_address_default_value);
                var key = _ctx.GetString(Resource.String.first_beacon_address_key);
                if (pm.Contains(key))
                {
                    res = pm.GetString(key, res);
                }
                return res;
            }
        }

        public string SecondBeaconAddress
        {
            get
            {
                return GetValue(_ctx.GetString(Resource.String.second_beacon_address_key),
                    _ctx.GetString(Resource.String.second_beacon_address_default_value));
            }
        }

        public int RssiTreshold
        {
            get
            {
                //var res = _ctx.GetString(Resource.String.rssi_treshold_default_value);
                //var key = _ctx.GetString(Resource.String.rssi_treshold_key);
                var res = DefaultRssiTreshholdValue;
                var key = DefaultRssiTreshholdKey;
                return Convert.ToInt32(GetValue(key, res));
            }
        }

        private string GetValue(string key, string defValue)
        {
            var res = defValue;
            if (pm.Contains(key))
            {
                res = pm.GetString(key, res);
            }
            return res;
        }
    }

    
}