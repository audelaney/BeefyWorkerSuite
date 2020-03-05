using System;
using System.Collections.Generic;

namespace AppConfig.Models
{
    public class ConfigModel
    {
        /// <summary>
        /// The path/location of the directory where files will be placed 
        /// for the app to ingest and then process.
        /// </summary>
        public string InputBucketPath { get; set; } = "";
        /// <summary>
        /// The path/location of the directory where files will be placed
        /// after their jobs have been ingested by the app and they are 
        /// awaiting their turn to be encoded in the queue.
        /// </summary>
        public string ProcessedBucketPath { get; set; } = "";
        /// <summary>
        /// The path/location of the directory where directories will be placed
        /// jobs are being performed and they need a directory to be perforemed
        /// in.
        /// </summary>
        public string ActiveBucketPath { get; set; } = "";
        /// <summary>
        /// The path/location of the directory where files will be placed
        /// after their job is completed and the encode is ready to be exported
        /// </summary>
        public string CompletedBucketPath { get; set; } = "";
        /// <summary>
        /// The path and filename of the file app logs will be placed
        /// </summary>
        public string LogFilePath { get; set; } = "";
        /// <summary>
        /// The type of datastore to use and the connection string or file
        /// name of that data store.
        /// </summary>
        public KeyValuePair<DbType, string> DBTypeAndString { get; set; }
        /// <summary>
        /// The maximum number of running jobs the system can withstand.
        /// </summary>
        public int MaxRunningLocalJobs { get; set; }
        /// <summary>
        /// The path/location of the directory where directories will be placed
        /// when their job has reached its max number of attempts but the job
        /// did not meet the requirements.
        /// </summary>
		public string FailedBucketPath { get; set; } = "";
        /// <summary>
        /// The type of file accessor to use
        /// </summary>
        public FileAccessorType FileAccessor { get; set; }
        /// <summary>
        /// The type of video accessor to use
        /// </summary>
        public VideoAccessorType VideoAccessor { get; set; }
        /// <summary>
        /// Default encoder for the application to use when not specified by job
        /// </summary>
        public string DefaultEncoder { get; set; } = "";
        /// <summary>
        /// Seconds between application runs
        /// </summary>
        public int PollingInterval { get; set; }
    }
}