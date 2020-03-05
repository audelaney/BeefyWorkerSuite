#nullable enable
using DataObjects;
using System;
using DataAccess;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AppLogic
{
    /// <summary>
    /// Logical encode job operations
    /// </summary>
    public abstract class EncodeJobManager
    {
        /// <summary>
        /// For logging purposes
        /// </summary>
        protected static ILogger? _logger;
        private static EncodeJobManager? _instance;

        /// <summary>Pointer for manager operations</summary>
        public static EncodeJobManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new RealEncodeJobManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Improves the quality of a job by altering command line arguments
        /// </summary>
        /// <todo>This isn't even close to being done</todo>
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
                            job.AdditionalCommandArguments = job.AdditionalCommandArguments
                                                                .Replace("-crf " + currentCrf, "-crf " + newCrf);
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

        /// <summary>
        /// Passes a logger into the manager for logging purposes
        /// </summary>
        public static bool SetLogger(ILogger logger)
        {
            try
            {
                _logger = logger;
                return true;
            }
            catch
            { return false; }
        }

        //Manager abstraction

        /// <summary>
        /// Adds an encode job to the database queue to await encoding.
        /// </summary>
        /// <param name="job">The job to be added to the database queue.</param>
        /// <returns>If the operation was a success.</returns>
        /// <remarks>Should this generate and return a guid for guidless jobs?</remarks>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - Database becomes unreachable
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if:
        ///     - Job to add to the database is invalid
        /// </exception>
        abstract public bool AddEncodeJobToQueue(EncodeJob job);

        /// <summary>
        /// Searches the database for a job with the matching GUID, and if its unable to find
        /// it, returns null instead.
        /// </summary>
        /// <param name="id">The GUID to search the database for.</param>
        /// <returns>The encode job pulled from the database queue that matches the GUID, or 
        /// null if no matching jobs are found.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - Database becomes unreachable
        /// </exception>
        abstract public EncodeJob? FindEncodeJob(Guid id);

        /// <summary>
        /// Changes the completion state of a job in the database.
        /// </summary>
        /// <todo>Ensure that CompletedTime property is properly implemented.</todo>
        /// <param name="job">The job that should be marked as completed or not.</param>
        /// <param name="completedStatus">The new status of the job.</param>
        /// <returns>If the operation was a success.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool MarkJobComplete(EncodeJob job, bool completedStatus);

        /// <summary>
        /// Changes the completion state of a job in the database.
        /// </summary>
        /// <todo>Ensure that CompletedTime property is properly implemented.</todo>
        /// <param name="id">The Id of the job to mark completion for.</param>
        /// <param name="completedStatus">The new status of the job.</param>
        /// <returns>If the operation was a success.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool MarkJobComplete(Guid id, bool completedStatus);

        /// <summary>
        /// Marks a jobs checked out time in the database, and if successful sets
        /// the job objects checked out to the appropriate time.
        /// </summary>
        /// <param name="job">The job object that needs its status changed</param>
        /// <param name="checkedOutStatus">The new checked out status of the job</param>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool MarkJobCheckedOut(EncodeJob job, bool checkedOutStatus);

        /// <summary>
        /// Marks a jobs checked out state in the database.
        /// </summary>
        /// <param name="id">The Id of the job to mark completion for.</param>
        /// <param name="checkedOutStatus">The new checked out status of the job</param>
        /// <returns>If the operation was a success.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool MarkJobCheckedOut(Guid id, bool checkedOutStatus);

        /// <summary>
        /// Updates a job's information in the database, but not completed or checked out time.
        /// </summary>
        /// <param name="oldJob">The old job information, current in the database.</param>
        /// <param name="job">The new information to use for the job</param>
        /// <returns>If the operation was a success.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public bool UpdateJob(EncodeJob oldJob, EncodeJob job);

        /// <summary>
        /// Gets all jobs that are not completed and not checked out residing in the database.
        /// </summary>
        /// <returns>Generic collection of jobs, or an empty colletion if there are none.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - Database becomes unreachable
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if:
        ///     - If the priority is out of range.
        /// </exception>
        abstract public IEnumerable<EncodeJob> GetIncompleteUncheckedOutEncodeJobs(int priority);

        /// <summary>
        /// Gets all jobs that are not completed and not checked out residing in the database.
        /// </summary>
        /// <returns>Generic collection of jobs, or an empty colletion if there are none.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public IEnumerable<EncodeJob> GetIncompleteUncheckedOutEncodeJobs();

        /// <summary>
        /// Gets all jobs that are not completed residing in the database.
        /// </summary>
        /// <returns>Generic collection of jobs, or an empty colletion if there are none.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if:
        ///     - If the priority is out of range.
        /// </exception>
        abstract public IEnumerable<EncodeJob> GetIncompleteEncodeJobs(int priority);

        /// <summary>
        /// Gets all jobs that are not completed residing in the database.
        /// </summary>
        /// <returns>Generic collection of jobs, or an empty colletion if there are none.</returns>
        /// <exception cref="System.ApplicationException">
        /// Thrown if:
        ///     - database becomes unreachable
        /// </exception>
        abstract public IEnumerable<EncodeJob> GetIncompleteEncodeJobs();

        /// <summary>
        /// Gets all jobs that have a mtching video name in the database
        /// </summary>
        /// <returns>Collection of jobs, or an empty collection if there are none</returns>
        abstract public IEnumerable<EncodeJob> GetJobsByVideoName(string videoName);
    }
}