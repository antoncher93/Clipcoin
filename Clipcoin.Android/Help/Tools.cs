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

namespace Clipcoin.Phone.Help
{
    public static class Tools
    {
        public static bool IsServiceRunning(Context context, Java.Lang.Class cls)
        {
            ActivityManager manager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            var services = manager.GetRunningServices(int.MaxValue);

            foreach (var proc in services)
            {
                if (proc.Service.ClassName.Equals(cls.CanonicalName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}