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
    public class VideoManagerTests
    {
        [TestInitialize]
        public void TestSetup()
        {
            var mockConfigModel = TestHelper.MakeConfig();
            AppConfigManager.SetConfig(mockConfigModel);
        }

        public void GetScenesFromVideoValid()
        {
            var initialScenePts = new[]
            {
                0,0.626,5.005,5.297,6.59,6.757,9.843,10.26,10.636,15.015000
            };
            var initialScenes = Scene.ScenesFromTimeStamps(initialScenePts);

            var result = VideoManager.Instance.GetScenesFromVideo("success");

            Assert.AreEqual(1, result.Count());
        }
    }
}