#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataObjects;

namespace DataAccess
{
    /// <summary></summary>
    public class MockVideoAccessor : IVideoAccessor
    {
        /// <summary></summary>
        public IEnumerable<Scene>? TestScenes { get; set; }

        /// <summary></summary>
        public Scene[] AnalyzeVideoInput(string videoFilePath)
        {
            if (videoFilePath == "success" && TestScenes != null)
            { return TestScenes.ToArray(); }
            throw new FileNotFoundException();
        }

        /// <summary></summary>
        public void ConcatVideosIntoOneOutput(List<string> videoPaths, string outputVideoFile)
        { }

        /// <summary></summary>
        public double GetVideoDuration(string videoPath) => 20;

        /// <summary></summary>
        public double GetVmaf(string sourcePath, string encodedPath) => 97;

        /// <summary></summary>
        public double GetVmafScene(string sourcePath, string scenePath, double sceneStartTime, double sceneEndTime)
                => 97;
    }
}