using System;
using DataObjects;
using DataAccess;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using DataAccess.Exceptions;
using AppConfig;
using AppConfig.Models;

namespace AppLogic
{
    internal class RealEncodeJobManager : EncodeJobManager, IConfigWatcher
    {
        /// Helper for using the logger
        private string PrintDB()
        {
            string result = " ";
            result += "database type: " + AppConfigManager.Model.DBTypeAndString.Key;
            if (string.IsNullOrWhiteSpace(AppConfigManager.Model.DBTypeAndString.Value))
            { result += ", no conn string."; }
            else
            { result += ", conn string: " + AppConfigManager.Model.DBTypeAndString.Value + "."; }
            return result;
        }

        private IEncodeJobDAO _jobAccessor;

        internal RealEncodeJobManager()
        {
            AppConfigManager.WatchForChanges(this);
            SetupFromConfig();
        }

        public override bool AddEncodeJobToQueue(EncodeJob newJob)
        {
            if (!newJob.IsValid)
            { throw new ArgumentException("Cannot add invalid job to queue: " + newJob.ToString()); }

            string message = $"Exception encountered while adding job: {newJob.ToString()} + {PrintDB()}";
            var result = false;

            try
            {
                if (Guid.Empty == newJob.Id)
                { newJob.Id = Guid.NewGuid(); }
                if (_jobAccessor.AddEncodeJobToQueue(newJob))
                { result = true; }
                else
                {
                    _logger?.LogWarning($"Attempted to add the same job to the datastore. Guid: {newJob.Id}");
                    var alreadyExistingJob = FindEncodeJob(newJob.Id);
                    var badGuid = newJob.Id;
                    newJob.Id = Guid.NewGuid();
                    if (null == alreadyExistingJob)
                    {
                        //try again i guess
                        _logger?.LogWarning("Trying again...");
                        if (!_jobAccessor.AddEncodeJobToQueue(newJob))
                        {
                            throw new ApplicationException($"A job was not found for old guid: {badGuid} and "
                            + $"job {newJob.ToString()} was still unable to be added.");
                        }
                    }
                    else if (alreadyExistingJob.Equals(newJob))
                    { throw new JobAlreadyExistsException($"Job: {newJob.ToString()}"); }
                    else
                    {
                        //Just try again with the new guid, should work for sure
                        result = _jobAccessor.AddEncodeJobToQueue(newJob);
                    }
                }
            }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (JobAlreadyExistsException jaee)
            {
                var up = new ApplicationException("That job already exists.", jaee);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                { _logger.LogError(ex, message); }

                result = false;
            }

            return result;
        }

        public override bool MarkJobCheckedOut(Guid id, bool checkedOutStatus)
        {
            if (Guid.Empty == id)
            { throw new ArgumentException("Cannot accept empty guid."); }

            string message = $"Exception encountered while checking out job with id: {id} {PrintDB()}";
            try
            {
                var result = false;
                DateTime? time = (checkedOutStatus) ? DateTime.Now : (DateTime?)null;
                result = _jobAccessor.MarkEncodeJobCheckedOut(id, time);
                return result;
            }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                {
                    _logger.LogError(ex, message);
                    return false;
                }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
                }
            }
        }

        public override bool MarkJobCheckedOut(EncodeJob job, bool checkedOutStatus)
        {
            if (!job.IsValid)
            { throw new ArgumentException($"Job is invalid: {job.ToString()}"); }

            string message = "Exception encountered while setting checked out status " 
                + $"to {checkedOutStatus} for job with id: {job.ToString()} {PrintDB()}";
            try
            {
                var result = false;
                if (checkedOutStatus)
                {
                    DateTime time = DateTime.Now;
                    result = _jobAccessor.MarkEncodeJobCheckedOut(job, time);
                    if (result) { job.CheckedOutTime = time; }
                }
                else
                {
                    result = _jobAccessor.MarkEncodeJobCheckedOut(job, null);
                    if (result) { job.CheckedOutTime = null; }
                }
                return result;
            }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                {
                    _logger.LogError(ex, message);
                    return false;
                }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
            }
        }
        }

        public override EncodeJob? FindEncodeJob(Guid id)
        {
            if (Guid.Empty == id)
            { throw new ArgumentException("Cannot find job for empty guid."); }

            string message = $"Exception encountered while finding job with id: {id} {PrintDB()}";
            try
            { return _jobAccessor.RetrieveEncodeJob(id); }
            catch (ApplicationException)
            { return null; }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                {
                    _logger.LogError(ex, message);
                    return null;
                }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
            }
        }
        }

        public override bool MarkJobComplete(EncodeJob job, bool completedStatus)
        {
            if (!job.IsValid)
            { throw new ArgumentException($"Job invalid: {job.ToString()}"); }

            string message = $"Exception encountered while completing job {job.ToString()} {PrintDB()}";
            try
            {return _jobAccessor.MarkJobCompletedStatus(job, completedStatus);}
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                {
                    _logger.LogError(ex, message);
                    return false;
                }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
            }
        }
        }

        public override bool MarkJobComplete(Guid id, bool completedStatus)
        {
            if (Guid.Empty == id)
            { throw new ArgumentException("Cannot mark complete for empty guid."); }

            string message = $"Exception encountered while completing job {id} {PrintDB()}";
            try
            { return _jobAccessor.MarkJobCompletedStatus(id, completedStatus); }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                {
                    _logger.LogError(ex, message);
                    return false;
                }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
            }
        }
        }

        public override bool UpdateJob(EncodeJob oldJob, EncodeJob job)
        {
            if (!oldJob.IsValid)
            { throw new ArgumentException($"Old job is invalid: {oldJob.ToString()}"); }
            if (!job.IsValid)
            { throw new ArgumentException($"New job is invalid: {job.ToString()}"); }

            string message = $"Exception encountered while updating job {oldJob.ToString()}"
                    + $" to new job {job.ToString()} {PrintDB()}";
            try
            { return _jobAccessor.UpdateJob(oldJob, job); }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                {
                    _logger.LogError(ex, message);
                    return false;
                }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
                }
            }
        }

        public override IEnumerable<EncodeJob> GetIncompleteUncheckedOutEncodeJobs(int priority)
        {
            IEnumerable<EncodeJob> output = new EncodeJob[0];
            string message = "Exception encountered while pulling incompleted job list. "
                    + $"Priority: {priority} {PrintDB()}";

            try
            {
                var result = _jobAccessor.RetrieveIncompleteEncodeJobs(priority);
                if (result.Count() != 0)
                { output = result; }
                else
                { _logger?.LogInformation($"No incomplete jobs found for priority {priority}"); }
            }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                { _logger.LogError(ex, message); }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
                }
            }

            return output;
        }

        public override IEnumerable<EncodeJob> GetIncompleteUncheckedOutEncodeJobs()
        {
            IEnumerable<EncodeJob> output = new EncodeJob[0];
            string message = $"Exception encountered while pulling incompleted unchecked-out job list. {PrintDB()}";

            try
            {
                var result = _jobAccessor.RetrieveIncompleteEncodeJobs();
                if (result.Count() != 0)
                { output = result; }
                else
                {
                    _logger?.LogInformation("No incomplete and unchecked out jobs found for any priority");
                }
            }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                { _logger.LogError(ex, message); }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
                }
            }

            return output;
        }


        public override IEnumerable<EncodeJob> GetIncompleteEncodeJobs(int priority)
        {
            IEnumerable<EncodeJob> output = new EncodeJob[0];
            string message = "Exception encountered while pulling incompleted job list. "
                    + $"Priority: {priority} {PrintDB()}";

            try
            {
                var result = _jobAccessor.RetrieveIncompleteEncodeJobs(priority);
                if (result.Count() != 0)
                { output = result; }
                else
                {
                    _logger?.LogInformation("No incomplete jobs found for priority " + priority);
                }
            }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                { _logger.LogError(ex, message); }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
                }
            }

            return output;
        }

        public override IEnumerable<EncodeJob> GetIncompleteEncodeJobs()
        {
            IEnumerable<EncodeJob> output = new EncodeJob[0];
            string message = $"Exception encountered while pulling incompleted job list. {PrintDB()}";

            try
            {
                var result = _jobAccessor.RetrieveIncompleteEncodeJobs();
                if (result.Count() != 0)
                { output = result; }
                else
                {
                    _logger?.LogInformation("No incomplete jobs found for any priority");
                }
            }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                { _logger.LogError(ex, message); }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
                }
            }

            return output;
        }

        public override IEnumerable<EncodeJob> GetJobsByVideoName(string videoName)
        {
            IEnumerable<EncodeJob> output = new EncodeJob[0];
            string message = $"Exception encountered while pulling jobs with video name: {videoName} {PrintDB()}";

            try
            {
                var result = _jobAccessor.RetrieveCompleteEncodeJobsByVideoName(videoName);
                if (result.Count() != 0)
                { output = result; }
                else
                { _logger?.LogInformation($"No jobs found with video name: {videoName}"); }
            }
            catch (DatabaseConnectionException dce)
            {
                //TODO cache logic
                var up = new ApplicationException(message, dce);
                throw up;
            }
            catch (Exception ex)
            {
                if (null != _logger)
                { _logger.LogError(ex, message); }
                else
                {
                    ex.Data["message"] = message;
                    throw ex;
                }
            }

            return output;
        }

        public void Notify() =>
            SetupFromConfig();

        private void SetupFromConfig()
        {
            _jobAccessor = AppConfigManager.Model.DBTypeAndString.Key switch
            {
                DbType.mongo => new EncodeJobDAOMongo(AppConfigManager.Model.DBTypeAndString.Value),
                DbType.mssql => new EncodeJobDAOmssql(AppConfigManager.Model.DBTypeAndString.Value),
                DbType.mockBadDb => new EncodeJobDAOMockBadDb(),
                _ => new EncodeJobDAOMockAlive()
            };
        }
    }
}