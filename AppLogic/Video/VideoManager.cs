using System;
using DataObjects;
using DataAccess;
using System.Collections.Generic;
using AppConfig;
using AppConfig.Models;

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
            get => _instance ?? throw new NotImplementedException();
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

            for (int i = 1; i < sceneData.Length; i++)
            {
                var thisScene = sceneData[i - 1];

                if (!MeetsCriteria(thisScene))
                { sceneData[i] = Scene.Combine(thisScene, sceneData[i]); }
                else
                { chunks.Add(thisScene); }
            }

            return chunks.ToArray();
        }
        #endregion
    }
}