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
    public class EncodeJobManagerBadDBTests
    {
        [ClassInitialize]
        public static void ClassStartup(TestContext context)
        {
            var mockConfigModel = new ConfigModel()
            {
                FileAccessor = FileAccessorType.workingFake,
                DBTypeAndString = new KeyValuePair<DbType, string>(DbType.mockBadDb, "")
            };
            AppConfigManager.SetConfig(mockConfigModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AddEncodeJobToQueueFailBadDatabaseConnection()
        {
            var job = TestHelper.MakeJob(valid:true);

            var result = EncodeJobManager.Instance.AddEncodeJobToQueue(job);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void FindEncodeJobFailBadDatabaseConnection()
        {
            Guid id = Guid.NewGuid();

            var result = EncodeJobManager.Instance.FindEncodeJob(id);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void MarkJobCompleteJobFailBadDatabaseConnection()
        {
            var job = TestHelper.MakeJob(valid:true);

            var result = EncodeJobManager.Instance.MarkJobComplete(job, true);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void MarkJobCompleteGuidFailBadDatabaseConnection()
        {
            Guid id = Guid.NewGuid();

            var result = EncodeJobManager.Instance.MarkJobComplete(id, true);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void MarkJobCheckedOutJobFailBadDatabaseConnection()
        {
            var job = TestHelper.MakeJob(valid:true);

            var result = EncodeJobManager.Instance.MarkJobCheckedOut(job, true);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void MarkJobCheckedOutGuidFailBadDatabaseConnection()
        {
            Guid id = Guid.NewGuid();

            var result = EncodeJobManager.Instance.MarkJobCheckedOut(id, true);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UpdateJobFailBadDatabaseConnection()
        {
            var job = TestHelper.MakeJob(valid:true);
            var newer = job.Clone() as EncodeJob;
            newer!.VideoDirectoryPath = "/somewhere/else/";

            var result = EncodeJobManager.Instance.UpdateJob(job, newer);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void GetIncompleteUncheckedOutEncodeJobsPriorityFailBadDatabaseConnection()
        {
            var priority = 1;

            var result = EncodeJobManager.Instance.GetIncompleteUncheckedOutEncodeJobs(priority);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void GetIncompleteUncheckedOutEncodeJobsFailBadDatabaseConnection()
        {
            var result = EncodeJobManager.Instance.GetIncompleteUncheckedOutEncodeJobs();

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void GetIncompleteEncodeJobsPriorityFailBadDatabaseConnection()
        {
            var priority = 1;

            var result = EncodeJobManager.Instance.GetIncompleteEncodeJobs(priority);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void GetIncompleteEncodeJobsFailBadDatabaseConnection()
        {
            var result = EncodeJobManager.Instance.GetIncompleteEncodeJobs();

            Assert.Fail();
        }
    }
}