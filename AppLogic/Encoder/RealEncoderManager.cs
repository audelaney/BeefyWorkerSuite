using System;
using System.IO;
using System.Linq;
using AppLogic.Encoders;
using DataAccess;
using DataObjects;
using AppConfig;
using AppConfig.Models;

namespace AppLogic
{
    internal class RealEncoderManager : EncoderManager, IConfigWatcher
    {
        public RealEncoderManager()
        {
            AppConfigManager.WatchForChanges(this);
            SetupFromConfig();
        }

        private IFileAccessor _fileAccessor;
        private IVideoAccessor _videoAccessor;

        public override void CombineSuccessfulEncodes(EncodeJob[] jobs, string outputFileName)
        {
            if (jobs.Length == 0)
            { throw new ArgumentException("0 successful encodes to combine."); }
            if (jobs.Where(j => !j.Completed).Count() != 0)
            { throw new ArgumentException("Some jobs not completed."); }
            if (jobs.Where(j => !j.IsChunk).Count() != 0)
            { throw new ArgumentException("Some jobs have invalid or no chunk/scene."); }
            var unmantchedJobs = jobs.Where(j => j.VideoFileName != jobs.First().VideoFileName).Count();
            if (unmantchedJobs != 0)
            { throw new ArgumentException(unmantchedJobs + " jobs don't match videos"); }

            //Sort the jobs and verify their expected output video actually exist
            var sortedJobs = jobs.OrderBy(j => j.Chunk?.StartTime);
            var sortedJobOutputFiles = sortedJobs.Select(j =>
                {
                    // The output file should be the only file in the job directory in the completed bucket
                    var outDir = Path.Combine(AppConfigManager.Model.CompletedBucketPath, j.Id.ToString());
                    if (_fileAccessor.DoesFolderExist(outDir)
                        && _fileAccessor.GetFilesInFolder(outDir).Count() == 1)
                    { return _fileAccessor.GetFilesInFolder(outDir).First(); }
                    else
                    {
                        throw new DirectoryNotFoundException(
                            "Couldn't find job directory in completed bucket for job. CompletedBucket: "
                            + AppConfigManager.Model.CompletedBucketPath
                            + " job: " + jobs.ToString());
                    }
                });

            //Concat the video files
            _videoAccessor.ConcatVideosIntoOneOutput(sortedJobOutputFiles.ToList()
                                    , Path.Combine(AppConfigManager.Model.CompletedBucketPath, outputFileName));
        }

        /// <summary>
        /// Opens an encoder and starts encoding a specified job
        /// </summary>
        public override void BeginEncodeJobAttempts(EncodeJob job, string encoderType)
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
                    var outputPath = Path.Combine(AppConfigManager.Model.ActiveBucketPath,
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
                            _videoAccessor.GetVmafScene(Path.Combine(job.VideoDirectoryPath, job.VideoFileName),
                                                        outputPath,
                                                        job.Chunk!.StartTime,
                                                        job.Chunk!.EndTime) :
                            _videoAccessor.GetVmaf(Path.Combine(job.VideoDirectoryPath, job.VideoFileName), outputPath),
                        FileSize = _fileAccessor.GetFileSize(outputPath)
                    };
                    var oldJob = (job.Clone() as EncodeJob)!;
                    oldJob.Id = job.Id;
                    oldJob.Attempts = job.Attempts;
                    job.Attempts.Add(attempt);
                    try
                    {
                        EncodeJobManager.Instance.UpdateJob(oldJob, job);
                        job = EncodeJobManager.Instance.FindEncodeJob(job.Id) ?? job;
                    }
                    catch
                    { }
                }
                catch
                { attempt = null; }
            } while (RunAgain(job, attempt?.OriginalOutputPath));
        }

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

        public void Notify() =>
            SetupFromConfig();

        private void SetupFromConfig()
        {
            _fileAccessor = AppConfigManager.Model.FileAccessor switch
            {
                FileAccessorType.real => new RealFileAccessor(),
                FileAccessorType.workingFake => new GoodFakeFileAccessor(),
                FileAccessorType.combineSuccessMock
                        => new TestingCombineSuccessFakeFileAccessor(),
                _ => new GoodFakeFileAccessor()
            };
            _videoAccessor = AppConfigManager.Model.VideoAccessor switch
            {
                VideoAccessorType.real => new RealVideoAccessor(AppConfigManager.Model.FfmpegPath,
                                                                AppConfigManager.Model.FfprobePath,
                                                                AppConfigManager.Model.PtsScriptPath),
                VideoAccessorType.fake => new MockVideoAccessor(),
                _ => new MockVideoAccessor()
            };
        }
    }
}