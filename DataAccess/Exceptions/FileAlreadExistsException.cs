using System;

namespace DataAccess.Exceptions
{
    #pragma warning disable 1591
    [System.Serializable]
    public class FileAlreadyExistsException : System.Exception
    {
        public FileAlreadyExistsException() { }
        public FileAlreadyExistsException(string message) : base(message) { }
        public FileAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
        protected FileAlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}