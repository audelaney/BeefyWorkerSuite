using System;

namespace DataAccess.Exceptions
{
    /// <summary></summary>
    [System.Serializable]
    public class DatabaseConnectionException : System.Exception
    {
        /// <summary></summary>
        public DatabaseConnectionException() { }
        /// <summary></summary>
        public DatabaseConnectionException(string message) : base(message) { }
        /// <summary></summary>
        public DatabaseConnectionException(string message, System.Exception inner) : base(message, inner) { }
        /// <summary></summary>
        protected DatabaseConnectionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}