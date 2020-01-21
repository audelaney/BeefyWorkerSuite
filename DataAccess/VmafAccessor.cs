using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DataAccess.Vmaf
{
    /// <summary>
    /// Class for pulling VMAF from videos
    /// </summary>
    public static class VmafAccessor
    {
        /// <summary>
        /// Gets vmaf using ffmpeg from a source and path
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Throws if:
        ///     - Either path does not exist
        /// </exception>
        public static double GetVmaf(string sourcePath, string encodedPath)
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
                FileName = "ffmpeg"
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
                catch (Exception ex)
                { }
            }

            return output;
        }

        /// <summary>
        /// Gets vmaf using ffmpeg piping to ffmpeg from a source and path
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Throws if:
        ///     - Either path does not exist
        /// </exception>
        public static double GetVmafScene(string sourcePath, string scenePath, double sceneStartTime, double sceneEndTime)
        {
            if (!File.Exists(sourcePath) || !File.Exists(scenePath))
            { throw new ArgumentException("Either path is invalid"); }

            double output = 0;

            string processArgs = "-c \"ffmpeg -hide_banner";
            processArgs += " -ss " + sceneStartTime;
            processArgs += " -to " + sceneEndTime;
            processArgs += " -i " + sourcePath;
            processArgs += " -an -sn -f yuv4mpegpipe";
            processArgs += " -pix_fmt yuv420p - | ";
            processArgs += "ffmpeg -r 23.976 -i " + scenePath;
            processArgs += " -r 23.976 -i pipe:";
            processArgs += " -lavfi libvmaf -f null -\"";

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

        private static double? ParseVmafFromLine(string? line)
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