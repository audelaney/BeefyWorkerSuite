#nullable enable
using System;
using DataObjects;

namespace AppLogic.Encoders
{
    /// <summary>
    /// Interface that an encoder is required to implement
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Encodes the specified job, returning the new filename of the encode if successful,
        /// or null if unsuccessful.
        /// </summary>
        public string? Encode(EncodeJob encodeJob, string outputPath);
    }
}