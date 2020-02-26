using System;
using System.IO;
using System.Linq;
using AppLogic.Encoders;
using DataAccess;
using DataObjects;

namespace AppLogic
{
    internal class RealEncoderManager : EncoderManager
    {
        public RealEncoderManager()
        {
            if (AppConfigManager.Instance.DBTypeAndString
                .Key.ToLower().Contains("mock"))
            { _fileAccessor = new GoodFakeFileAccessor(); }
            else
            { _fileAccessor = new RealFileAccessor(); }
        }
        
        private readonly IFileAccessor _fileAccessor;

        public override void Combine(EncodeJob[] jobs, string outputFileName)
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
                    if (!_fileAccessor.DoesFolderExist(outDir)
                        && _fileAccessor.GetFilesInFolder(outDir).Count() == 1)
                    { return _fileAccessor.GetFilesInFolder(outDir).First(); }
                    else
                    {
                        throw new DirectoryNotFoundException(
                            "Couldn't find job directory in completed bucket for job. CompletedBucket: "
                            + AppConfigManager.Instance.CompletedBucketPath
                            + " job: " + jobs.ToString());
                    }
                });

            //Concat the video files
            RealVideoAccessor.ConcatVideosIntoOneOutput(sortedJobOutputFiles.ToList()
                                    , Path.Combine(AppConfigManager.Instance.CompletedBucketPath));
        }

        public override void StartJob(EncodeJob job, string encoderType)
        {
            //make the encoder
            IEncoder encoder = encoderType.ToLower() switch
            {
                "libaomffmpeg" => new EncoderLibaomFfmpeg(),
                "hevcffmpeg" => new EncoderHevcFfmpeg(),
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
                                                    EncodeJob.GenerateJobOutputFilename(job));

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
                            RealVideoAccessor.GetVmafScene(Path.Combine(job.VideoDirectoryPath, job.VideoFileName),
                                                        outputPath,
                                                        double.Parse(job.ChunkInterval!.Split('-').First()),
                                                        double.Parse(job.ChunkInterval!.Split('-').Last())) :
                            RealVideoAccessor.GetVmaf(Path.Combine(job.VideoDirectoryPath, job.VideoFileName), outputPath),
                        FileSize = _fileAccessor.GetFileSize(outputPath)
                    };
                    job.Attempts.Add(attempt);
                    // Update job in db?
                }
                catch
                { attempt = null; }
            } while (RunAgain(job, attempt?.OriginalOutputPath));
        }

        /// <todo>Move this</todo>
        private bool RunAgain(EncodeJob job, string? result)
        {
            if (result == null)
            { return true; }
            if (job.Attempts.Count >= job.MaxAttempts)
            { return false; }
            if (job.DoesMostRecentAttemptMeetRequirements())
            { return false; }

            return true;
        }
        
        /// <todo>This should be in the EncodeJob class</todo>
        private double? GetJobSceneStartTime(EncodeJob job)
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
    }
}