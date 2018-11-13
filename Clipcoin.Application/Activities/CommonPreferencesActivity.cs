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
using static Android.Provider.CalendarContract;

namespace Clipcoin.Application.Activities
{
    [Activity(Label = "CommonPreferencesActivity")]
    public class CommonPreferencesActivity : PreferenceActivity, 
        ISharedPreferencesOnSharedPreferenceChangeListener
    {
        ISharedPreferences prefs;
        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            var pref = FindPreference(key);
            
            if(pref is EditTextPreference)
            {
                pref.Summary = (pref as EditTextPreference).Text;
            }
            
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            prefs.RegisterOnSharedPreferenceChangeListener(this);

            AddPreferencesFromResource(Resource.Xml.pref);

            foreach(var key in prefs.All.Keys)
            {
                OnSharedPreferenceChanged(prefs, key);
            }
            // Create your application here
        }


        protected override void OnResume()
        {
            base.OnResume();


        }




    }
}