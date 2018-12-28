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

namespace Clipcoin.Phone.Settings
{
    public abstract class BaseSettings
    {
        protected const string SettingsFileName = "CommonServiceSettings";

        protected Context Context { get; }
        protected ISharedPreferences Preferences { get; set; }

        protected BaseSettings(Context context)
        {
            Context = context;
            Preferences = context.GetSharedPreferences(SettingsFileName, FileCreationMode.Private);
        }
    }
}