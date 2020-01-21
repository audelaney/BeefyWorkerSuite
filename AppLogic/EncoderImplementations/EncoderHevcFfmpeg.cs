#nullable enable
using System.IO;
using System;
using DataObjects;
using System.Threading;
using System.Diagnostics;

namespace AppLogic.Encoders
{
    /// <summary>
    /// Single pass ffmpeg based libaom encoder implementation
    /// </summary>
    public class EncoderHevcFfmpeg : IEncoder
    {
        /// <summary></summary>
        public string? Encode(EncodeJob encodeJob, string outputPath)
        {
            if (!Directory.Exists(encodeJob.VideoDirectoryPath))
            { throw new ArgumentException("Video directory does not exist"); }
            if (string.IsNullOrWhiteSpace(outputPath))
            { throw new ArgumentException("File is empty"); }
            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            { throw new DirectoryNotFoundException(); }

            try
            {
                string input = Path.Combine(encodeJob.VideoDirectoryPath, encodeJob.VideoFileName);

                string processArguments = "";
                if (encodeJob.IsChunk)
                {
                    string[] times = encodeJob.ChunkInterval!.Split('-');
                    if (double.TryParse(times[0], out double start) &&
                        double.TryParse(times[1], out double end))
                    {
                        processArguments += "-ss " + times[0];
                        processArguments += " -to " + end;
                        processArguments += " -i " + input;
                    }
                    else
                    { throw new ApplicationException(); }
                }
                else
                { processArguments += "-i " + input; }

                processArguments += " -c:v libx265 -threads 2";
                processArguments += " " + encodeJob.AdditionalCommandArguments;
                processArguments += " " + outputPath;

                ProcessStartInfo libaomProcessInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = processArguments,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process hevcProcess = new Process { StartInfo = libaomProcessInfo })
                {
                    hevcProcess.Start();
                    hevcProcess.Refresh();
                    hevcProcess.WaitForExit();
                    hevcProcess.Close();
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