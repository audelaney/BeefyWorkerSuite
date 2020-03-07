using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using AppConfig.Models;

namespace AppConfig.Accessors
{
    internal class XMLAppConfigReader : IAppConfigAccessor
    {
        private readonly string _path;
        public XMLAppConfigReader(string filepath) =>
                _path = filepath;

        public ConfigModel GetConfig()
        {
            var config = new ConfigModel();
            config.PollingInterval = IntervalTime;
            config.DefaultEncoder = DefaultEncoder;
            config.ActiveBucketPath = ActiveBucketPath;
            config.InputBucketPath = InputBucketPath;
            config.CompletedBucketPath = CompletedBucketPath;
            config.DBTypeAndString = DBTypeAndString;
            config.FailedBucketPath = FailedBucketPath;
            config.InputBucketPath = InputBucketPath;
            config.LogFilePath = LogFilePath;
            config.MaxRunningLocalJobs = MaxRunningLocalJobs;
            config.ProcessedBucketPath = ProcessedBucketPath;
            config.FileAccessor = FileAccessorType.real;
            config.VideoAccessor = VideoAccessorType.real;
            config.FfprobePath = FfprobePath;
            config.FfmpegPath = FfmpegPath;
            config.PtsScriptPath = PtsScriptPath;
            return config;
        }

        public string InputBucketPath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("InputBucket");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string FailedBucketPath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("FailedBucket");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string ProcessedBucketPath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("ProcessedBucket");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string LogFilePath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("LogFile");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string ActiveBucketPath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("ActiveBucket");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string CompletedBucketPath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("CompletedBucket");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string DefaultEncoder
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("DefaultEncoder", "type");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public KeyValuePair<DbType, string> DBTypeAndString
        {
            get
            {
                try
                {
                    //Check if the startup node has a useful entry for the DbConnection
                    var startupNode = XDocument.Load(XmlReader.Create(_path)).Root.Element("Local");
                    bool startupNodeHasDbConnectionData = ((startupNode.HasElements &&
                                                            startupNode.Element("DbConnection").Attribute("type") != null &&
                                                            startupNode.Element("DbConnection").Attribute("string") != null));
                    //If it doesn't we just keep using 'mock' queue db
                    var dbStr = startupNodeHasDbConnectionData
                        ? startupNode.Element("DbConnection").Attribute("type").Value
                        : "mock";

                    DbType dbType = dbStr switch
                    {
                        "mongo" => DbType.mongo,
                        "mock-baddb" => DbType.mockBadDb,
                        "mock" => DbType.mock,
                        _ => DbType.mock
                    };

                    var connString = (dbType == DbType.mock || dbType == DbType.mockBadDb) ? ""
                                    : startupNode.Element("DbConnection").Attribute("string").Value;

                    return new KeyValuePair<DbType, string>(dbType, connString);
                }
                catch (Exception ex)
                { throw ex; }
            }
        }
        public int MaxRunningLocalJobs
        {
            get
            {
                string result;

                var rootNode = XDocument.Load(XmlReader.Create(_path)).Root;

                //Try to find the value from the running node first
                if (!string.IsNullOrWhiteSpace(rootNode.Element("Local").Element("MaxJobs").Attribute("num").Value))
                { result = rootNode.Element("Local").Element("MaxJobs").Attribute("num").Value; }
                //Then the default node
                else
                { return 0; }
                if (int.TryParse(result, out int jobMax))
                { return jobMax; }
                else
                { return 0; }
            }
        }
        public int IntervalTime
        {
            get
            {
                string result;

                var rootNode = XDocument.Load(XmlReader.Create(_path)).Root;

                //Try to find the value from the running node first
                if (!string.IsNullOrWhiteSpace(rootNode.Element("Local").Element("IntervalTime").Attribute("seconds").Value))
                { result = rootNode.Element("Local").Element("IntervalTime").Attribute("seconds").Value; }
                //Then the default node
                else
                { return 61; }
                if (int.TryParse(result, out int interval))
                { return interval; }
                else
                { return 61; }
            }
        }

        public string FfprobePath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("Ffprobe", "path");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string FfmpegPath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("Ffprobe", "path");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string PtsScriptPath
        {
            get
            {
                try
                {
                    return GetAttributePathValueFromRunningOrDefaultNode("PtsScript", "path");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        private string GetAttributePathValueFromRunningOrDefaultNode(string node, string attribute = "path")
        {
            string result;

            var rootNode = XDocument.Load(XmlReader.Create(_path)).Root;

            //Try to find the value from the running node first
            if (!string.IsNullOrWhiteSpace(rootNode.Element("Local").Element(node).Attribute(attribute).Value))
            { result = rootNode.Element("Local").Element(node).Attribute(attribute).Value; }
            else
            { throw new ApplicationException("Failed to read XML for " + node); }

            return result;
        }
    }
}