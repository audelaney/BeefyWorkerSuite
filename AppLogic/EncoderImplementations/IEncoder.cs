#nullable enable
using System;
using DataObjects;

namespace AppLogic.Encoders
{
    public interface IEncoder
    {
        /// <summary>
        /// Encodes the specified job, returning the new filename of the encode if successful,
        /// or null if unsuccessful.
        /// </summary>
        public string? Encode(EncodeJob encodeJob, string? outputDirectoryPath = null);
        public int EncodesRun { get; }
    }
}