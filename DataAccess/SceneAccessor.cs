using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataObjects;

namespace DataAccess
{
    public class SceneAccessor
    {
        /// <summary>
        /// Verifies input exists, runs input through ffmpeg scene change
        /// detection, uses resulting timestamp set to analyze scenes and returns the 
        /// collection of those scenes.
        /// </summary>
        public static Scene[] AnalyzeVideoInput(string videoInputPath)
        {
            if (!File.Exists(videoInputPath))
            { throw new ArgumentException("Video does not exist"); }
            
            //ffmpeg -i input.flv  -filter:v "select='gt(scene,0.4)',showinfo" -f null - 2> ffout
            //grep showinfo ffout | grep pts_time:[0-9.]* -o | grep [0-9.]* -o > timestamps

            //Ffmpeg the video for scene change time stamps
            var processInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = string.Format(@"-i {0} -filter:v ""select='gt(scene,0.4)',showinfo"" -f null -", videoInputPath),
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            var unparsedTimestamps = new List<string>();

            using (Process ffmpegProcess = new Process { StartInfo = processInfo })
            {
                ffmpegProcess.Start();
                while (!ffmpegProcess.StandardError.EndOfStream)
                {
                    string buffer = ffmpegProcess.StandardError.ReadLine() ?? "";
                    if (buffer.Contains("pts_time:"))
                    { unparsedTimestamps.Add(buffer); }
                }
                ffmpegProcess.Close();
                ffmpegProcess.Dispose();
            }

            if (unparsedTimestamps.Count == 0)
            {
                throw new ApplicationException("No time stamp markers found in ffmpeg output.");
            }

            //basically fuck readability
            var timeStamps = unparsedTimestamps.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                                .Select(
                                                    l => l.First(i => i.Contains("pts_time:"))
                                                          .Replace("pts_time:", "")
                                                ).Prepend("0");

            List<Scene> scenes = new List<Scene>();
            for (int i = 0; i < timeStamps.Count() - 1; i++)
            {
                try
                {
                    scenes.Add(
                        new Scene
                        {
                            StartTime = double.Parse(timeStamps.ElementAt(i)),
                            EndTime = double.Parse(timeStamps.ElementAt(i + 1)),
                        }
                    );
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return scenes.ToArray();
        }
    }
}