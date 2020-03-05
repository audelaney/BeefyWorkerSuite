using Newtonsoft.Json;
using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataObjects
{
    public class Scene : ICloneable
    {
        public Scene(double startTime, double endTime) =>
        (this.StartTime, this.EndTime) = (startTime, endTime);

        public Scene()
        {
        }

        public double StartTime { get; set; }
        public double EndTime { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public TimeSpan Duration
        {
            get
            {
                TimeSpan start = TimeSpan.FromSeconds(StartTime);
                TimeSpan end = TimeSpan.FromSeconds(EndTime);
                return end.Subtract(start);
            }
        }
        [BsonIgnore]
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                if (StartTime < 0 || EndTime < StartTime)
                { return false; }
                else
                { return true; }
            }
        }

        public override string ToString()
        {
            return $"Start time: {StartTime}, End time: {EndTime}, Duration: {Duration.ToString()}";
        }

        public object Clone()
        {
            return new Scene(this.StartTime, this.EndTime);
        }

        #region Static
        public static Scene Combine(Scene firstScene, Scene secondScene)
        {
            var output = new Scene(firstScene.StartTime, secondScene.EndTime);

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

        public static IEnumerable<Scene> ScenesFromTimeStamps(IEnumerable<double> pts)
        {
            var scenes = new List<Scene>(pts.Count());
            for (int i = 1; i < pts.Count(); i++)
            { scenes.Add(new Scene { StartTime = pts.ElementAt(i - 1), EndTime = pts.ElementAt(i) }); }
            return scenes;
        }
        #endregion
    }
}