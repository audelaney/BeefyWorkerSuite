#nullable enable
using System.IO;
using System;
using DataObjects;
using System.Threading;
using System.Diagnostics;

namespace AppLogic.Encoders
{
    public class EncoderLibaomFfmpeg : IEncoder
    {
        public int _encodesRun = 0;
        public int EncodesRun
        {
            get
            {
                return _encodesRun;
            }
        }

        public string? Encode(EncodeJob encodeJob, string? outputDirectoryPath = null)
        {
            if (!Directory.Exists(encodeJob.VideoDirectoryPath))
            { throw new ArgumentException("Video directory does not exist"); }
            if (!string.IsNullOrWhiteSpace(outputDirectoryPath))
            {
                if (!Directory.Exists(outputDirectoryPath))
                { throw new ArgumentException("Video directory does not exist"); }
            }
            _encodesRun++;

            try
            {
                string input = Path.Combine(encodeJob.VideoDirectoryPath, encodeJob.VideoFileName);
                string output = Path.Combine(outputDirectoryPath ?? encodeJob.VideoDirectoryPath,
                                            Path.GetFileNameWithoutExtension(input));
                if (encodeJob.ChunkNumber != 0) { output += ".chunk" + encodeJob.ChunkNumber; }
                output += ".attempt" + _encodesRun + ".mkv";

                string processArguments = "";
                if (!string.IsNullOrWhiteSpace(encodeJob.InputInterval))
                {
                    string[] times = encodeJob.InputInterval.Split('-');
                    if (double.TryParse(times[0], out double start) &&
                        double.TryParse(times[1], out double end))
                    {
                        processArguments += "-ss " + times[0];
                        processArguments += " -i" + input;
                        processArguments += " -t " + (end - start);
                    }
                    else
                    {

                    }
                }
                else
                {
                    processArguments += "-i " + input;
                }
                processArguments += " -strict experimental -c:v libaom-av1";
                processArguments += " " + encodeJob.AdditionalCommandArguments;
                processArguments += " " + output;

                ProcessStartInfo libaomProcessInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = processArguments,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process libaomProcess = new Process { StartInfo = libaomProcessInfo })
                {
                    libaomProcess.Start();

                    while (!libaomProcess.HasExited)
                    {
                        Thread.Sleep(2000);
                        libaomProcess.Refresh();
                    }

                    libaomProcess.Close();
                }

                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}