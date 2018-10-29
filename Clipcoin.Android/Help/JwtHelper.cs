using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clipcoin.Phone.Help
{
    public static class JwtHelper
    {
        private const string AspNetUserIdPayloadKey =
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public static string GetAspNetUserId(string authToken)
        {
            var jwtToken = new JwtSecurityToken(authToken);
            var aspNetUserId = jwtToken.Payload[AspNetUserIdPayloadKey].ToString();
            return aspNetUserId;
        }
    }
}