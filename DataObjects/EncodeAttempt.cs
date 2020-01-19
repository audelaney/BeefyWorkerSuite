#nullable enable
using System;

namespace DataObjects
{
    /// <summary>
    /// A record of the attempt made at performing a job
    /// </summary>
    /// <todo>
    /// <task>Machine identifier property</task>
    /// </todo>
    public class EncodeAttempt
    {
        /// <summary>
        /// The command line that was used for that attempt
        /// </summary>
        public string CommandLineArgs { get; set; }
        /// <summary>
        /// The time that the attempt started
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// The time the attempt ended
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// Resulting VMAF of the attempt
        /// </summary>
        public double VmafResult { get; set; }
        /// <summary>
        /// The file size of the resulting output file
        /// </summary>
        public ulong FileSize { get; set; }
        /// <summary>
        /// The output path that the file was originally pointed towards
        /// </summary>
        public string OriginalOutputPath { get; set; }

        /// <summary>
        /// Default constructor, initializes all values but is in an invalid state
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if:
        ///     - Output path is null
        /// </exception>
        public EncodeAttempt(string? outputPath)
        {
            if (outputPath == null)
                throw new ArgumentNullException("Ouputpath null.");
            OriginalOutputPath = outputPath;
            CommandLineArgs = "";
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            VmafResult = 0;
            FileSize = 0;
        }

        /// <summary>
        /// If the object qualifies as valid or not
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (VmafResult > 0 && VmafResult <= 100 &&
                        DateTime.Compare(EndTime, DateTime.Now) <= 0 &&
                        FileSize != 0 && 
                        !string.IsNullOrWhiteSpace(OriginalOutputPath));
            }
        }
    }
}