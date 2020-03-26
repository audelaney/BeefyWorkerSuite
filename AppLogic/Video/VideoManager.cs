using System;
using DataObjects;
using DataAccess;
using System.Collections.Generic;
using AppConfig;
using AppConfig.Models;
using System.Linq;

namespace AppLogic
{
    /// <summary>
    /// Generic video related logic operations
    /// </summary>
    public abstract class VideoManager
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static VideoManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AppConfigManager.Model.Logic switch
                    {
                        LogicType.mock => new MockVideoManager(),
                        LogicType.real => new RealVideoManager()
                    };
                }
                return _instance;
            }
        }
        private static VideoManager? _instance;

        /// <summary>
        /// Converts a video into timestamped scenes that match criteria
        /// </summary>
        public abstract Scene[] GetScenesFromVideo(string videoPath);

        #region Static
        /// <summary>
        /// If the scene 
        /// </summary>
        /// <todo>Should be adding a 'criteria' object of some kind as
        /// a param.</todo>
        protected static bool MeetsCriteria(Scene scene)
        {
            var minTime = new TimeSpan(0, 0, 20);

            if (scene.Duration.TotalSeconds < minTime.TotalSeconds)
            { return false; }
            else { return true; }
        }

        /// <summary>
        /// Mushes scenes together based on desired speed/efficiency/accuracy/
        /// precision.
        /// </summary>
        /// <param name="sceneData">The scenes to mush together.</param>
        protected static Scene[] AnalyzeScenes(Scene[] sceneData)
        {
            var chunks = new List<Scene>();

            chunks.Add(sceneData.First());

            var scenes = sceneData.Length;
            for (int i = 1; i < scenes; i++)
            {
                if (MeetsCriteria(chunks.Last()))
                {
                    var remaining = sceneData.ToList().GetRange(i,scenes-i);
                    if (MeetsCriteria(Scene.Combine(remaining)))
                    {
                        chunks.Add(sceneData[i]);
                    }
                    else
                    {
                        var lastScene = chunks.Last();
                        chunks[chunks.Count - 1] = Scene.Combine(lastScene, sceneData[i]);
                    }
                }
                else
                {
                    var lastScene = chunks.Last();
                    chunks[chunks.Count - 1] = Scene.Combine(lastScene, sceneData[i]);
                }
            }

            return chunks.ToArray();
        }
        #endregion
    }
}