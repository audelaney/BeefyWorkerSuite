#nullable enable
using AppLogic.Encoders;
using System;
using DataObjects;
using System.IO;

namespace AppLogic
{
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

            string? outputDirectoryPath = null;
            if (null != job.InputInterval)
            {
                outputDirectoryPath = Path.Combine(AppConfigManager.Instance.ActiveBucketPath,
                                                    job.Id.ToString());
            }
            string? output = null;
            do
            {
                if (0 != encoder.EncodesRun)
                {
                    EncodeJobManager.ImproveQuality(job);
                }
                output = encoder.Encode(job, outputDirectoryPath);
            } while (RunAgain(job, output, encoder.EncodesRun));

            //TODO requirement check and logic
            if (!EncodeJobManager.Instance.MarkJobComplete(job, true))
            {
                var tryAgainJob = job.Clone() as EncodeJob;
                tryAgainJob!.Completed = true;
                tryAgainJob!.Id = job.Id;
                EncodeJobManager.Instance.UpdateJob(job, tryAgainJob);
            }
        }

        private static bool RunAgain(EncodeJob job, string? result, int runs)
        {
            if (result == null)
            { return true; }
            if (runs >= job.MaxAttempts)
            {
                return false;
            }
            if (EncodeJobManager.EncodeMeetsRequirements(job, result))
            {
                return false;
            }

            return true;
        }
    }
}