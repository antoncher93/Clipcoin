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

namespace Clipcoin.Application.Settings
{
    public class UserSettings
    {
        private const string SettingsFileName = "UserSettings";
        private const string userToken = "UserToken";
        private const string userLogin = "UserLogin";
        private const string userPassword = "UserPassword";

        private Context context;
        private ISharedPreferences mSettings;

        private UserSettings(Context context)
        {
            this.context = context;
            mSettings = context.GetSharedPreferences(SettingsFileName, FileCreationMode.Private);
        }
        public static UserSettings GetInstanceForApp(Context context)
        {
            return new UserSettings(context);
        }
        public string Login
        {
            get
            {
                var res = "";
                if (mSettings.Contains(userLogin))
                {
                    res = mSettings.GetString(userLogin, string.Empty);
                }
                return res;
            }
            set
            {
                mSettings.Edit().PutString(userLogin, value).Apply();
            }
        }

        public string Password
        {
            get
            {
                var res = "";
                if (mSettings.Contains(userPassword))
                {
                    res = mSettings.GetString(userPassword, string.Empty);
                }
                return res;
            }
            set
            {
                mSettings.Edit().PutString(userPassword, value).Apply();
            }
        }

        public string Token
        {
            get
            {
                string res = "";
                if (mSettings.Contains(userToken))
                {
                    res = mSettings.GetString(userToken, string.Empty);
                }
                return res;
            }
            set
            {
                mSettings.Edit().PutString(userToken, value).Apply();
            }
        }
    }
}