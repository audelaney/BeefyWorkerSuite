using System;
using System.Collections.Generic;
using DataObjects;

namespace Tests
{
    public static class TestHelper
    {
        public static EncodeJob MakeJob(bool valid, int num = 4)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<EncodeJob> MakeJobs(bool valid, int num = 4)
        {
            throw new NotImplementedException();
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