using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppLogic;
using System;
using DataObjects;
using System.Linq;
using DataAccess;
using System.Collections.Generic;

namespace Tests.AppLogic
{
    [TestClass]
    public class EncoderManagerTests
    {
        [TestInitialize]
        public void TestSetup()
        {
            AppConfigManager.SetConfig("mock");
        }

        [TestMethod]
        public void CombineJustWorksMan()
        {
            var jobs = TestHelper.MakeJobs(valid:true);            
        }
    }
}