#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppLogic;
using DataObjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EncodeJobOverseer
{
    public class Worker : BackgroundService
    {
        #region HelperProperties
        public bool CanStartNewJob
        {
            get
            { return runningLocalJobs.Count < AppConfigManager.Instance.MaxRunningLocalJobs; }
        }
        #endregion

        #region Fields
        private readonly int jobCheckIntervalSeconds = 60;

        private readonly ILogger<Worker> _logger;
        private static ILogger? staticLogger;

        private List<Thread> runningLocalJobs = new List<Thread>();
        #endregion

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            staticLogger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Videos for jobs being retrieved from " + AppConfigManager.Instance.ProcessedBucketPath);
            _logger.LogInformation("Local jobs setup in directory " + AppConfigManager.Instance.ActiveBucketPath);
            _logger.LogInformation("Encoded videos output to " + AppConfigManager.Instance.CompletedBucketPath);
            _logger.LogInformation("Failed jobs moved to " + AppConfigManager.Instance.FailedBucketPath);
            _logger.LogInformation("Completed jobs moved to " + AppConfigManager.Instance.CompletedBucketPath);
            _logger.LogInformation("Database being used: " + AppConfigManager.Instance.DBTypeAndString.Key + " @@ "
                                                            + AppConfigManager.Instance.DBTypeAndString.Value);
            if (!EncodeJobManager.SetLogger(_logger))
            { _logger.LogError("Unable to pass logger into logic layer."); }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running...");

                CleanRunningLocalJobs();

                if (CanStartNewJob)
                {
                    EncodeJob? encodeJob = ChooseNextJob();

                    if (null != encodeJob)
                    {
                        PrepLocalJobDirectory(encodeJob);

                        StartLocalJob(encodeJob);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(jobCheckIntervalSeconds), stoppingToken);
            }
        }

        private EncodeJob? ChooseNextJob()
        {
            EncodeJob? output = null;

            try
            {
                int[] priorityRange = { 1, 2, 3 };
                for (int i = 0; i < priorityRange.Length; i++)
                {
                    output ??= EncodeJobManager.Instance.GetIncompleteEncodeJobs(priorityRange[i])?.FirstOrDefault(j => j.CheckedOutTime == null);
                }
            }
            catch (System.Exception ex)
            {
                if (null == output)
                {
                    _logger.LogCritical(ex, "Fatal exception encountered while choosing next job.");
                }
                else
                {
                    _logger.LogError(ex, "Exception encountered while choosing next job. Job found: "
                                    + output.ToString());
                }
            }

            return output;
        }

        private bool PrepLocalJobDirectory(EncodeJob job)
        {
            _logger.LogInformation("Creating local directory for job id: " + job.Id.ToString());
            bool result = false;
            try
            {
                //build output directory
                string outputDirectory = Path.Combine(AppConfigManager.Instance.ActiveBucketPath, job.Id.ToString());
                if (!Directory.Exists(outputDirectory))
                { Directory.CreateDirectory(outputDirectory); }
                else
                {
                    if (Directory.GetFiles(outputDirectory).Length != 0)
                    { throw new ApplicationException("Directory already exists and is not empty, something is busted somewhere."); }
                }

                string videoDir = AppConfigManager.Instance.ProcessedBucketPath;

                // If the job is a chunk, it won't be in the working dir, it will be one above
                string targetVideoDirectory = (job.IsChunk) ?
                                                AppConfigManager.Instance.ActiveBucketPath :
                                                outputDirectory;

                if (!File.Exists(Path.Combine(targetVideoDirectory, job.VideoFileName)))
                {
                    File.Move(Path.Combine(videoDir, job.VideoFileName),
                              Path.Combine(targetVideoDirectory, job.VideoFileName));
                }

                videoDir = targetVideoDirectory;

                //update the video location in database
                try
                {
                    var oldJob = job.Clone() as EncodeJob ?? throw new ApplicationException("?????");
                    job.VideoDirectoryPath = videoDir;
                    oldJob.Id = job.Id;
                    result = EncodeJobManager.Instance.UpdateJob(oldJob, job);
                }
                catch (Exception ex)
                { throw ex; }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception found while creating local job directory for job: \n"
                                + job.ToString());
                throw ex;
            }

            return result;
        }

        private bool StartLocalJob(EncodeJob job)
        {
            _logger.LogInformation("Starting local job with id: " + job.Id.ToString());
            try
            {
                EncodeJobManager.Instance.MarkJobCheckedOut(job.Id, true);
                Thread t = new Thread(RunEncode);
                t.IsBackground = false;
                t.Name = job.Id.ToString();
                t.Start(job);
                runningLocalJobs.Add(t);
                _logger.LogInformation(string.Format("Encode job {0} started for video file {1}"
                                        , job.Id.ToString(), job.VideoFileName));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured while starting an encode job" +
                                job.ToString());
                return false;
            }
        }

        public static void RunEncode(object? job)
        {
            if (!(job is EncodeJob activeJob))
            { return; }

            EncodeJobManager.Instance.MarkJobCheckedOut(activeJob, true);
            EncodeJob oldJob = (EncodeJob)activeJob.Clone();
            oldJob.Id = activeJob.Id;

            //Do the encode
            string encoderConfig = "hevcffmpeg";
            EncoderManager.StartJob(activeJob, encoderConfig);

            // Update job doesn't update the completed status or checked out time.
            EncodeJobManager.Instance.UpdateJob(oldJob, activeJob);
            var jobComplete = activeJob.DoesMostRecentAttemptMeetRequirements();
            EncodeJobManager.Instance.MarkJobComplete(activeJob, jobComplete);
            string oldWorkingDir = Path.Combine(AppConfigManager.Instance.ActiveBucketPath,
                                        activeJob.Id.ToString());

            if (Directory.Exists(oldWorkingDir))
            {
                string newDir = (jobComplete) ?
                    Path.Combine(AppConfigManager.Instance.CompletedBucketPath, activeJob.Id.ToString()) :
                    Path.Combine(AppConfigManager.Instance.FailedBucketPath, activeJob.Id.ToString());
                try
                { Directory.Move(oldWorkingDir, newDir); }
                catch
                { }
            }
            else
            { }
        }

        private bool StartRemoteJob(EncodeJob job, object location)
        {
            throw new NotImplementedException();
        }

        private void CleanRunningLocalJobs()
        {
            var doneJobs = runningLocalJobs.Where(t => !t.IsAlive).ToList();
            if (doneJobs.Count != 0)
            {
                _logger.LogInformation(string.Format("Found {0} finished jobs", doneJobs.Count));
                doneJobs.ForEach(j => runningLocalJobs.Remove(j));
            }
        }
    }
}