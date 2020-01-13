using System;
using DataObjects;
using DataAccess;
using System.Collections.Generic;

namespace AppLogic
{
    public class SceneManager
    {
        /// <summary>
        /// Mushes the scenes together based on desired speed/efficiency/accuracy/
        /// precision.
        /// </summary>
        private static Scene[] AnalyzeScenes(Scene[] sceneData)
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

        public static bool MeetsCriteria(Scene scene)
        {
            var minTime = new TimeSpan(0, 0, 20);

            if (scene.Duration.TotalSeconds < minTime.TotalSeconds)
            { return false; }

            else { return true; }
        }

        public static Scene[] AnalyzeVideoInput(string videoPath)
        {
            return AnalyzeScenes(SceneAccessor.AnalyzeVideoInput(videoPath));
        }
    }
}