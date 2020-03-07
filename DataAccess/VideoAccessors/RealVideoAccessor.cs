using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataObjects;

namespace DataAccess
{
    /// <summary>
    /// Accessor for generic video functions. Currently linux dependent
    /// </summary>
    public class RealVideoAccessor : IVideoAccessor
    {
        /// <summary>
        /// Initializes a video accessor with the supplied paths
        /// </summary>
        public RealVideoAccessor(string ffmpegPath, string ffprobePath, string ptsScriptPath)
        {
            _ffmpegExecutablePath = ffmpegPath;
            _ffprobeExecutablePath = ffprobePath;
            _ptsScriptPath = ptsScriptPath;
        }
        
        private readonly string _ffmpegExecutablePath;
        private readonly string _ffprobeExecutablePath;
        private readonly string _ptsScriptPath;

        /// <summary></summary>
        public Scene[] AnalyzeVideoInput(string videoInputPath)
        {
            if (!File.Exists(videoInputPath))
            { throw new FileNotFoundException("Video does not exist"); }

            //ffmpeg -i input.flv  -filter:v "select='gt(scene,0.4)',showinfo" -f null - 2> ffout
            //grep showinfo ffout | grep pts_time:[0-9.]* -o | grep [0-9.]* -o > timestamps

            //Ffmpeg the video for scene change time stamps
            var processInfo = new ProcessStartInfo
            {
                FileName = "sh",
                Arguments =  $"{_ptsScriptPath} {videoInputPath}",
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            IEnumerable<string> unparsedFfmpegLines = new List<string>();

            using (Process ffmpegProcess = new Process { StartInfo = processInfo })
            {
                ffmpegProcess.Start();
                ffmpegProcess.Refresh();
                var lineSep = new[] {'\r','\n'};
                var buffer = ffmpegProcess.StandardError.ReadToEnd().Split(lineSep, StringSplitOptions.RemoveEmptyEntries);
                unparsedFfmpegLines = buffer.Where(l => l.Contains("pts_time:"));
                ffmpegProcess.Close();
            }

            if (unparsedFfmpegLines.Count() == 0)
            { throw new ApplicationException("No time stamp markers found in ffmpeg output."); }

            //basically fuck readability
            var timeStamps = unparsedFfmpegLines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                                .Select(
                                                    l => l.First(i => i.Contains("pts_time:"))
                                                          .Replace("pts_time:", ""))
                                                .Prepend("0")
                                                .Select(ts => (double.TryParse(ts, out double res))
                                                        ? res : throw new InvalidCastException($"Bad ts: {ts}"))
                                                .ToList();

            // Add the final duration as the end point of the video, otherwise the last scene ends on the last scene
            // change and not at the end of the video
            timeStamps.Add(GetVideoDuration(videoInputPath));

            List<Scene> scenes = new List<Scene>();
            for (int i = 0; i < timeStamps.Count() - 1; i++)
            {
                try
                {
                    scenes.Add(
                        new Scene
                        {
                            StartTime = timeStamps.ElementAt(i),
                            EndTime = timeStamps.ElementAt(i + 1)
                        }
                    );
                }
                catch (Exception ex)
                { throw ex; }
            }

            return scenes.ToArray();
        }
        /// <summary></summary>
        public void ConcatVideosIntoOneOutput(List<string> videoPaths, string outputVideoPath)
        {
            if (File.Exists(outputVideoPath))
            { File.Delete(outputVideoPath); }
            videoPaths.ForEach(p => { if (!File.Exists(p)) { throw new ArgumentException(p + " doesn't exist"); } });

            // Write the list of video files out to a text file to use for the ffmpeg command
            string textFileName = outputVideoPath + ".txt";
            using (StreamWriter writer = new StreamWriter(textFileName))
            {
                videoPaths.ForEach(p => writer.WriteLine($"file '{p}'"));
                writer.Close();
            }

            var ffmpegStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                FileName = _ffmpegExecutablePath,
                Arguments = "-f concat -safe 0 -i " + textFileName + " -c copy " + outputVideoPath
            };

            using (Process ffmpegProcess = new Process { StartInfo = ffmpegStartInfo })
            {
                ffmpegProcess.Start();
                ffmpegProcess.Refresh();
                ffmpegProcess.WaitForExit();
                var output = ffmpegProcess.StandardError.ReadToEnd();
                ffmpegProcess.Close();
            }

            File.Delete(textFileName);
        }
        /// <summary></summary>
        public double GetVideoDuration(string videoPath)
        {
            if (!File.Exists(videoPath))
            { throw new FileNotFoundException($"{videoPath} does not exist."); }

            var ffprobeStartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                FileName = _ffprobeExecutablePath,
                Arguments = "-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 " + videoPath
            };

            string processOutput = "";

            using (Process ffprobeProcess = new Process { StartInfo = ffprobeStartInfo })
            {
                ffprobeProcess.Start();
                ffprobeProcess.Refresh();
                ffprobeProcess.WaitForExit();
                processOutput = ffprobeProcess.StandardOutput.ReadToEnd().Trim();
                ffprobeProcess.Close();
            }

            try
            { return double.Parse(processOutput); }
            catch
            { return 0; }
        }
        /// <summary></summary>
        public double GetVmaf(string sourcePath, string encodedPath)
        {
            if (!File.Exists(sourcePath) || !File.Exists(encodedPath))
            { throw new ArgumentException("Either path is invalid"); }

            double output = 0;

            ProcessStartInfo vmafFfmpegStartInfo = new ProcessStartInfo
            {
                Arguments = string.Format("-r 23.976 -i {0} -r 23.976 -i {1} -lavfi libvmaf -f null -", encodedPath, sourcePath),
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = _ffmpegExecutablePath
            };

            using (Process vmafFfmpegProcess = new Process { StartInfo = vmafFfmpegStartInfo })
            {
                try
                {
                    vmafFfmpegProcess.Start();
                    vmafFfmpegProcess.Refresh();
                    vmafFfmpegProcess.WaitForExit();
                    while (!vmafFfmpegProcess.StandardError.EndOfStream)
                    {
                        string? line = vmafFfmpegProcess.StandardError.ReadLine();
                        var maybeVmaf = ParseVmafFromLine(line);

                        if (null != maybeVmaf)
                        {
                            output = maybeVmaf.Value;
                            break;
                        }
                    }
                }
                catch
                { }
            }

            return output;
        }
        /// <summary></summary>
        public double GetVmafScene(string sourcePath, string scenePath, double sceneStartTime, double sceneEndTime)
        {
            if (!File.Exists(sourcePath) || !File.Exists(scenePath))
            { throw new ArgumentException("Either path is invalid"); }

            double output = 0;

            string processArgs = $"-c \"{_ffmpegExecutablePath} -hide_banner";
            processArgs += $" -ss  {sceneStartTime} -to  {sceneEndTime}";
            processArgs += $" -i {sourcePath}";
            processArgs += " -an -sn -f yuv4mpegpipe -pix_fmt yuv420p - | ";
            processArgs += $"{_ffmpegExecutablePath} -r 23.976 -i {scenePath}";
            processArgs += " -r 23.976 -i pipe: -lavfi libvmaf -f null -\"";

            ProcessStartInfo vmafFfmpegStartInfo = new ProcessStartInfo
            {
                Arguments = processArgs,
                RedirectStandardError = true,
                FileName = "sh"
            };

            using (Process vmafBashProcess = new Process { StartInfo = vmafFfmpegStartInfo })
            {
                vmafBashProcess.Start();
                vmafBashProcess.Refresh();
                vmafBashProcess.WaitForExit();
                while (!vmafBashProcess.StandardError.EndOfStream)
                {
                    string? line = vmafBashProcess.StandardError.ReadLine();
                    var maybeVmaf = ParseVmafFromLine(line);

                    if (null != maybeVmaf)
                    {
                        output = maybeVmaf.Value;
                        break;
                    }
                }
            }

            return output;
        }

        private double? ParseVmafFromLine(string? line)
        {
            double? output = null;

            if (!string.IsNullOrWhiteSpace(line) && line.Contains("VMAF score:"))
            {
                string vmafNum = line.Split(' ').Last();
                if (double.TryParse(vmafNum, out double vmafParsed))
                {
                    output = vmafParsed;
                }
            }

            return output;
        }
    }
}