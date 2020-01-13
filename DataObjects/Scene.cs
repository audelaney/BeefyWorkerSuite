using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataObjects
{
    public class Scene
    {
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public int VidSizeKB { get; set; }
        public TimeSpan Duration
        {
            get
            {
                TimeSpan start = TimeSpan.FromSeconds(StartTime);
                TimeSpan end = TimeSpan.FromSeconds(EndTime);
                return end.Subtract(start);
            }
        }

        public override string ToString()
        {
            string output = "";

            output += "Start time: " + StartTime;
            output += ",End time: " + EndTime;
            output += ",Duration: " + Duration.ToString();

            return output;
        }

        public static Scene Combine(Scene firstScene, Scene secondScene)
        {
            var output = new Scene
            {
                StartTime = firstScene.StartTime,
                EndTime = secondScene.EndTime,
                VidSizeKB = firstScene.VidSizeKB + secondScene.VidSizeKB
            };

            return output;
        }

        public static Scene Combine(IEnumerable<Scene> scenes)
        {
            if (0 == scenes.Count())
            {
                throw new InvalidOperationException("Cannot combine empty collection.");
            }

            var output = scenes.First();

            for (int i = 1; i < scenes.Count(); i++)
            {
                output = Combine(output, scenes.ElementAt(i));
            }

            return output;
        }
    }

}