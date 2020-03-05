using System;
using DataObjects;
using AppConfig;
using AppConfig.Models;
using DataAccess;

namespace AppLogic
{
    /// <summary>
    /// Video manager that has the real logic
    /// </summary>
    public class RealVideoManager : VideoManager, IConfigWatcher
    {
        private IVideoAccessor _videoAccessor;

        internal RealVideoManager()
        {
            AppConfigManager.WatchForChanges(this);
            SetupFromConfig();
        }

        /// <summary>
        /// Converts a video into timestamped scenes that match criteria
        /// </summary>
        public override Scene[] GetScenesFromVideo(string videoPath)
        {
            return AnalyzeScenes(_videoAccessor.AnalyzeVideoInput(videoPath));
        }

        /// <summary>
        /// Notify the manager of a configuration change
        /// </summary>
        public void Notify() =>
            SetupFromConfig();

        private void SetupFromConfig()
        {
            _videoAccessor = AppConfigManager.Model.VideoAccessor switch
            {
                VideoAccessorType.real => new RealVideoAccessor(),
                VideoAccessorType.fake => new MockVideoAccessor(),
                _ => new MockVideoAccessor()
            };
        }
    }
}