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
        public string? Encode(EncodeJob encodeJob, string outputPath)
        {
            if (!Directory.Exists(encodeJob.VideoDirectoryPath))
            { throw new ArgumentException("Video directory does not exist"); }
            if (!string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("File is empty");
            }

            try
            {
                string input = Path.Combine(encodeJob.VideoDirectoryPath, encodeJob.VideoFileName);

                string processArguments = "";
                if (!string.IsNullOrWhiteSpace(encodeJob.ChunkInterval))
                {
                    string[] times = encodeJob.ChunkInterval.Split('-');
                    if (double.TryParse(times[0], out double start) &&
                        double.TryParse(times[1], out double end))
                    {
                        processArguments += "-ss " + times[0];
                        processArguments += " -i" + input;
                        processArguments += " -t " + (end - start);
                    }
                    else
                    { }
                }
                else
                { processArguments += "-i " + input; }
                
                processArguments += " -strict experimental -c:v libaom-av1";
                processArguments += " " + encodeJob.AdditionalCommandArguments;
                processArguments += " " + outputPath;

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

                return outputPath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}