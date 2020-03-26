#nullable enable
using System;
using System.Text;

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
        #region Props
        /// <summary>
        /// The command line that was used for that attempt
        /// </summary>
        public string CommandLineArgs { get; set; } = "";
        /// <summary>
        /// The time that the attempt started
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;
        /// <summary>
        /// The time the attempt ended
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.Now;
        /// <summary>
        /// Resulting VMAF of the attempt
        /// </summary>
        public double VmafResult { get; set; } = 0;
        /// <summary>
        /// The file size of the resulting output file
        /// </summary>
        public ulong FileSize { get; set; } = 0;
        /// <summary>
        /// The output path that the file was originally pointed towards
        /// </summary>
        public string OriginalOutputPath { get; set; }
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
        #endregion

        /// <summary>
        /// Default constructor, initializes all values but is in an invalid state
        /// </summary>
        public EncodeAttempt(string outputPath)
        {
            OriginalOutputPath = outputPath;
            CommandLineArgs = "";
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            VmafResult = 0;
            FileSize = 0;
        }

        public override string ToString()
        {
            StringBuilder sb= new StringBuilder();

            sb.Append("CLI args: ");
            sb.Append(CommandLineArgs);

            sb.Append(", Start time: ");
            sb.Append(StartTime);

            sb.Append(", End time: ");
            sb.Append(EndTime);

            sb.Append(", VMAF result: ");
            sb.Append(VmafResult);

            sb.Append(", File size: ");
            sb.Append(FileSize.ToString());

            sb.Append(", Original path: ");
            sb.Append(OriginalOutputPath);

            return sb.ToString();
        }
    }
}