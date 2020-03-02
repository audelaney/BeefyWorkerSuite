using System;
using AppLogic;
using DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.AppLogic
{
    [TestClass]
    public class EncodeJobManagerBadArgTests
    {
        [ClassInitialize]
        public static void ClassStartup(TestContext context)
        {
            AppConfigManager.SetConfig("mock");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEncodeJobToQueueFailInvalidJob()
        {
            var job = new EncodeJob();

            if (job.IsValid)
            { Assert.Fail("You made a valid job bruh"); }
            else
            {
                EncodeJobManager.Instance.AddEncodeJobToQueue(job);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindEncodeJobFailEmptyGuid()
        {
            EncodeJobManager.Instance.FindEncodeJob(Guid.Empty);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MarkJobCompleteFailInvalidJob()
        {
            var job = new EncodeJob();

            if (job.IsValid)
            { Assert.Fail("You made a valid job bruh"); }
            else
            {
                EncodeJobManager.Instance.MarkJobComplete(job, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MarkJobCompleteFailInvalidGuid()
        {
            EncodeJobManager.Instance.MarkJobComplete(Guid.Empty, true);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MarkJobCheckedOutFailInvalidJob()
        {
            var job = new EncodeJob();

            if (job.IsValid)
            { Assert.Fail("You made a valid job bruh"); }
            else
            {
                EncodeJobManager.Instance.MarkJobCheckedOut(job, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MarkJobCheckedOutFailInvalidGuid()
        {
            EncodeJobManager.Instance.MarkJobCheckedOut(Guid.Empty, true);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateJobFailInvalidOldJob()
        {
            var job = new EncodeJob();
            var goodJob = new EncodeJob
            { VideoFileName = "goodvideo.mkv" };

            if (!job.IsValid && goodJob.IsValid)
            {
                EncodeJobManager.Instance.UpdateJob(job, goodJob);
            }

            Assert.Fail();
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateJobFailInvalidNewJob()
        {
            var job = new EncodeJob();
            var goodJob = new EncodeJob
            { VideoFileName = "goodvideo.mkv" };

            if (!job.IsValid && goodJob.IsValid)
            {
                EncodeJobManager.Instance.UpdateJob(goodJob, job);
            }

            Assert.Fail();
        }
    }
}