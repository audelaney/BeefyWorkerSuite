using System;
using System.Collections.Generic;
using System.IO;

namespace AppLogic
{
    /// <summary>
    /// Holds/gets globally used strings for the application
    /// </summary>
    public abstract class AppConfigManager
    {
        static internal string DEFAULT_CONFIG_PATH = "/var/local/EncodeWorker/overseer-xml.config";
        static internal string? _configPath;
        static internal string ConfigPath
        {
            get
            {
                return _configPath ?? DEFAULT_CONFIG_PATH;
            }
        }
        static private AppConfigManager? _instance;
        /// <summary>
        /// Singleton instance
        /// </summary>
        static public AppConfigManager Instance
        {
            get
            {
                if (null == _instance)
                { _instance = new XMLAppConfigReader(); }
                return _instance;
            }
        }

        /// <summary>
        /// Sets the config instance to whatever is found at the path, or throws
        /// an exception if there is nothing at the end of the path or the file
        /// is not compatible. Also resets the EncodeJobManager instance.
        /// </summary>
        public static void SetConfig(string configFilePath)
        {
            EncodeJobManager.ConfigReset();

            if (File.Exists(configFilePath))
            {
                _instance = (Path.GetExtension(configFilePath)) switch
                {
                    ".config" => new XMLAppConfigReader(),
                    ".json" => throw new InvalidOperationException("Json config not supported"),
                    ".txt" => throw new InvalidOperationException("Plain text config not supported"),
                    _ => throw new InvalidOperationException("Unknown config type."),
                };
                _configPath = configFilePath;
            }
            else if (configFilePath == "mock" ||
                     configFilePath == "mock-baddb")
            {
                _instance = new MockAppConfigReader(configFilePath);
            }
            else
            {
                var apex = new ApplicationException("Config path does not exist and mock string was not found.");
                apex.Data["path"] = configFilePath;
                throw apex;
            }
        }

        /// <summary>
        /// The path/location of the directory where files will be placed 
        /// for the app to ingest and then process.
        /// </summary>
        abstract public string InputBucketPath { get; }
        /// <summary>
        /// The path/location of the directory where files will be placed
        /// after their jobs have been ingested by the app and they are 
        /// awaiting their turn to be encoded in the queue.
        /// </summary>
        abstract public string ProcessedBucketPath { get; }
        /// <summary>
        /// The path/location of the directory where directories will be placed
        /// jobs are being performed and they need a directory to be perforemed
        /// in.
        /// </summary>
        abstract public string ActiveBucketPath { get; }
        /// <summary>
        /// The path/location of the directory where files will be placed
        /// after their job is completed and the encode is ready to be exported
        /// </summary>
        abstract public string CompletedBucketPath { get; }
        /// <summary>
        /// The path and filename of the file app logs will be placed
        /// </summary>
        abstract public string LogFilePath { get; }
        /// <summary>
        /// The type of datastore to use and the connection string or file
        /// name of that data store.
        /// </summary>
        abstract public KeyValuePair<string, string> DBTypeAndString { get; }
        /// <summary>
        /// The maximum number of running jobs the system can withstand.
        /// </summary>
        abstract public int MaxRunningLocalJobs { get; }
        /// <summary>
        /// The path/location of the directory where directories will be placed
        /// when their job has reached its max number of attempts but the job
        /// did not meet the requirements.
        /// </summary>
		abstract public string FailedBucketPath { get; }
    }
}