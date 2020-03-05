using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppLogic;
using System;
using DataObjects;
using System.Linq;
using AppConfig.Models;
using System.Collections.Generic;
using AppConfig;

namespace Tests.AppLogic
{
    [TestClass]
    public class EncodeJobManagerTests
    {
        // Refresh the Manager and the DAO before every run.
        [TestInitialize]
        public void TestSetup()
        {
            var mockConfigModel = TestHelper.MakeConfig();
            AppConfigManager.SetConfig(mockConfigModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AddEncodeJobToQueueFailJobExistsSameGuid()
        {
            var sourceJob = TestHelper.MakeJob(valid:true);
            sourceJob.Id = Guid.NewGuid();

            if (sourceJob.IsValid)
            {
                if (EncodeJobManager.Instance.AddEncodeJobToQueue(sourceJob))
                {
                    var secondResult = EncodeJobManager.Instance.AddEncodeJobToQueue(sourceJob);

                    Assert.Fail();
                }
                else
                { Assert.Fail("First job add unsuccessful."); }
            }
            else
            { Assert.Fail("Job somehow invalid"); }
        }

        [TestMethod]
        public void AddEncodeJobToQueueSuccess()
        {
            var job = TestHelper.MakeJob(valid:true);

            if (job.IsValid)
            {
                var result = EncodeJobManager.Instance.AddEncodeJobToQueue(job);
                Assert.IsTrue(result);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void FindEncodeJobNoJobNullReturn()
        {
            Guid id = Guid.NewGuid();

            var result = EncodeJobManager.Instance.FindEncodeJob(id);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindEncodeJobSucessValidReturn()
        {
            var job = TestHelper.MakeJob(valid:true);

            if (job.IsValid)
            {
                if (EncodeJobManager.Instance.AddEncodeJobToQueue(job))
                {
                    var jobs = EncodeJobManager.Instance.GetIncompleteEncodeJobs();
                    if (jobs.Count() == 1)
                    {
                        var jobId = jobs.First().Id;

                        var resultJob = EncodeJobManager.Instance.FindEncodeJob(jobId);

                        Assert.IsNotNull(resultJob);
                        Assert.AreEqual(job, resultJob);
                    }
                    else
                    {
                        Assert.Fail("Too many or too few jobs found in GetIncompleteEncodeJobs");
                    }
                }
                else
                {
                    Assert.Fail("Unsuccessful in adding encode job.");
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateEncodeJobSuccess()
        {
            var firstJob = TestHelper.MakeJob(valid:true);

            if (firstJob.IsValid)
            {
                if (EncodeJobManager.Instance.AddEncodeJobToQueue(firstJob))
                {
                    var jobs = EncodeJobManager.Instance.GetIncompleteEncodeJobs();
                    if (jobs.Count() == 1)
                    {
                        var jobAfterAdd = jobs.First();
                        var jobAfterChange = jobAfterAdd.Clone() as EncodeJob;
                        jobAfterChange!.VideoDirectoryPath = "/some/other/place/";

                        var finalResult = EncodeJobManager.Instance.UpdateJob(jobAfterAdd, jobAfterChange);
                        var finalJobs = EncodeJobManager.Instance.GetIncompleteEncodeJobs();
                        var finalJob = finalJobs.First();

                        Assert.IsTrue(finalResult);
                        Assert.AreEqual(jobAfterChange, finalJob);
                        Assert.AreEqual(1, finalJobs.Count());
                    }
                    else
                    {
                        Assert.Fail($"Found {jobs.Count()} in GetIncompleteEncodeJobs, expected 1");
                    }
                }
                else
                {
                    Assert.Fail("Unsuccessful in adding encode job.");
                }
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}