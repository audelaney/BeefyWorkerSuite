using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Combines multiple videos into a single video in order
        /// </summary>
        void ConcatVideosIntoOneOutput(List<string> videoPaths, string outputVideoFile);
        /// <summary>
        /// Uses ffprobe to get the duration for a specified video
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if:
        ///     - videoPath doesn't exist.
        /// </exception>
        double GetVideoDuration(string videoPath);
        /// <summary>
        /// Gets vmaf using ffmpeg from a source and path
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Throws if:
        ///     - Either path does not exist
        /// </exception>
        double GetVmaf(string sourcePath, string encodedPath);
        /// <summary>
        /// Gets vmaf using ffmpeg piping to ffmpeg from a source and path
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Throws if:
        ///     - Either path does not exist
        /// </exception>
        double GetVmafScene(string sourcePath, string scenePath, double sceneStartTime, double sceneEndTime);
    }
}