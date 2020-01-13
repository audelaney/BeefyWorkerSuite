#nullable enable
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using DataObjects;
using DataAccess.Exceptions;

namespace DataAccess
{
    /// <summary>
    /// Mock data accessor object with a "live" repo attached.
    /// </summary>
    public class EncodeJobDAOMockAlive : IEncodeJobDAO
    {
        private List<EncodeJob> _repository = new List<EncodeJob>();
        public bool AddEncodeJobToQueue(EncodeJob job)
        {
            if (_repository.Where(j => j.Id == job.Id).Count() > 0)
            {
                foreach (var item in _repository)
                {
                    if (item.Equals(job))
                    { throw new JobAlreadyExistsException("Guid: " + job.Id); }
                }
                
                return false;
            }
            else
            {
                _repository.Add(job);
                return true;
            }
        }

        public bool MarkEncodeJobCheckedOut(Guid id, bool completed)
        {
            var item = _repository.FirstOrDefault(job => job.Id == id);
            if (item == null)
            {
                return false;
            }
            else
            {
                item.Completed = completed;
                return true;
            }
        }

        public bool MarkEncodeJobCheckedOut(EncodeJob job, bool checkedOut)
        {
            var item = _repository.FirstOrDefault(j => j.Id == job.Id);
            if (item == null)
            {
                return false;
            }
            else
            {
                item.CheckedOutTime = null;
                item.CheckedOutTime = (checkedOut) ?
                                    DateTime.Now : item.CheckedOutTime;
                return true;
            }
        }

        public bool MarkJobCompletedStatus(Guid id, bool completed)
        {
            var item = _repository.FirstOrDefault(j => j.Id == id);
            if (item == null)
            {
                return false;
            }
            else
            {
                item.Completed = completed;
                return true;
            }
        }

        public bool MarkJobCompletedStatus(EncodeJob job, bool completed)
        {
            var item = _repository.FirstOrDefault(j => j.Id == job.Id);
            if (item == null)
            {
                return false;
            }
            else
            {
                item.Completed = completed;
                return true;
            }
        }

        public bool RemoveEncodeJobFromQueue(Guid id)
        {
            var item = _repository.FirstOrDefault(j => j.Id == id);
            if (item == null)
            {
                return false;
            }
            else
            {
                return _repository.Remove(item);
            }
        }

        public IEnumerable<EncodeJob> RetrieveCompleteEncodeJobs()
        {
            return _repository.Where(j => j.Completed);
        }

        public EncodeJob RetrieveEncodeJob(Guid id)
        {
            var item = _repository.FirstOrDefault(j => j.Id == id);
            if (item == null)
            {
                throw new ApplicationException();
            }
            else
            {
                return item;
            }
        }

        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs()
        {
            return _repository.Where(j => !j.Completed);
        }

        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs(int priority)
        {
            return _repository.Where(j => !j.Completed && j.Priority == priority);
        }

        public bool UpdateJob(EncodeJob oldData, EncodeJob newData)
        {
            var count = _repository.Where(j => j.Equals(oldData)).Count();
            if (count == 1)
            {
                _repository.Remove(oldData);
                _repository.Add(newData);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}