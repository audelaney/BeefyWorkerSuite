using System;

namespace DataAccess.Exceptions
{
    [System.Serializable]
    public class DatabaseConnectionException : System.Exception
    {
        public DatabaseConnectionException() { }
        public DatabaseConnectionException(string message) : base(message) { }
        public DatabaseConnectionException(string message, System.Exception inner) : base(message, inner) { }
        protected DatabaseConnectionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}