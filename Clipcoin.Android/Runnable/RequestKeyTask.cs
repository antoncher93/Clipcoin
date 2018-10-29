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
using Clipcoin.Phone.Services.Classes.Wifi.Events;
using Clipcoin.Phone.Services.Enums;
using Clipcoin.Phone.Services.Interfaces;
using Java.IO;
using Square.OkHttp;

namespace Clipcoin.Phone.Runnable
{
    public class RequestKeyTask : Java.Lang.Object, Java.Lang.IRunnable, ICallback
    {
        private const string ttoken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjdXN0b21lckBmb28uYmFyIiwianRpIjoiYzA5N2MzODItNDBmMS00OWEwLTg2N2EtZmRkMDljYjFmNjkzIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxOTM3N2ZjYS0yZDVmLTRkOGEtODVmNi04YWRiMGExZThlMTIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwiZXhwIjoxNTQyMzg2MzU2LCJpc3MiOiJodHRwOi8vc2hvcHBlcmNvaW4ub3JnIiwiYXVkIjoiaHR0cDovL3Nob3BwZXJjb2luLm9yZyJ9.LOOvr2iurgXLeZcbxtOt9vV3tKbCqecMoSrVhwKGZik";
        private const string IdHostAdress = "http://technobee.elementstore.ru/api/Customer/pointpassword/";
        private OkHttpClient client = new OkHttpClient();
        private string _token;
        public IAccessPoint AccessPoint { get; private set; }
        public event EventHandler<KeyRequestEventArgs> OnComplete;
        

        public RequestKeyTask(IAccessPoint point, string token)
        {
            AccessPoint = point;
            _token = token;
        }

        public void Run()
        {
            Request request = new Request.Builder()
                .Url(IdHostAdress + AccessPoint.Bssid)
                .AddHeader("Authorization", "Bearer " + _token)
                .Build();

            client.NewCall(request).Enqueue(this);
        }

        public void OnFailure(Request request, IOException iOException)
        {
            OnComplete?.Invoke(this, new KeyRequestEventArgs { Status = KeyResponceStatus.Fail });
        } 

        public void OnResponse(Response response)
        {
            int code = response.Code();
            string body = response.Body().String();

            if(code == 200)
            {
                AccessPoint.Password = body;
                OnComplete?.Invoke(this, new KeyRequestEventArgs { Status = KeyResponceStatus.Ok, Code = code, AccessPoint = this.AccessPoint});
            }
            else
            {
                OnComplete?.Invoke(this, new KeyRequestEventArgs { Status = KeyResponceStatus.NotFound, Code = code, AccessPoint = this.AccessPoint});
            }

            
        }
    }
}