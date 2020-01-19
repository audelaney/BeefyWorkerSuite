using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataAccess
{
    /// <summary>
    /// Static accessor for generic video functions
    /// </summary>
    public class VideoAccessor
    {
        /// <summary>
        /// Combines multiple videos into a single video in order
        /// </summary>
        public static void ConcatVideosIntoOneOutput(List<string> videoPaths, string outputVideoFile)
        {
            if (File.Exists(outputVideoFile))
            { File.Delete(outputVideoFile); }
            videoPaths.ForEach(p => { if (!File.Exists(p)) { throw new ArgumentException(p + " doesn't exist"); } });

            // Write the list of video files out to a text file to use for the ffmpeg command
            string textFileName = outputVideoFile + ".txt";
            using (StreamWriter writer = new StreamWriter(textFileName))
            {
                videoPaths.ForEach(p => writer.WriteLine(p));
                writer.Close();
            }

            var ffmpegStartInfo = new ProcessStartInfo
            {
                RedirectStandardError = true,
                FileName = "ffmpeg",
                Arguments = "-f concat -safe 0 -i " + textFileName + " -c copy " + outputVideoFile
            };

            using (Process ffmpegProcess = new Process { StartInfo = ffmpegStartInfo })
            {
                ffmpegProcess.Start();
                ffmpegProcess.Refresh();
                ffmpegProcess.WaitForExit();
                ffmpegProcess.Close();
            }

            File.Delete(textFileName);
        }
    }
}