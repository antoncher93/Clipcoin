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
using Clipcoin.Phone.Help;
using Clipcoin.Phone.Services.Http;
using Clipcoin.Phone.Settings;
using Clipcoin.Smartphone.SignalManagement.Interfaces;

namespace Clipcoin.Phone.Services.Signals
{
    public class SignalSender : IPackageExecutor
    {
        private readonly IApiClient _client;
        private readonly string _userID;

        public SignalSender(IApiClient client)
        {
            _client = client;
            _userID = JwtHelper.GetAspNetUserId(UserSettings.Token);
        }

        public void Execute(IEnumerable<(IBeaconSignal, DateTime)> package)
        {
            _client.SendTelemetry(_userID, package);
        }
    }
}