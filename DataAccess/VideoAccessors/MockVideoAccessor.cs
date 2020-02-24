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
    }
}