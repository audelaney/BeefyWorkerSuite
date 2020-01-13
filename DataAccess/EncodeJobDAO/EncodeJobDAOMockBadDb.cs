using System;
using DataAccess.Exceptions;
using System.Collections.Generic;
using DataObjects;

namespace DataAccess
{
    public class EncodeJobDAOMockBadDb : IEncodeJobDAO
    {
        public bool AddEncodeJobToQueue(EncodeJob job)
        {
            throw new DatabaseConnectionException();
        }

        public bool MarkEncodeJobCheckedOut(Guid id, bool completed)
        {
            throw new DatabaseConnectionException();
        }

        public bool MarkEncodeJobCheckedOut(EncodeJob job, bool completed)
        {
            throw new DatabaseConnectionException();
        }

        public bool MarkJobCompletedStatus(Guid id, bool completed)
        {
            throw new DatabaseConnectionException();
        }

        public bool MarkJobCompletedStatus(EncodeJob job, bool completed)
        {
            throw new DatabaseConnectionException();
        }

        public bool RemoveEncodeJobFromQueue(Guid id)
        {
            throw new DatabaseConnectionException();
        }

        public IEnumerable<EncodeJob> RetrieveCompleteEncodeJobs()
        {
            throw new DatabaseConnectionException();
        }

        public EncodeJob RetrieveEncodeJob(Guid id)
        {
            throw new DatabaseConnectionException();
        }

        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs()
        {
            throw new DatabaseConnectionException();
        }

        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs(int priority)
        {
            throw new DatabaseConnectionException();
        }

        public bool UpdateJob(EncodeJob oldData, EncodeJob newData)
        {
            throw new DatabaseConnectionException();
        }
    }
}