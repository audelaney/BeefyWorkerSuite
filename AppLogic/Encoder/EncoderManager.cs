#nullable enable
using DataAccess;
using System.Linq;
using AppLogic.Encoders;
using System.Collections.Generic;
using System;
using DataObjects;
using System.IO;

namespace AppLogic
{
    /// <summary>
    /// Encoding related logic operations
    /// </summary>
    public abstract class EncoderManager
    {
        /// <summary>
        /// Instance used for actual logical operations of the publicly available methods
        /// </summary>
        static public EncoderManager Instance
        {
            get
            {
                if (_instance == null)
                { _instance = new RealEncoderManager(); }
                return _instance;
            }
        }
        static private EncoderManager? _instance;

        /// <summary>
        /// Combines the resulting output from a successful round of transcodes
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Thrown if:
        ///     - Any of the jobs are not marked as completed
        ///     - Jobs don't have a <see cref="DataObjects.EncodeJob.Chunk" />
        ///     - Jobs that have a video source that doesn't match the first video
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Thrown if:
        ///     - A job is found which is marked as completed but does not have a directory in the
        ///     completed bucket.
        /// </exception>
        public abstract void CombineSuccessfulEncodes(EncodeJob[] jobs, string outputFileName);


        /// <summary>
        /// Opens an encoder and starts encoding a specified job
        /// </summary>
        public abstract void BeginEncodeJobAttempts(EncodeJob job, string encoderType);
    }
}