using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppLogic;
using System;
using DataObjects;
using System.Linq;
using AppConfig;
using AppConfig.Models;
using System.Collections.Generic;

namespace Tests.AppLogic
{
    [TestClass]
    public class EncodeJobManagerImproveQualityTests
    {
        // Refresh the Manager and the DAO before every run.
        [TestInitialize]
        public void TestSetup()
        {
            var mockConfigModel = TestHelper.MakeConfig();
            AppConfigManager.SetConfig(mockConfigModel);
        }

        [TestMethod]
        public void BasicCrfImproveSuccess()
        {
            var oldAddlCmd = "-crf 34";
            var expectedAddlCmd = "-crf 32";
            
            var job = new EncodeJob
            {
                VideoFileName = "somefile.mkv",
                AdditionalCommandArguments = oldAddlCmd
            };

            EncodeJobManager.ImproveQuality(job);

            Assert.AreEqual(expectedAddlCmd, job.AdditionalCommandArguments);
        }

        [TestMethod]
        public void ComplexCrfImproveSuccess()
        {
            var oldAddlCmd = "-b:v 2500k -crf 34 -cpu-used 3";
            var expectedAddlCmd = "-b:v 2500k -crf 32 -cpu-used 3";
            
            var job = new EncodeJob
            {
                VideoFileName = "somefile.mkv",
                AdditionalCommandArguments = oldAddlCmd
            };

            EncodeJobManager.ImproveQuality(job);

            Assert.AreEqual(expectedAddlCmd, job.AdditionalCommandArguments);
        }
    }
}