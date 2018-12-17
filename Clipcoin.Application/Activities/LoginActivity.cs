using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Clipcoin.Application.Settings;
using Java.IO;
using Square.OkHttp;

namespace Clipcoin.Application.Activities
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity, ICallback
    {
        private const string UrlLogin = "http://technobee.elementstore.ru/api/account/login";

        UserSettings settings;
        Button btLogin;
        EditText etLogin;
        EditText etPassword;
        CheckBox chbxShowPassword;
        OkHttpClient client;
        Handler handler;
        ProgressDialog dialog;

        int _backPressedCount;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            // Create your application here

            handler = new Handler(Looper.MainLooper);
            settings = UserSettings.GetInstanceForApp(this);
            btLogin = FindViewById<Button>(Resource.Id.button_login);
            etLogin = FindViewById<EditText>(Resource.Id.editText_login);
            etPassword = FindViewById<EditText>(Resource.Id.editText_password);
            chbxShowPassword = FindViewById<CheckBox>(Resource.Id.checkBox_showPassword);

            dialog = new ProgressDialog(this);
            dialog.SetMessage("Please wait");
            dialog.Indeterminate = true;
            dialog.SetCancelable(true);

            client = new OkHttpClient();

            btLogin.Click += (s, e) => { Login(); };

            
            chbxShowPassword.CheckedChange += (s, e) =>
            {
                if (e.IsChecked)
                {
                    RunOnUiThread(() => etPassword.TransformationMethod = HideReturnsTransformationMethod.Instance);
                }
                else
                {
                    RunOnUiThread(() => etPassword.TransformationMethod = PasswordTransformationMethod.Instance);
                }
            };
        }

        private void Login()
        {
            dialog.Show();

            settings.Login = etLogin.Text;
            settings.Password = etPassword.Text;

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new LoginData { Login = etLogin.Text, Password = etPassword.Text });
            var type = MediaType.Parse("application / json; charset = utf - 8");
            RequestBody body = RequestBody.Create(type, json);

            Request request = new Request.Builder()
            .Url(UrlLogin)
            .Header("Content-Type", "application/json")
            .Post(body)
            .Build();

            client.NewCall(request).Enqueue(this);
        }

        public void OnFailure(Request request, IOException iOException)
        {
            dialog.Cancel();
            handler.Post(() =>
            {
                Toast.MakeText(this, "Fail Connection. Try again.", ToastLength.Short).Show();
            });
        }

        public void OnResponse(Response response)
        {
            dialog.Cancel();
            switch(response.Code())
            {
                case 200:
                    settings.Token = response.Body().String();
                    Clipcoin.Phone.Settings.UserSettings.Token = settings.Token;
                    handler.Post(() => StartActivity(new Intent(this, new MainLoopActivity().Class)));
                    break;
                default:
                    handler.Post(() => Toast.MakeText(this, "Fail Connection. Try again.", ToastLength.Short).Show()); 
                    break;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            _backPressedCount = 0;

            etLogin.Text = settings.Login;
            etPassword.Text = settings.Password;

            //Login();
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            _backPressedCount++;
            
            if (_backPressedCount > 1)
            {
                
                FinishAffinity();
            }
            else
            {
                Toast.MakeText(this, "Нажмите еще раз для выхода", ToastLength.Short).Show();
            }


        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var item = menu.Add(0, 1, 1, "Settings");
            item.SetIntent(new Intent(this, new CommonPreferencesActivity().Class));
            return base.OnCreateOptionsMenu(menu);
        }


    }
}