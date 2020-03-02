using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppLogic;
using System;
using DataObjects;
using System.Linq;
using DataAccess;
using AppConfig;

namespace Tests.AppLogic
{
    [TestClass]
    public class EncoderManagerNoFilesFoundTests
    {
        [TestInitialize]
        public void TestSetup()
        {
            var mockConfigModel = TestHelper.MakeConfig();
            AppConfigManager.SetConfig(mockConfigModel);
        }
    }
}