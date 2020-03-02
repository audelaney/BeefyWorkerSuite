using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppLogic;
using System;
using DataObjects;
using System.Linq;
using DataAccess;
using System.Collections.Generic;
using AppConfig.Models;
using AppConfig;

namespace Tests.AppLogic
{
    [TestClass]
    public class EncoderManagerTests
    {
        [TestInitialize]
        public void TestSetup()
        {
            var mockConfigModel = TestHelper.MakeConfig();
            mockConfigModel.FileAccessor = FileAccessorType.combineSuccessMock;
            AppConfigManager.SetConfig(mockConfigModel);
        }

        [TestMethod]
        public void CombineJustWorksMan()
        {
            var jobs = TestHelper.MakeJobs(valid:true).ToList();
            jobs.ForEach(j => j.Completed = true);

            EncoderManager.Instance.CombineSuccessfulEncodes(jobs.ToArray(), "somefile.mkv");
        }
    }
}