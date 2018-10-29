using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security.Cert;
using Javax.Net.Ssl;

namespace Clipcoin.Phone.Http
{
    public class X509TrustManager : Java.Lang.Object, IX509TrustManager
    {
        public void CheckClientTrusted(Java.Security.Cert.X509Certificate[] chain, string authType)
        {

        }

        public void CheckServerTrusted(Java.Security.Cert.X509Certificate[] chain, string authType)
        {

        }

        public Java.Security.Cert.X509Certificate[] GetAcceptedIssuers()
        {
            Java.Security.Cert.X509Certificate[] cArr = new Java.Security.Cert.X509Certificate[0];

            return cArr;
        }
    }
}