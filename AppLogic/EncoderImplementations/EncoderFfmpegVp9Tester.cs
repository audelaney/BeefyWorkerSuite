#nullable enable
using System.IO;
using System.Diagnostics;
using System;
using DataObjects;
using System.Threading;

namespace AppLogic.Encoders
{
    public class EncoderFfmpegVp9Tester : IEncoder
    {
        /// <summary>
        /// Job info is mostly irrelevant, just use the input file and
        /// toss everything else. Otherwise a good demonstration of how
        /// to make your own implementation.
        /// </summary>
        public string? Encode(EncodeJob encodeJob, string? outputDirectoryPath = null)
        {
            if (!Directory.Exists(encodeJob.VideoDirectoryPath))
            { throw new ArgumentException("Video directory does not exist"); }

            _encodesRun++;

            try
            {
                string input = Path.Combine(encodeJob.VideoDirectoryPath, encodeJob.VideoFileName);
                string output = Path.Combine(encodeJob.VideoDirectoryPath,
                                            Path.GetFileNameWithoutExtension(input) + ".attempt" + _encodesRun + ".mkv");

                string processArguments = "-i " + input;
                processArguments += " -c:v libvpx-vp9 -c copy";
                processArguments += " -crf 15 -cpu-used 5 -y";
                processArguments += " " + output;

                ProcessStartInfo testProcessInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = processArguments,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process testProcess = new Process { StartInfo = testProcessInfo })
                {
                    testProcess.Start();

                    while (!testProcess.HasExited)
                    {
                        Thread.Sleep(2000);
                        testProcess.Refresh();
                    }

                    using (StreamWriter writer = new StreamWriter(output + ".log"))
                    {
                        writer.Write(testProcess.StandardError.ReadToEnd());
                        writer.Close();
                    }

                    testProcess.Close();

                    return output;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int _encodesRun = 0;
        public int EncodesRun
        {
            get
            {
                return _encodesRun;
            }
        }
    }
}