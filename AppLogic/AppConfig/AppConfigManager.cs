using System;
using System.Collections.Generic;
using System.IO;

namespace AppLogic
{
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

        abstract public string InputBucketPath { get; }
        abstract public string ProcessedBucketPath { get; }
        abstract public string ActiveBucketPath { get; }
        abstract public string CompletedBucketPath { get; }
        abstract public string LogFilePath { get; }
        abstract public KeyValuePair<string, string> DBTypeAndString { get; }
        abstract public int MaxRunningLocalJobs { get; }
		abstract public string FailedBucketPath { get; }
	}
}