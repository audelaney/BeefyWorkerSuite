using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppLogic;
using System;
using DataObjects;
using System.Linq;
using DataAccess;
using AppConfig.Models;
using System.Collections.Generic;
using AppConfig;

namespace Tests.AppLogic
{
    [TestClass]
    public class EncoderManagerBadArgTests
    {
        [TestInitialize]
        public void TestSetup()
        {
            var mockConfigModel = TestHelper.MakeConfig();
            AppConfigManager.SetConfig(mockConfigModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CombineDoesntAcceptIncompleteJobs()
        {
            var jobs = TestHelper.MakeJobs(valid:true);

            EncoderManager.Instance
                    .CombineSuccessfulEncodes(jobs.ToArray(), "output.mkv");

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CombineDoesntAcceptEmptyList()
        {
            var jobs = new EncodeJob[0];

            EncoderManager.Instance
                    .CombineSuccessfulEncodes(jobs, "output.mkv");

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void CombineDoesntAcceptInvalidJobs()
        {
            var jobs = TestHelper.MakeJobs(valid:false);

            EncoderManager.Instance
                    .CombineSuccessfulEncodes(jobs.ToArray(), "output.mkv");

            Assert.Fail();
        }
    }
}