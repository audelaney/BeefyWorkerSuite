using System;

namespace DataAccess.Exceptions
{
    [System.Serializable]
    public class JobAlreadyExistsException : System.Exception
    {
        public JobAlreadyExistsException() { }
        public JobAlreadyExistsException(string message) : base(message) { }
        public JobAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
        protected JobAlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}