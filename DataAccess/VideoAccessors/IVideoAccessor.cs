using System;
using DataObjects;

namespace DataAccess
{
    /// <summary>
    /// Generic video related data operations interface
    /// </summary>
    public interface IVideoAccessor
    {
        /// <summary>
        /// Verifies input exists, runs input through ffmpeg scene change
        /// detection, uses resulting timestamp set to analyze scenes and returns the 
        /// collection of all scenes.
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if:
        ///     - The video input does not exist
        /// </exception>
        Scene[] AnalyzeVideoInput(string videoFilePath);
    }
}