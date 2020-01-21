using System;

namespace DataAccess.Exceptions
{
        /// <summary></summary>
    [System.Serializable]
    public class JobAlreadyExistsException : System.Exception
    {
        /// <summary></summary>
        public JobAlreadyExistsException() { }
        /// <summary></summary>
        public JobAlreadyExistsException(string message) : base(message) { }
        /// <summary></summary>
        public JobAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
        /// <summary></summary>
        protected JobAlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}