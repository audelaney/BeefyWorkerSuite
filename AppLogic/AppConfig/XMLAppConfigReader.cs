using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace AppLogic
{
    internal class XMLAppConfigReader : AppConfigManager
    {
        //Private variable to enable caching so we don't boot up an XML reader every time we need a path.
        private DateTime LastFileChange;

        /// <summary>
        /// Check if the config file was changed to enable the use of cached values.
        /// </summary>
        /// <returns>If the file was changed since last check.</returns>
        private bool ConfigFileChanged
        {
            get
            {
                if (!LastFileChange.Equals(File.GetLastWriteTime(ConfigPath)))
                {
                    LastFileChange = File.GetLastWriteTime(ConfigPath);
                    inputBucketPath = null;
                    processedBucketPath = null;
                    logFilePath = null;
                    activeBucketPath = null;
                    completedBucketPath = null;
                    maxRunningLocalJobs = null;
                    return true;
                }
                return false;
            }
        }

        private string? inputBucketPath;
        public override string InputBucketPath
        {
            get
            {
                if (ConfigFileChanged || string.IsNullOrWhiteSpace(inputBucketPath))
                {
                    try
                    {
                        inputBucketPath = GetAttributePathValueFromRunningOrDefaultNode("InputBucket");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return inputBucketPath;
            }
        }

        private string? processedBucketPath;
        public override string ProcessedBucketPath
        {
            get
            {
                if (ConfigFileChanged || null == processedBucketPath)
                {
                    try
                    {
                        processedBucketPath = GetAttributePathValueFromRunningOrDefaultNode("ProcessedBucket");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return processedBucketPath;
            }
        }

        private string? logFilePath;
        public override string LogFilePath
        {
            get
            {
                if (ConfigFileChanged || string.IsNullOrWhiteSpace(logFilePath))
                {
                    try
                    {
                        logFilePath = GetAttributePathValueFromRunningOrDefaultNode("LogFile");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return logFilePath;
            }
        }

        private string? activeBucketPath;
        public override string ActiveBucketPath
        {
            get
            {
                if (ConfigFileChanged || null == activeBucketPath)
                {
                    try
                    {
                        activeBucketPath = GetAttributePathValueFromRunningOrDefaultNode("ActiveBucket");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return activeBucketPath;
            }
        }

        private string? completedBucketPath;
        public override string CompletedBucketPath
        {
            get
            {
                if (ConfigFileChanged || null == completedBucketPath)
                {
                    try
                    {
                        completedBucketPath = GetAttributePathValueFromRunningOrDefaultNode("CompletedBucket");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return completedBucketPath;
            }
        }
        private KeyValuePair<string, string> dataBaseTypeAndString = new KeyValuePair<string, string>("", "");
        public override KeyValuePair<string, string> DBTypeAndString
        {
            get
            {
                if (ConfigFileChanged || string.IsNullOrWhiteSpace(dataBaseTypeAndString.Key))
                {
                    try
                    {
                        string dbType = "mock";
                        string connectionString = "";

                        //Check if the startup node has a useful entry for the DbConnection
                        var startupNode = XDocument.Load(XmlReader.Create(ConfigPath)).Root.Element("Startup");
                        bool startupNodeHasDbConnectionData = ((startupNode.HasElements &&
                                                                startupNode.Element("DbConnection").Attribute("type") != null &&
                                                                startupNode.Element("DbConnection").Attribute("string") != null));
                        //If it doesn't we just keep using 'mock' queue db
                        dbType = startupNodeHasDbConnectionData
                            ? startupNode.Element("DbConnection").Attribute("type").Value
                            : dbType;

                        connectionString = ("mock" == dbType)
                            ? ""
                            : startupNode.Element("DbConnection").Attribute("string").Value;

                        dataBaseTypeAndString = new KeyValuePair<string, string>(dbType, connectionString);
                    }
                    catch
                    {
                        throw;
                    }
                }

                return dataBaseTypeAndString;
            }
        }

        private int? maxRunningLocalJobs;
        public override int MaxRunningLocalJobs
        {
            get
            {
                if (null == maxRunningLocalJobs || ConfigFileChanged)
                {
                    string result;

                    var rootNode = XDocument.Load(XmlReader.Create(ConfigPath)).Root;

                    //Try to find the value from the running node first
                    if (!string.IsNullOrWhiteSpace(rootNode.Element("Running").Element("MaxJobs").Attribute("num").Value))
                    { result = rootNode.Element("Running").Element("MaxJobs").Attribute("num").Value; }
                    //Then the default node
                    else if (!string.IsNullOrWhiteSpace(rootNode.Element("Defaults").Element("MaxJobs").Attribute("num").Value))
                    { result = rootNode.Element("Defaults").Element("MaxJobs").Attribute("num").Value; }
                    //Then panic
                    else
                    {
                        throw new ApplicationException("Failed to read XML for " + "MaxJobs");
                    }
                    if (int.TryParse(result, out int jobMax))
					{
						maxRunningLocalJobs = jobMax;
					}
					else
					{
						throw new ApplicationException("Max jobs not an integer");
					}
                }
                return maxRunningLocalJobs.Value;
            }
        }

        private string GetAttributePathValueFromRunningOrDefaultNode(string attribute)
        {
            string result;

            var rootNode = XDocument.Load(XmlReader.Create(ConfigPath)).Root;

            //Try to find the value from the running node first
            if (!string.IsNullOrWhiteSpace(rootNode.Element("Running").Element(attribute).Attribute("path").Value))
            { result = rootNode.Element("Running").Element(attribute).Attribute("path").Value; }
            //Then the default node
            else if (!string.IsNullOrWhiteSpace(rootNode.Element("Defaults").Element(attribute).Attribute("path").Value))
            { result = rootNode.Element("Defaults").Element(attribute).Attribute("path").Value; }
            //Then panic
            else
            {
                throw new ApplicationException("Failed to read XML for " + attribute);
            }

            return result;
        }
    }
}