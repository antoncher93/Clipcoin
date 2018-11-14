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
using Clipcoin.Phone.Runnable;
using Clipcoin.Phone.Services.Classes;
using Clipcoin.Phone.Services.Interfaces;
using Java.Lang;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class RunnableTest
    {
        private class Callback : ISearchTrackerTaskCallback
        {
            public bool Complete { get; private set; } = false;
            public void OnFindTrackers(IEnumerable<ITracker> items)
            {
                Complete = true;
            }
        }
        [Test]
        public void SearchTrackerTest()
        {
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjdXN0b21lckBmb28uYmFyIiwianRpIjoiMzAyYmJlYWYtMjc5OC00NjM4LTgzNTEtZmQ4MTkyM2M5MGQ1IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxOTM3N2ZjYS0yZDVmLTRkOGEtODVmNi04YWRiMGExZThlMTIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwiZXhwIjoxNTQ0NjE2ODAzLCJpc3MiOiJodHRwOi8vc2hvcHBlcmNvaW4ub3JnIiwiYXVkIjoiaHR0cDovL3Nob3BwZXJjb2luLm9yZyJ9.TAZ463fmYbUYqZ-ajJuMjiH2_rbW1Du54tL-ItNJmQY";
            var callback = new Callback();
            var items = new List<APointInfo>
            {
                new APointInfo{Bssid = "b8:27:eb:d2:87:40"},
                new APointInfo{Bssid = "b0:39:56:4f:22:a5" }
            };

            //Resource.String.
            var task = new SearchTrackerTask(callback, items, token);
            task.Run();




            int count = 0;
            while(!callback.Complete && count < 10)
            {
                count++;
                Thread.Sleep(100);
            }

            Assert.True(callback.Complete);
        }
    }
}