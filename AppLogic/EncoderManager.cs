#nullable enable
using AppLogic.Encoders;
using System;
using DataObjects;
using System.IO;
using DataAccess.Vmaf;

namespace AppLogic
{
    /// <summary>
    /// Encoding related logic operations
    /// </summary>
    public static class EncoderManager
    {
        /// <summary>
        /// Opens an encoder and starts encoding a specified job
        /// </summary>
        public static void StartJob(EncodeJob job, string encoderType)
        {
            //make the encoder
            IEncoder encoder = encoderType.ToLower() switch
            {
                "vp9test" => new EncoderFfmpegVp9Tester(),
                "libaomffmpeg" => new EncoderLibaomFfmpeg(),
                _ => throw new InvalidOperationException()
            };

			EncodeAttempt? attempt;
			do
            {
                if (0 != job.Attempts.Count)
                { EncodeJobManager.ImproveQuality(job); }
                try
                {
                    var outputPath = EncodeJobManager.GenerateJobOutputFilename(job);

					//Start the encode
					DateTime startTime = DateTime.Now;
					encoder.Encode(job, outputPath);

					//Save the attempt
					attempt = new EncodeAttempt(outputPath)
					{
						CommandLineArgs = (string)job.AdditionalCommandArguments.Clone(),
						StartTime = startTime,
						EndTime = DateTime.Now,
						VmafResult = VmafAccessor.GetVmaf(Path.Combine(job.VideoDirectoryPath,
										job.VideoFileName), outputPath),
						FileSize = (ulong) new FileInfo(outputPath).Length
					};
                    job.Attempts.Add(attempt);
                }
                catch { attempt = null; }
            } while (RunAgain(job, attempt?.OriginalOutputPath));
        }

        private static bool RunAgain(EncodeJob job, string? result)
        {
            if (result == null)
            { return true; }
            if (job.Attempts.Count >= job.MaxAttempts)
            { return false; }
            if (EncodeJobManager.AttemptMeetsRequirements(job, result))
            { return false; }

            return true;
        }
    }
}