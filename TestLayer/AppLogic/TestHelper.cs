using System;
using System.Collections.Generic;
using AppConfig.Models;
using DataObjects;

namespace Tests
{
    public static class TestHelper
    {
        public static ConfigModel MakeConfig()
        {
            return new ConfigModel()
            {
                VideoAccessor = VideoAccessorType.fake,
                FileAccessor = FileAccessorType.workingFake,
                DBTypeAndString = new KeyValuePair<DbType, string>(DbType.mock, ""),
                CompletedBucketPath = "/completed/",
                InputBucketPath = "/input/",
                ProcessedBucketPath = "/processed"
            };
        }
        public static EncodeJob MakeJob(bool valid)
        {
            if (valid)
            {
                var job = new EncodeJob();
                job.VideoFileName = "realname.mkv";
                if (job.IsValid) {return job;}
                else
                { throw new TestHelperException("Job isn't valid"); }
            }
            else
            {
                var job = new EncodeJob();
                job.Priority = 10;
                if (!job.IsValid) {return job;}
                else
                { throw new TestHelperException("Job is valid"); }
            }
        }

        public static IEnumerable<EncodeJob> MakeJobs(bool valid, int num = 4)
        {
            var jobs = new List<EncodeJob>();

            for (int i = 0; i < num; i++)
            {
                var job = MakeJob(valid:valid);
                job.ChunkNumber = (uint)(i+1);
                var scene = new Scene(i, job.ChunkNumber);
                job.Chunk = scene;
                jobs.Add(job);
            }

            return jobs;
        }

        /// <summary>
        /// Injects a default of 3 valid attempts into an encode job
        /// </summary>
        public static void PopulateValidJobWithValidAttempts(this EncodeJob job, int quantity = 3)
        {
            if (!job.IsValid)
                throw new TestHelperException("Job isn't valid");

            // Add attempts

            if (!job.IsValid) 
                throw new TestHelperException("Something went wrong while adding attempts");

            throw new NotImplementedException();
        }
    }

    [System.Serializable]
    public class TestHelperException : System.Exception
    {
        public TestHelperException() { }
        public TestHelperException(string message) : base(message) { }
        public TestHelperException(string message, System.Exception inner) : base(message, inner) { }
        protected TestHelperException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}