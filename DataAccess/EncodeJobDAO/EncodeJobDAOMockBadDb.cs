using System;
using DataAccess.Exceptions;
using System.Collections.Generic;
using DataObjects;

namespace DataAccess
{
    /// <summary></summary>
    public class EncodeJobDAOMockBadDb : IEncodeJobDAO
    {
        /// <summary></summary>
        public bool AddEncodeJobToQueue(EncodeJob job)
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public bool MarkEncodeJobCheckedOut(Guid id, bool completed)
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public bool MarkEncodeJobCheckedOut(EncodeJob job, bool completed)
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public bool MarkJobCompletedStatus(Guid id, bool completed)
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public bool MarkJobCompletedStatus(EncodeJob job, bool completed)
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public bool RemoveEncodeJobFromQueue(Guid id)
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveCompleteEncodeJobs()
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public EncodeJob RetrieveEncodeJob(Guid id)
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs()
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs(int priority)
        {
            throw new DatabaseConnectionException();
        }

        /// <summary></summary>
        public bool UpdateJob(EncodeJob oldData, EncodeJob newData)
        {
            throw new DatabaseConnectionException();
        }
    }
}