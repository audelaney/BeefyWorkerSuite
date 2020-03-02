#nullable enable
using AppLogic;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using DataObjects;
using Newtonsoft.Json;
using AppConfig;
using AppConfig.Models;

namespace EncodeJobIngester
{
    public class Worker : BackgroundService
    {
        private readonly int searchIntervalMS = 60 * 1000;
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// Starts the worker
        /// </summary>
        /// <param name="logger">The logging object to use for the worker.</param>
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Video input being retrieved from " + AppConfigManager.Model.InputBucketPath);
            _logger.LogInformation("Video output set for " + AppConfigManager.Model.ProcessedBucketPath);
            _logger.LogInformation("Database being used: " + AppConfigManager.Model.DBTypeAndString.Key + " @@ "
                                    + AppConfigManager.Model.DBTypeAndString.Value);
            if (!EncodeJobManager.SetLogger(_logger))
            { _logger.LogError("Unable to pass logger into logic layer."); }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string activeBucket = AppConfigManager.Model.InputBucketPath;
                _logger.LogInformation($"Searching {activeBucket} for video files with matching job files..");

                Dictionary<string, EncodeJob> jobFilesWithJobs = new Dictionary<string, EncodeJob>();
                try { jobFilesWithJobs = SearchFilesForValidEncodeJobs(_logger, Directory.GetFiles(activeBucket)); }
                catch (Exception ex)
                {
                    _logger.LogError(ex
                        , $"Uncaught exception while searching {activeBucket} for jobs");
                }

                if (jobFilesWithJobs.Count > 0)
                {
                    _logger.LogInformation($"Found {jobFilesWithJobs.Count} jobs. Iterating now");
                }

                //First operate on jobs that don't involve split input and master videos
                foreach (KeyValuePair<string, EncodeJob> fileNameAndJob in jobFilesWithJobs.Where(kp => !kp.Value.IsChunk))
                {
                    try
                    {
                        //Attempt to add the job to the queue, and if successful, delete the now redundant job file
                        //and move the video to the output (processed) queue
                        if (EncodeJobManager.Instance.AddEncodeJobToQueue(fileNameAndJob.Value))
                        {
                            _logger.LogInformation("Encode job added to DB from file " + fileNameAndJob.Key);
                            File.Move(Path.Combine(activeBucket, fileNameAndJob.Value.VideoFileName),
                                      Path.Combine(AppConfigManager.Model.InputBucketPath, fileNameAndJob.Value.VideoFileName));
                            File.Delete(Path.Combine(activeBucket, fileNameAndJob.Key));
                        }
                        else
                        { throw new ApplicationException($"Unable to add {fileNameAndJob.Value}"
                                    + $" to queue in database {AppConfigManager.Model.DBTypeAndString.Key}"); }
                    }
                    catch (UnauthorizedAccessException unAuthEx)
                    {
                        _logger.LogCritical(unAuthEx,
                            "Access denied while moving video and job data files.");
                    }
                    catch (ApplicationException apEx)
                    {
                        _logger.LogError(apEx
                            , "Handled exception while processing a valid encode job");
                    }
                    catch (ArgumentException argex)
                    {
                        if (argex.Data.Contains("job"))
                        {
                            _logger.LogError("Argument exception for job: \n" + argex.Data["job"]?.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex
                            , "Uncaught exception while processing a valid encode job");
                    }
                }

                //Then operate on the jobs that have the same master video file
                foreach (string masterVideo in jobFilesWithJobs.Select(kp => kp.Value.VideoFileName).Distinct())
                {
                    //get the relevant jobfile/job pairs
                    var relevantJobs = jobFilesWithJobs.Where(kp => kp.Value.VideoFileName == masterVideo);

                    try
                    {
                        foreach (var fileNameAndJob in relevantJobs)
                        {
                            //Attempt to add the job to the queue, and if successful, delete the now redundant job file
                            //and move the video to the output (processed) queue
                            if (EncodeJobManager.Instance.AddEncodeJobToQueue(fileNameAndJob.Value))
                            {
                                _logger.LogInformation("Encode job added to DB from file " + fileNameAndJob.Key);
                                File.Delete(Path.Combine(activeBucket, fileNameAndJob.Key));
                            }
                            else
                            { throw new ApplicationException($"Unable to add {fileNameAndJob.Value.ToString()}"
                                    + " to queue in database {AppConfigManager.Model.DBTypeAndString.Key}"); }
                        }

                        File.Move(Path.Combine(activeBucket, masterVideo),
                                  Path.Combine(AppConfigManager.Model.ProcessedBucketPath, masterVideo));
                    }
                    catch (UnauthorizedAccessException unAuthEx)
                    {
                        _logger.LogCritical(unAuthEx,
                            "Access denied while moving video and job data files.");
                    }
                    catch (ApplicationException apEx)
                    {
                        _logger.LogError(apEx
                            , "Handled exception while processing a valid encode job");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex
                            , "Uncaught exception while processing a valid encode job");
                    }
                }

                await Task.Delay(searchIntervalMS, stoppingToken);
            }
        }

        /// <summary>
        /// Accepts an array of full file paths all from the same directory and searches through
        /// them to find video files and matching json files containing encode job stats/data.
        /// </summary>
        /// <returns> A dictionary where each file that was successfully turned into a job is 
        /// used as a key to the job it contained. </returns>
        public static Dictionary<string, EncodeJob> SearchFilesForValidEncodeJobs(ILogger logger, string[] files)
        {
            if (0 == files.Length) { return new Dictionary<string, EncodeJob>(); }

            logger.LogInformation($"Found {files.Length} files in input bucket.");
            Dictionary<string, EncodeJob> output = new Dictionary<string, EncodeJob>(files.Length / 2);

            try
            {
                string parentDirectory = Path.GetDirectoryName(files.First()) ?? "";

                //Sort the files and return early if they don't meet criteria
                var jsonFiles = files.Where(f => f.EndsWith(".json"));
                if (0 == jsonFiles.Count()) { return output; }
                logger.LogInformation("Found " + jsonFiles.Count() + " json files.");
                var nonJsonFiles = files.Where(f => !f.EndsWith(".json"));
                if (0 == nonJsonFiles.Count()) { return output; }
                logger.LogInformation("Found " + nonJsonFiles.Count() + " non json files.");

                IEnumerable<VideoWithJobFiles> videoWithJobFiles = nonJsonFiles.Select(f => new VideoWithJobFiles
                {
                    VideoFileName = Path.GetFileName(f),
                    VideoFileNameExtensionless = Path.GetFileNameWithoutExtension(f)
                });

                //Turn each match into an encodejob DTO and throw it in the list chief
                foreach (var videoJob in videoWithJobFiles)
                {
                    videoJob.JobFiles = jsonFiles.Where(jf => jf.Contains(videoJob.VideoFileNameExtensionless)).ToArray();

                    foreach (string jobFile in videoJob.JobFiles)
                    {
                        using StreamReader reader = new StreamReader(Path.GetFullPath(Path.Combine(parentDirectory, jobFile)));
                        string json = reader.ReadToEnd();

                        var encodeJob = EncodeJob.FromJson(json);
                        if (null == encodeJob)
                        {
                            logger.LogError("Error deserializing json:" + json);
                        }
                        else if (!encodeJob.IsValid)
                        {
                            logger.LogError("Error building valid job from file: " + jobFile + "\n Job: " + encodeJob.ToString());
                        }
                        else
                        {
                            encodeJob.VideoFileName = videoJob.VideoFileName;
                            output[jobFile] = encodeJob;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return output;
        }
#nullable disable
        public class VideoWithJobFiles
        {
            /// <summary>
            /// With extension
            /// </summary>
            public string VideoFileName { get; set; }
            public string VideoFileNameExtensionless { get; set; }
            /// <summary>
            /// Without extension
            /// </summary>
            public string[] JobFiles { get; set; }
            public bool IsValid
            {
                get
                {
                    if (VideoFileName.Length > 4 &&
                        JobFiles.Length > 1)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

    }
}