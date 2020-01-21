#nullable enable
using System;

namespace DataAccess.Exceptions
{
    /// <summary></summary>
    [System.Serializable]
    public class BadConnectionStringException : System.Exception
    {
        /// <summary></summary>
        public string ConnectionString { get; set; }
        /// <summary></summary>
        public BadConnectionStringException(string connString)
        { ConnectionString = connString; }
        /// <summary></summary>
        public BadConnectionStringException(string connString, string message) : base(message)
        { ConnectionString = connString; }
        /// <summary></summary>
        public BadConnectionStringException(string connString, string message, System.Exception inner) : base(message, inner)
        { ConnectionString = connString; }
        /// <summary></summary>
        protected BadConnectionStringException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { ConnectionString = ""; }
    }
}