#nullable enable
using DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using DataObjects;
using System.Linq;

namespace DataAccess
{
    /// <summary></summary>
    public class EncodeJobDAOMongo : IEncodeJobDAO
    {
        private static readonly string DEFAULT_DB_NAME = "encodedb";
        private static readonly string encodeJobCollectionName = "prodencodejob";
        private readonly IMongoDatabase database;
        /// <summary></summary>
        public EncodeJobDAOMongo(string? connString = null, string? dbName = null)
        {
            try
            {
                var client = (string.IsNullOrWhiteSpace(connString)) ?
                        new MongoClient() : new MongoClient(connString);
                database = (string.IsNullOrWhiteSpace(dbName)) ?
                            client.GetDatabase(DEFAULT_DB_NAME)
                            : client.GetDatabase(dbName);
            }
            catch (MongoConfigurationException mce)
            {
                throw new BadConnectionStringException(connString ?? "NULL", "", mce);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary></summary>
        public bool AddEncodeJobToQueue(EncodeJob job)
        {
            if (!job.IsValid)
            {
                var argex = new ArgumentException("Job submitted to queue is invalid");
                argex.Data["job"] = job;
                throw argex;
            }

            try
            {
                int attempt = 0;
                do
                {
                    attempt++;
                    if (job.Id == Guid.Empty)
                    { job.Id = Guid.NewGuid(); }
                    var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                    try { collection.InsertOne(job); }
                    catch (MongoWriteException)
                    { job.Id = Guid.Empty; }
                    if (attempt > 5)
                    { throw new ApplicationException("Exceeding 5 attempts adding job to database."); }
                } while (job.Id == Guid.Empty);
            }
            catch (System.Exception ex)
            {
                ex.Data["job"] = job;
                throw ex;
            }

            return true;
        }

        /// <summary></summary>
        public bool MarkEncodeJobCheckedOut(EncodeJob job, DateTime? checkedOutTime)
        {
            throw new NotImplementedException("Haven't worked this one out yet");
            
            if (!job.IsValid)
            {
                var argex = new ArgumentException("Job submitted to queue is invalid");
                argex.Data["job"] = job;
                throw argex;
            }

            bool result = true;
            if (job.Id == Guid.Empty)
            {
                throw new NotImplementedException("Need smarter guidless interactions");
                try
                {
                    var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                    var filter = Builders<EncodeJob>.Filter.Eq("VideoFileName", job.VideoFileName) &
                                    Builders<EncodeJob>.Filter.Eq("VideoFilePath", job.VideoDirectoryPath) &
                                    Builders<EncodeJob>.Filter.Eq("AdditionalCommandArguments", job.AdditionalCommandArguments) &
                                    Builders<EncodeJob>.Filter.Eq("Completed", job.Completed) &
                                    Builders<EncodeJob>.Filter.Eq("Priority", job.Priority);
                    var update = Builders<EncodeJob>.Update.Set("CheckedOutTime", DateTime.Now);
                    collection.UpdateOne(filter, update);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                result = MarkEncodeJobCheckedOut(job.Id, checkedOutTime);
            }
            return result;
        }

        /// <summary></summary>
        public bool MarkEncodeJobCheckedOut(Guid id, DateTime? checkedOutTime)
        {
            if (id == Guid.Empty)
            { throw new ArgumentException("Empty guid in CheckoutEncodeJob method."); }
            try
            {
                var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                var filter = Builders<EncodeJob>.Filter.Eq("_id", id);
                var update = Builders<EncodeJob>.Update.Set("CheckedOutTime", DateTime.Now);
                collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = false });
                var job = RetrieveEncodeJob(id);
                if (job.CheckedOutTime == null)
                { throw new ApplicationException("Unable to update checkout time for item guid: " + id); }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        /// <summary></summary>
        public bool MarkJobCompletedStatus(Guid id, bool completed)
        {
            if (id == Guid.Empty)
            { throw new ArgumentException("Empty guid in MarkJobComplete method."); }
            try
            {
                var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                var filter = Builders<EncodeJob>.Filter.Eq("_id", id);
                var update = Builders<EncodeJob>.Update.Set("Completed", completed);
                var result = collection.UpdateOne(filter, update);
                return result.MatchedCount == 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary></summary>
        public bool MarkJobCompletedStatus(EncodeJob job, bool completed)
        {
            if (!job.IsValid)
            {
                var argex = new ArgumentException("Job submitted to queue is invalid");
                argex.Data["job"] = job;
                throw argex;
            }
            bool result = true;
            if (job.Id == Guid.Empty)
            {
                throw new NotImplementedException("Need revised guidless operations");
                try
                {
                    var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                    var filter = Builders<EncodeJob>.Filter.Eq("VideoFileName", job.VideoFileName) &
                                Builders<EncodeJob>.Filter.Eq("VideoFilePath", job.VideoDirectoryPath) &
                                Builders<EncodeJob>.Filter.Eq("AdditionalCommandArguments", job.AdditionalCommandArguments) &
                                Builders<EncodeJob>.Filter.Eq("Completed", job.Completed) &
                                Builders<EncodeJob>.Filter.Eq("Priority", job.Priority);
                    var update = Builders<EncodeJob>.Update.Set("Completed", bool.TrueString);
                    collection.UpdateOne(filter, update);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                result = MarkJobCompletedStatus(job.Id,completed);
            }
            return result;
        }

        /// <summary></summary>
        public bool RemoveEncodeJobFromQueue(Guid id)
        {
            if (id == Guid.Empty)
            { throw new ArgumentException("Empty guid in RemoveEncodeJobFromQueue method."); }
            throw new NotImplementedException();
        }

        /// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs()
        {
            IEnumerable<EncodeJob> result = new List<EncodeJob>();

            try
            {
                var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                var filter = Builders<EncodeJob>.Filter.Eq("Completed", bool.FalseString);
                result = collection.Find<EncodeJob>(filter).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs(int priority)
        {
            IEnumerable<EncodeJob> result = new List<EncodeJob>();

            try
            {
                var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                var filter = Builders<EncodeJob>.Filter.Eq("Completed", bool.FalseString) &
                                Builders<EncodeJob>.Filter.Eq("Priority", priority);
                result = collection.Find<EncodeJob>(filter).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary></summary>
        public EncodeJob RetrieveEncodeJob(Guid id)
        {
            if (id == Guid.Empty)
            { throw new ArgumentException("Empty guid in GetEncodeJob method."); }

            try
            {
                var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                var filter = Builders<EncodeJob>.Filter.Eq("_id", id);
                var result = collection.Find(filter).ToList();
                if (1 == result.Count)
                { return result.First(); }
                else if (0 == result.Count)
                { throw new ApplicationException("No result for guid found."); }
                else
                { throw new ApplicationException("Guid exists in multiple records."); }
            }
            catch (ApplicationException apex)
            {
                apex.Data["guid"] = id;
                throw apex;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveCompleteEncodeJobs()
        {
            IEnumerable<EncodeJob> result = new List<EncodeJob>();

            try
            {
                var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                var filter = Builders<EncodeJob>.Filter.Eq("Completed", bool.TrueString);
                result = collection.Find<EncodeJob>(filter).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary></summary>
        public bool UpdateJob(EncodeJob oldData, EncodeJob newData)
        {
            if (!oldData.IsValid)
            {
                var argex = new ArgumentException("Job submitted to update from is invalid");
                argex.Data["job"] = oldData;
                throw argex;
            }
            if (!newData.IsValid)
            {
                var argex = new ArgumentException("Job submitted to update to is invalid");
                argex.Data["job"] = newData;
                throw argex;
            }
            if (oldData.Id != newData.Id)
            {
                var argex = new ArgumentException("Job id doesn't match");
                argex.Data["job"] = newData;
                throw argex;
            }
            bool result = true;
            try
            {
                var collection = database.GetCollection<EncodeJob>(encodeJobCollectionName);
                var filter = Builders<EncodeJob>.Filter.Eq("_id", oldData.Id);
                var options = new UpdateOptions { IsUpsert = true };
                var replaceResult = collection.ReplaceOne(filter, newData, options);
                result = replaceResult.MatchedCount == 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}