#nullable enable
using Newtonsoft.Json;
using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Text;

namespace DataObjects
{
    public class EncodeJob : ICloneable
    {
        private static readonly string _configFilePathDefault = "/var/local/svt-config/hi.cfg";

        [BsonId]
        [JsonIgnore]
        public Guid Id { get; set; }
        public int ChunkNumber { get; set; }
        /// <summary>
        /// A very lazy way of designating a timespan from a video input that is being used for
        /// multiple encode jobs...
        /// </summary>
        /// <example>23.4-57.9001</example>
        public string? InputInterval { get; set; }
        /// <summary>
        /// The no path, file name with extension of the associated video.
        /// </summary>
        public string VideoFileName { get; set; }
        /// <summary>
        /// The path to the directory that the video will reside in, which should only be 
        /// changed for jobs where one video is used as a source for multiple jobs.
        /// </summary>
        public string VideoDirectoryPath { get; set; }
        /// <summary>
		/// Absolute path of the config file to be used. Can be used in generating command line
		/// arguments.
		/// </summary>
        public string ConfigFilePath { get; set; }
        /// <summary>
        /// Additional arguments to be provided to the encoder. Will probably eventually change
        /// to a config object of some kind. If null, returns empty string.
        /// </summary>
        public string AdditionalCommandArguments { get; set; }
        public int Priority { get; set; }
        public int MaxAttempts { get; set; }
        /// <summary>
        /// When performing a re-encode, favoring accuracy entails more settings changing
        /// more drastically.
        /// </summary>
        public double MinPsnr { get; set; }
        /// <summary>
        /// Minimum allowed VMAF by the "smart" encode
        /// </summary>
        public double MinVmaf { get; set; }
        /// <summary>
        /// When changing settings (TODO which are not implemented), adjustment factor
        /// factor is deterministic of what kind of steps will be taken to change those
        /// settings.
        /// </summary>
        public AdjustmentFactor AdjustmentFactor { get; set; }
        /// <summary>
        /// Date-time that this job was ingested into the system
        /// </summary>
        [JsonIgnore]
        public DateTime IngestDateTime { get; set; }
        /// <summary>
        /// Date-time that this job was assigned for encode execution
        /// </summary>
        public DateTime? CheckedOutTime { get; set; }
        /// <summary>
        /// Date-time that this job was marked as finished
        /// </summary>
        public DateTime? CompletedTime { get; set; }
        /// <summary>
        /// If the job has been completed currently.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EncodeJob()
        {
            Id = Guid.Empty;
            VideoDirectoryPath = "";
            VideoFileName = "";
            AdjustmentFactor = AdjustmentFactor.accuracy;
            ConfigFilePath = _configFilePathDefault;
            AdditionalCommandArguments = "";
            MinPsnr = 40;
            MinVmaf = 93;
            MaxAttempts = 3;
            Priority = 3;
            IngestDateTime = DateTime.Now;
            Completed = false;
            CompletedTime = null;
            CheckedOutTime = null;
            InputInterval = null;
            ChunkNumber = 0;
        }

        /// <summary>
        /// If this particular object is considered a "valid" object. Currently
        /// does not check for guid, so pre-database loaded objects can be
        /// validated.
        /// </summary>
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VideoFileName) ||
                    string.IsNullOrWhiteSpace(ConfigFilePath) ||
                    0 > Priority || 5 < Priority || 0 > ChunkNumber ||
                    0 > MaxAttempts || 0 > MinPsnr || 0 > MinVmaf)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Does not evaluate Id, completed, or any time fields.
        /// </summary>
        public override bool Equals(object? obj)
        {
            EncodeJob? otherJob = obj as EncodeJob;

            if (null == otherJob)
            {
                return false;
            }

            return (otherJob.AdditionalCommandArguments == AdditionalCommandArguments &&
                otherJob.AdjustmentFactor == AdjustmentFactor &&
                otherJob.ConfigFilePath == ConfigFilePath &&
                otherJob.InputInterval == InputInterval &&
                otherJob.MaxAttempts == MaxAttempts &&
                otherJob.MinPsnr == MinPsnr &&
                otherJob.MinVmaf == MinVmaf &&
                otherJob.Priority == Priority &&
                otherJob.VideoFileName == VideoFileName &&
                otherJob.VideoDirectoryPath == VideoDirectoryPath &&
                otherJob.ChunkNumber == ChunkNumber);
        }

        /// <summary>
        /// New object clone, empty ID
        /// </summary>
        public object Clone()
        {
            return new EncodeJob
            {
                VideoFileName = (string)this.VideoFileName.Clone(),
                AdditionalCommandArguments = (string)this.AdditionalCommandArguments.Clone(),
                AdjustmentFactor = this.AdjustmentFactor,
                VideoDirectoryPath = (string)this.VideoDirectoryPath.Clone(),
                MinPsnr = this.MinPsnr,
                MinVmaf = this.MinVmaf,
                ConfigFilePath = (string)this.ConfigFilePath.Clone(),
                InputInterval = (string?)this.InputInterval?.Clone(),
                MaxAttempts = this.MaxAttempts,
                Priority = this.Priority,
                Id = this.Id,
                Completed = this.Completed,
                CompletedTime = this.CompletedTime,
                CheckedOutTime = this.CheckedOutTime,
                IngestDateTime = this.IngestDateTime
            };
        }

        public static EncodeJob? FromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<EncodeJob>(json);
            }
            catch
            {
                return null;
            }
        }

        public static string? ToJson(EncodeJob? job)
        {
            if (null == job) { return ""; }

            try
            {
                return JsonConvert.SerializeObject(job, Formatting.Indented);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(InputInterval);
            hash.Add(VideoFileName);
            hash.Add(VideoDirectoryPath);
            hash.Add(ConfigFilePath);
            hash.Add(AdditionalCommandArguments);
            hash.Add(Priority);
            hash.Add(MaxAttempts);
            hash.Add(MinPsnr);
            hash.Add(MinVmaf);
            hash.Add(AdjustmentFactor);
            hash.Add(IngestDateTime);
            hash.Add(CheckedOutTime);
            hash.Add(CompletedTime);
            hash.Add(Completed);
            hash.Add(IsValid);
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append("GUID: ");
            output.Append(Id.ToString());
            output.AppendLine();

            if (ChunkNumber != 0)
            {
                output.Append("Chunk number: ");
                output.Append(ChunkNumber);
                output.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(InputInterval))
            {
                output.Append("Input interval: ");
                output.Append(InputInterval);
                output.AppendLine();
            }

            output.Append("Video file name: ");
            output.Append(VideoFileName);
            output.AppendLine();

            output.Append("Video directory: ");
            output.Append(VideoDirectoryPath);
            output.AppendLine();

            output.Append("Config path: ");
            output.Append(ConfigFilePath);
            output.AppendLine();

            output.Append("Additional arguments: ");
            output.Append(AdditionalCommandArguments);
            output.AppendLine();

            output.Append("Priority: ");
            output.Append(Priority);
            output.AppendLine();

            output.Append("Max attempts: ");
            output.Append(MaxAttempts);
            output.AppendLine();

            output.Append("Min psnr: ");
            output.Append(MinPsnr);
            output.AppendLine();

            output.Append("Min vmaf: ");
            output.Append(MinVmaf);
            output.AppendLine();

            return output.ToString();
        }
    }
}