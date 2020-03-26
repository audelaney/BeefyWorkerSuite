#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using AppConfig.Models;
using AppConfig.Accessors;
using System.Configuration;

namespace AppConfig
{
    /// <summary>
    /// Holds/gets globally used strings for the application
    /// </summary>
    public static class AppConfigManager
    {
        private static ConfigModel? _model;
        public static ConfigModel Model
        {
            get
            { return _model ?? throw new Exception("Configuration not set"); }
            private set 
            {
                _model = value;
                NotifyWatchersOfChange();
            }
        }

        /// <summary>
        /// Sets the config instance to whatever is found at the path, or throws
        /// an exception if there is nothing at the end of the path or the file
        /// is not compatible. Also resets the EncodeJobManager instance.
        /// </summary>
        public static void SetConfig(string configFilePath)
        {
            if (File.Exists(configFilePath))
            {
                var dao = (Path.GetExtension(configFilePath)) switch
                {
                    ".config" => new XMLAppConfigReader(configFilePath),
                    ".xml" => new XMLAppConfigReader(configFilePath),
                    ".json" => throw new InvalidOperationException("Json config not supported"),
                    ".txt" => throw new InvalidOperationException("Plain text config not supported"),
                    _ => throw new InvalidOperationException("Unknown config type."),
                };

                var newConfig = dao.GetConfig();
                Model = newConfig;
            }
        }

        public static void SetConfig(ConfigModel model) =>
            Model = model;
        
        private static List<IConfigWatcher> _watchers = new List<IConfigWatcher>();

        public static void WatchForChanges(IConfigWatcher watcher) => 
            _watchers.Add(watcher);

        private static void NotifyWatchersOfChange()
        {
            _watchers.RemoveAll(w => w == null);
            _watchers.ForEach(w => w.Notify());
        }
    }
}