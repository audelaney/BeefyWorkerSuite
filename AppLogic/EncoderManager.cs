#nullable enable
using DataAccess;
using System.Linq;
using AppLogic.Encoders;
using System.Collections.Generic;
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
        /// Combines the resulting output from 
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Thrown if:
        ///     - Any of the jobs are not marked as completed
        ///     - Jobs don't have an InputInterval
        ///     - Jobs that have a video source that doesn't match the first video
        /// </exception>
        public static void Combine(EncodeJob[] jobs, string outputFileName)
        {
            if (jobs.Where(j => !j.Completed).Count() != 0)
            { throw new ArgumentException("Some jobs not completed."); }
            if (jobs.Where(j => null == GetJobSceneStartTime(j)).Count() != 0)
            { throw new ArgumentException("Some jobs have invalid or no InputInterval."); }
            var unmantchedJobs = jobs.Where(j => j.VideoFileName != jobs.First().VideoFileName).Count();
            if (unmantchedJobs != 0)
            { throw new ArgumentException(unmantchedJobs + " jobs don't match videos"); }

            //Sort the jobs and verify their expected output video actually exist
            var sortedJobs = jobs.OrderBy(j => GetJobSceneStartTime(j));
            var sortedJobOutputFiles = sortedJobs.Select(j =>
                {
                    // The output file should be the only file in the job directory in the completed bucket
                    var outDir = Path.Combine(AppConfigManager.Instance.CompletedBucketPath, j.Id.ToString());
                    if (!Directory.Exists(outDir) && Directory.GetFiles(outDir).Count() == 1)
                    { return Directory.GetFiles(outDir).First(); }
                    else
                    {
                        throw new InvalidOperationException(
                            "Couldn't find job directory in completed bucket for job. CompletedBucket: "
                            + AppConfigManager.Instance.CompletedBucketPath
                            + " job: " + jobs.ToString());
                    }
                });

            //Concat the video files
            VideoAccessor.ConcatVideosIntoOneOutput(sortedJobOutputFiles.ToList()
                                                    , Path.Combine(AppConfigManager.Instance.CompletedBucketPath));
        }

        private static double? GetJobSceneStartTime(EncodeJob job)
        {
            try
            {
                if (double.TryParse(job.ChunkInterval?.Split('-').First() ?? "", out double result))
                { return result; }
            }
            catch
            { }

            return null;
        }

        /// <summary>
        /// Opens an encoder and starts encoding a specified job
        /// </summary>
        public static void StartJob(EncodeJob job, string encoderType)
        {
            //make the encoder
            IEncoder encoder = encoderType.ToLower() switch
            {
                "libaomffmpeg" => new EncoderLibaomFfmpeg(),
                "ffmpeghevc" => new EncoderFfmpegHevc(),
                _ => throw new InvalidOperationException()
            };

            EncodeAttempt? attempt;
            do
            {
                if (0 != job.Attempts.Count)
                { EncodeJobManager.ImproveQuality(job); }
                try
                {
                    var outputPath = Path.Combine(AppConfigManager.Instance.ActiveBucketPath,
                                                    job.Id.ToString(),
                                                    EncodeJobManager.GenerateJobOutputFilename(job));

                    //Start the encode
                    DateTime startTime = DateTime.Now;
                    encoder.Encode(job, outputPath);

                    //Save the attempt
                    attempt = new EncodeAttempt(outputPath)
                    {
                        CommandLineArgs = (string)job.AdditionalCommandArguments.Clone(),
                        StartTime = startTime,
                        EndTime = DateTime.Now,
                        VmafResult = (job.IsChunk) ?
                            VmafAccessor.GetVmafScene(Path.Combine(job.VideoDirectoryPath, job.VideoFileName),
                                                        outputPath,
                                                        double.Parse(job.ChunkInterval!.Split('-').First()),
                                                        double.Parse(job.ChunkInterval!.Split('-').Last())) :
                            VmafAccessor.GetVmaf(Path.Combine(job.VideoDirectoryPath, job.VideoFileName), outputPath),
                        FileSize = (ulong)new FileInfo(outputPath).Length
                    };
                    job.Attempts.Add(attempt);
                }
                catch
                {
                    attempt = null;
                }
            } while (RunAgain(job, attempt?.OriginalOutputPath));
        }

        private static bool RunAgain(EncodeJob job, string? result)
        {
            if (result == null)
            { return true; }
            if (job.Attempts.Count >= job.MaxAttempts)
            { return false; }
            if (job.DoesMostRecentAttemptMeetRequirements())
            { return false; }

            return true;
        }
    }
}