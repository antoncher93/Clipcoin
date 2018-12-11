using System;
using System.Linq;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Trackers;
using Moq;
using NUnit.Framework;

namespace ClipCoinTest
{
    [TestFixture]
    public class TrackerManagerTests
    {
        [Test]
        public void CanAddTracker()
        {
            // arrange

            var trackerGuid = Guid.NewGuid();
            var trackerMock = new Mock<ITracker>();
            trackerMock.SetupGet(m => m.UUID)
                .Returns(trackerGuid.ToString());

            var targetManager = new TrackerManager();

            // act
            targetManager.OnFindTrackers(new[] { trackerMock.Object });

            // assert
            Assert.AreEqual(1, targetManager.Trakers.Count);
            var resultTracker = targetManager.Trakers.First();
            Assert.AreEqual(trackerGuid.ToString(), resultTracker);

        }
    }
}