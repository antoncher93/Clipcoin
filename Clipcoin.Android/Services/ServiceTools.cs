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

namespace Clipcoin.Phone.Services
{
    public static class ServiceTools
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

        public static void StartFinally(Context ctx, Java.Lang.Class cls)
        {
            if(!IsServiceRunning(ctx, cls))
            {
                ctx.StartService(new Intent(ctx, cls));
            }
        }

        public static void StopFinally(Context ctx, Java.Lang.Class cls)
        {
            if (IsServiceRunning(ctx, cls))
            {
                ctx.StopService(new Intent(ctx, cls));
            }
        }
    }
}