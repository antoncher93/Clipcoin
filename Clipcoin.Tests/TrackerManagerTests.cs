using System;
using System.Linq;
using Clipcoin.Phone.Services.Interfaces;
using Clipcoin.Phone.Services.Trackers;
using Clipcoin.Smartphone.SignalManagement.Interfaces;
using Clipcoin.Smartphone.SignalManagement.Trackers;
using Moq;
using NUnit.Framework;

namespace ClipCoin.Tests
{
    [TestFixture]
    public class TrackerManagerTests
    {
        [Test]
        public void Can_Add_New_Tracker()
        {
            // arrange
            var trackerUUID = Guid.NewGuid();
            var trackerMock = new Mock<ITracker>();
            trackerMock.SetupGet(m => m.UUID)
                .Returns(trackerUUID.ToString());

            var targetManager = new TrackerManager();

            // act
            targetManager.OnFindTrackers(new[] { trackerMock.Object });

            // assert
            var trackersArray = targetManager.Trackers.ToArray();
            Assert.AreEqual(1, trackersArray.Length);
            var resultTracker = trackersArray.First();
            Assert.AreEqual(trackerUUID.ToString(), resultTracker.UUID);

        }

        [Test]
        public void Dont_Add_Existing_Tracker()
        {
            // arrange
            var trackerUUID = Guid.NewGuid();
            
            var trackerMock1 = new Mock<ITracker>();
            trackerMock1.SetupGet(m => m.UUID)
                .Returns(trackerUUID.ToString());

            var trackerMock2 = new Mock<ITracker>();
            trackerMock2.SetupGet(m => m.UUID)
                .Returns(trackerUUID.ToString());

            var targetManager = new TrackerManager();

            targetManager.OnFindTrackers(new[] { trackerMock1.Object });

            // act
            targetManager.OnFindTrackers(new[] { trackerMock2.Object });

            // assert
            var trackersArray = targetManager.Trackers.ToArray();
            Assert.AreEqual(1, trackersArray.Length);
            var resultTracker = trackersArray.First();
            Assert.AreEqual(trackerMock1.Object, resultTracker);
        }
    }
}