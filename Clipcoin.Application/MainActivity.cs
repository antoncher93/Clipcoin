using Android.App;
using Android.Widget;
using Android.OS;
using Square.OkHttp;
using Java.IO;
using Clipcoin.Application.Settings;
using Clipcoin.Application.Activities;

namespace Clipcoin.Application
{
    [Activity(Label = "Clipcoin", MainLauncher = true)]
    public class MainActivity : Activity, ICallback
    {
        Button btTryAgain;
        OkHttpClient client;
        UserSettings settings;
        ProgressDialog dialog;
        Handler handler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            handler = new Handler(Looper.MainLooper);

            btTryAgain = FindViewById<Button>(Resource.Id.button_try_again);
            client = new OkHttpClient();
            settings = UserSettings.GetInstanceForApp(this);

            dialog = new ProgressDialog(this);
            dialog.SetMessage("Please wait");
            dialog.Indeterminate = true;
            dialog.SetCancelable(true);

            btTryAgain.Click += (s, e) => CallToken();
        }

        protected override void OnResume()
        {
            base.OnResume();

            btTryAgain.Visibility = Android.Views.ViewStates.Invisible;

            CallToken();
        }

        private void CallToken()
        {
            dialog.Show();

            Request request = new Request.Builder()
                   .Url("http://technobee.elementstore.ru/api/person/")
                   .AddHeader("Authorization", "Bearer " + settings.Token)
                   .Build();
            client.NewCall(request).Enqueue(this);
        }

        public void OnFailure(Request request, IOException iOException)
        {
            dialog.Cancel();
            RunOnUiThread(() => btTryAgain.Visibility = Android.Views.ViewStates.Visible);
            handler.Post(() =>
            {
                Toast.MakeText(this, "Fail Internet Connection", ToastLength.Short).Show();
            });
        }

        public void OnResponse(Response response)
        {
            dialog.Cancel();
            
            switch (response.Code())
            {
                case 200:
                    handler.Post(() =>
                    {
                        StartActivity(new Android.Content.Intent(this, new MainLoopActivity().Class));
                    });
                    break;
                default:
                    handler.Post(() =>
                    {
                        StartActivity(new Android.Content.Intent(this, new LoginActivity().Class));
                    });

                    RunOnUiThread(() => btTryAgain.Visibility = Android.Views.ViewStates.Visible);

                    break;
            }
        }
    }
}

