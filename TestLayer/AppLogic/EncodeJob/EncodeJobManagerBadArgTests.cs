using System;
using AppLogic;
using DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppConfig;
using AppConfig.Models;
using System.Collections.Generic;

namespace Tests.AppLogic
{
    [TestClass]
    public class EncodeJobManagerBadArgTests
    {
        [ClassInitialize]
        public static void ClassStartup(TestContext context)
        {
            var mockConfigModel = TestHelper.MakeConfig();
            AppConfigManager.SetConfig(mockConfigModel);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEncodeJobToQueueFailInvalidJob()
        {
            var job = TestHelper.MakeJob(valid: false);

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
            var job = TestHelper.MakeJob(valid: false);

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
            var job = TestHelper.MakeJob(valid: false);

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
            var job = TestHelper.MakeJob(valid: false);
            var goodJob = TestHelper.MakeJob(valid: true);
            var videoName = "goodvideo.mkv";

            (job.VideoFileName, goodJob.VideoFileName) = (videoName, videoName);

            if (!job.IsValid && goodJob.IsValid)
            {
                EncodeJobManager.Instance.UpdateJob(job, goodJob);
                Assert.Fail("UpdateJob accepted arguments");
            }

            Assert.Fail("Jobs have incorrect validity");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateJobFailInvalidNewJob()
        {
            var job = TestHelper.MakeJob(valid: false);
            var goodJob = TestHelper.MakeJob(valid: true);
            var videoName = "goodvideo.mkv";

            (job.VideoFileName, goodJob.VideoFileName) = (videoName, videoName);

            if (!job.IsValid && goodJob.IsValid)
            {
                EncodeJobManager.Instance.UpdateJob(goodJob, job);
            }

            Assert.Fail("Jobs have incorrect validity");
        }
    }
}