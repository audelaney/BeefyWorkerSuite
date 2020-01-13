#nullable enable
using DataObjects;
using System;
using DataAccess;
using System.IO;
using DataAccess.Vmaf;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AppLogic
{
    public abstract class EncodeJobManager
    {
        protected static ILogger? _logger;
        private static EncodeJobManager? _instance;
        public static EncodeJobManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new RealEncodeJobManager(AppConfigManager.Instance.DBTypeAndString.Key,
                                                         AppConfigManager.Instance.DBTypeAndString.Value);
                }
                return _instance;
            }
        }

        internal static void ConfigReset()
        {
            _instance = null;
        }

        //Encoder based method TODO
        public static void ImproveQuality(EncodeJob job)
        {
            string[] oldCommandSplit = job.AdditionalCommandArguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (oldCommandSplit.Contains("-crf"))
            {
                try
                {
                    for (int i = 0; i < oldCommandSplit.Length; i++)
                    {
                        if (oldCommandSplit[i] == "-crf" && int.TryParse(oldCommandSplit[i + 1], out int currentCrf))
                        {
                            int newCrf = currentCrf - 2;
                            job.AdditionalCommandArguments.Replace("-crf " + currentCrf, "-crf " + newCrf);
                        }
                    }
                }
                catch { }
            }

            if (oldCommandSplit.Contains("-b:v"))
            {
                try
                {
                    for (int i = 0; i < oldCommandSplit.Length; i++)
                    {
                        //Not implemented
                    }
                }
                catch { }
            }
        }
        
        //Encoder based method TODO
        public static bool EncodeMeetsRequirements(EncodeJob activeJob, string outputFileName)
        {
            string inputPath = Path.Combine(activeJob.VideoDirectoryPath, activeJob.VideoFileName);
            string outputPath = Path.Combine(activeJob.VideoDirectoryPath, outputFileName);

            //TODO if files don't exist


            double vmaf = 0;

            if (activeJob.InputInterval == null)
            {
                vmaf = VmafAccessor.GetVmaf(inputPath, outputPath);
            }
            else
            {
                double sceneStartTime = 0;
                double sceneEndTime = 10;
                vmaf = VmafAccessor.GetVmafScene(inputPath, outputPath, sceneStartTime, sceneEndTime);
            }

            return (vmaf >= activeJob.MinVmaf);
        }

        /// <summary>
        /// Gets the guid from a designated working directory, passing back an empty if the directory 
        /// is found/exists but does not translate to a guid.
        /// </summary>
        public static Guid GetGuidFromWorkingDirectory(string directory)
        {
            string guid = new DirectoryInfo(directory).Name;
            try
            {
                return new Guid(guid);
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public static bool SetLogger(ILogger logger)
        {
            try
            {
                _logger = logger;
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Manager abstraction

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>Should this generate and return a guid for guidless jobs?</remarks>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if:
        ///     - Job to add to the database is invalid
        /// </exception>
        abstract public bool AddEncodeJobToQueue(EncodeJob job);
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public EncodeJob? FindEncodeJob(Guid id);
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool MarkJobComplete(EncodeJob job, bool completedStatus);
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool MarkJobComplete(Guid id, bool completedStatus);
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool MarkJobCheckedOut(EncodeJob job, bool checkedOutStatus);
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool MarkJobCheckedOut(Guid id, bool checkedOutStatus);
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool UpdateJob(EncodeJob oldJob, EncodeJob job);
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public IEnumerable<EncodeJob> GetIncompleteUncheckedOutEncodeJobs(int priority);
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public IEnumerable<EncodeJob> GetIncompleteUncheckedOutEncodeJobs();
        
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public IEnumerable<EncodeJob> GetIncompleteEncodeJobs(int priority);

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public IEnumerable<EncodeJob> GetIncompleteEncodeJobs();
    }
}