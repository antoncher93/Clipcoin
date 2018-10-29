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
using static Android.Provider.Settings;

namespace Clipcoin.Phone.Help
{
    public static class CommonHelp
    {
        public static string GetDeviceId(Context context)
        {
            string result = "";

            result = Secure.GetString(context.ContentResolver, Secure.AndroidId);

            return result;
        }
    }
}