using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clipcoin.Smartphone.SignalManagement.Signals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class GroupObserver_Test
    {
        [TestMethod]
        public async Task GroupObserver_Timer_test()
        {
            //arrange
            int count = 0;

            GroupObserver go = new GroupObserver(GroupID.FromValues(1, 1), null);
            go.Timer.Elapsed += (s, e) =>
            {
                count++;
            };

            go.SetSignal(new BeaconSignal { Minor = 1, Major = 1, Time = DateTime.Now});
            await Task.Delay(7000);

            Assert.AreEqual(1, count);
        }
    }
}
