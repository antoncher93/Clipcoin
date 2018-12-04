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
using Java.IO;
using Java.Lang;
using Square.OkHttp;

namespace Clipcoin.Phone.Runnable
{
    public class ConfirmTask : Java.Lang.Object, IRunnable
    {
        private const string ConfirmUrl = "http://technobee.elementstore.ru/api/Contract/activities/confirm";
        OkHttpClient client = new OkHttpClient();
        string _token;
        string _confirmtype;
        private ICallback _callback;

        public ConfirmTask(string type, string token, ICallback callback)
        {
            _token = token;
            _confirmtype = type;
            _callback = callback;
        }


        public void Run()
        {
            var data = new { type = _confirmtype, timestamp = DateTime.Now, description = "" };
            var type = MediaType.Parse("application/json; charset=utf-8");
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var body = RequestBody.Create(type, str);
            Request request = new Request.Builder()
            .Url(ConfirmUrl)
            .AddHeader("Authorization", "Bearer " + _token)
            .Post(body)
            .Build();

            client.NewCall(request).Enqueue(_callback);
        }
    }
}