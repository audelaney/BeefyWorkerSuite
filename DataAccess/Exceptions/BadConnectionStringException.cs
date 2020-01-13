#nullable enable
using System;

namespace DataAccess.Exceptions
{
    [System.Serializable]
    public class BadConnectionStringException : System.Exception
    {
        public string ConnectionString { get; set; }
        public BadConnectionStringException(string connString)
        {
            ConnectionString = connString;
        }
        public BadConnectionStringException(string connString, string message) : base(message)
        {
            ConnectionString = connString;
        }
        public BadConnectionStringException(string connString, string message, System.Exception inner) : base(message, inner)
        {
            ConnectionString = connString;
        }
        protected BadConnectionStringException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            ConnectionString = "";
        }
    }
}