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
using Clipcoin.Phone.Http;
using Clipcoin.Phone.Runnable;
using Java.IO;
using Java.Lang;
using Javax.Net.Ssl;
using Newtonsoft.Json;
using Square.OkHttp;

namespace Clipcoin.Phone.Runnable
{
    public class CallTrackerTask : Java.Lang.Object, IRunnable, ICallback
    {
        private string _ipAddress;

        private OkHttpClient client = new OkHttpClient();

        private string _deviceId;


        public CallTrackerTask(string IpAddress, string deviceId)
        {
            _ipAddress = "https://" +  IpAddress;
            _deviceId = deviceId;

            client.RetryOnConnectionFailure = true;
            SSLContext sslContext = SSLContext.GetInstance("SSL");
            ITrustManager[] trustAllCerfs = new ITrustManager[] { new X509TrustManager() };
            sslContext.Init(null, trustAllCerfs, new Java.Security.SecureRandom());
            client.SetSslSocketFactory(sslContext.SocketFactory);
            client.SetHostnameVerifier(new HostNameVerifier());
        }

        public void Run()
        {
            var data = new UserData
            {
                Version = "0.1",
                Id = "1", // id пользователя
                DeviceId = _deviceId,
                TimeStart = DateTime.Now
            };

            string json = JsonConvert.SerializeObject(data);

            RequestBody body = RequestBody.Create(MediaType.Parse("application/json; charset=utf-8"), json);

            Request request = new Request.Builder()
                .Url(_ipAddress)
                .AddHeader("Phone-Action", "hello")
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .Post(body)
                .Build();

            client.NewCall(request).Enqueue(this);
        }

        public void OnFailure(Request request, IOException iOException)
        {
            
        }

        public void OnResponse(Response response)
        {
            string body = response.Body()?.String();

            System.Diagnostics.Debug.WriteLine("TRACKER: " + body);
        }
    }
}