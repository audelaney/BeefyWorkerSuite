#nullable enable
using Newtonsoft.Json;
using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DataObjects
{
    public class EncodeJob : ICloneable
    {
        #region Props
        [BsonId]
        [JsonIgnore]
        public Guid Id { get; set; }
        /// <summary>
        /// The chunk number of the video that it is being sliced from. Default 0 for no chunk.
        /// </summary>
        public uint ChunkNumber { get; set; }
        /// <summary>
        /// A very lazy way of designating a timespan from a video input that is being used for
        /// multiple encode jobs...
        /// </summary>
        /// <example>23.4-57.9001</example>
        public Scene? Chunk { get; set; }
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
        /// Additional arguments to be provided to the encoder. Will probably eventually change
        /// to a config object of some kind. If null, returns empty string.
        /// </summary>
        public string AdditionalCommandArguments { get; set; }
        public int Priority { get; set; }
        /// <summary>
        /// How many times the encoder should attempt to change the arguments
        /// used to run the job, per execution pass.
        /// </summary>
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
        /// If the job has been completed currently.
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// Record of the attempts made to perform the encode
        /// </summary>
        public List<EncodeAttempt> Attempts { get; set; }
        [JsonIgnore]
        [BsonIgnore]
        public bool IsChunk
        {
            get
            { 
                if (Chunk == null)
                    return false;
                if (ChunkNumber == 0)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// If this particular object is considered a "valid" object. Currently
        /// does not check for guid, so pre-database loaded objects can be
        /// validated.
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public bool IsValid
        {
            get
            {
                if (0 > Priority || 5 < Priority || 
                    0 > MaxAttempts || 0 > MinPsnr || 0 > MinVmaf ||
                    (Attempts.Where(a => !a.IsValid).Count() != 0))
                { return false; }
                else
                {
                    return true;
                }
            }
        }
        #endregion

        #region Instance methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public EncodeJob()
        {
            Id = Guid.Empty;
            VideoDirectoryPath = "";
            VideoFileName = "";
            AdjustmentFactor = AdjustmentFactor.accuracy;
            AdditionalCommandArguments = "";
            MinPsnr = 40;
            MinVmaf = 93;
            MaxAttempts = 3;
            Priority = 3;
            IngestDateTime = DateTime.Now;
            Completed = false;
            CheckedOutTime = null;
            Chunk = null;
            ChunkNumber = 0;
            Attempts = new List<EncodeAttempt>();
        }

        /// <summary>
        /// Does not evaluate Id, completed, time fields, or attempts.
        /// </summary>
        public override bool Equals(object? obj)
        {
            EncodeJob? otherJob = obj as EncodeJob;

            if (null == otherJob)
            { return false; }

            return (otherJob.AdditionalCommandArguments == AdditionalCommandArguments &&
                otherJob.AdjustmentFactor == AdjustmentFactor &&
                otherJob.Chunk == Chunk &&
                otherJob.MaxAttempts == MaxAttempts &&
                otherJob.MinPsnr == MinPsnr &&
                otherJob.MinVmaf == MinVmaf &&
                otherJob.Priority == Priority &&
                otherJob.VideoFileName == VideoFileName &&
                otherJob.VideoDirectoryPath == VideoDirectoryPath &&
                otherJob.ChunkNumber == ChunkNumber);
        }

        /// <summary>
        /// New object clone, empty ID, without attempts.
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
                Chunk = this.Chunk?.Clone() as Scene,
                MaxAttempts = this.MaxAttempts,
                Priority = this.Priority,
                Completed = this.Completed,
                CheckedOutTime = this.CheckedOutTime,
                IngestDateTime = this.IngestDateTime,
                ChunkNumber = this.ChunkNumber
            };
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Chunk);
            hash.Add(VideoFileName);
            hash.Add(VideoDirectoryPath);
            hash.Add(AdditionalCommandArguments);
            hash.Add(Priority);
            hash.Add(MaxAttempts);
            hash.Add(MinPsnr);
            hash.Add(MinVmaf);
            hash.Add(AdjustmentFactor);
            hash.Add(IngestDateTime);
            hash.Add(CheckedOutTime);
            hash.Add(Completed);
            hash.Add(IsValid);
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append($"GUID: {Id.ToString()}");
            output.AppendLine();

            if (ChunkNumber != 0)
            {
                output.Append($"Chunk number: {ChunkNumber}");
                output.AppendLine();
            }

            if (Chunk != null)
            {
                output.Append("-- Chunk Data --");
                output.AppendLine();
                output.Append(Chunk.ToString());
                output.AppendLine();
            }

            output.Append($"Video file name: {VideoFileName}");
            output.AppendLine();

            output.Append($"Video directory: {VideoDirectoryPath}");
            output.AppendLine();

            output.Append($"Additional arguments: {AdditionalCommandArguments}");
            output.AppendLine();

            output.Append($"Priority: {Priority}");
            output.AppendLine();

            output.Append($"Max attempts: {MaxAttempts}");
            output.AppendLine();

            output.Append($"Min psnr: {MinPsnr}");
            output.AppendLine();

            output.Append($"Min vmaf: {MinVmaf}");
            output.AppendLine();

            for (int i = 0; i < Attempts.Count; i++)
            {
                output.Append($"Attempt #{(i + 1)}: {Attempts[i].ToString()}");
                output.AppendLine();
            }

            return output.ToString();
        }

        /// <summary>
        /// Returns true if the most recent attempt attached to the obj meets the
        /// required vmaf of the object, and returns false otherwise.
        /// </summary>
        public bool DoesMostRecentAttemptMeetRequirements()
        {
            var attempt = GetMostRecentAttempt();
            if (attempt == null)
            { return false; }
            else
            { return attempt.VmafResult > MinVmaf; }
        }

        /// <summary>
        /// Gets the most recent encode attempt for this job.
        /// </summary>
        public EncodeAttempt? GetMostRecentAttempt()
        {
            if (Attempts.Count == 0)
            { return null; }
            else
            { return Attempts.OrderBy(j => j.EndTime).First(); }
        }

        /// <summary>
        /// First tries to return the smallest filesize that matches MinVmaf
        /// Failing to do that will return the largest Vmaf
        /// </summary>
        public EncodeAttempt? GetBestAttempt()
        {
            if (Attempts.Count == 0)
            { return null; }
            else
            {
                var meetsVmaf = Attempts.Where(a => a.VmafResult >= this.MinVmaf);
                if (meetsVmaf.Count() != 0)
                { return meetsVmaf.OrderByDescending(a => a.FileSize).First(); }
                else
                { return Attempts.OrderBy(a => a.VmafResult).First(); }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DoesBestAttemptMeetRequirements()
        {
            var attempt = GetBestAttempt();
            if (attempt == null)
            { return false; }
            else
            { return attempt.VmafResult > MinVmaf; }
        }
        #endregion

        #region Static methods
        public static EncodeJob? FromJson(string json)
        {
            try
            { return JsonConvert.DeserializeObject<EncodeJob>(json); }
            catch
            { return null; }
        }

        public static string? ToJson(EncodeJob? job)
        {
            if (null == job) { return ""; }

            try
            { return JsonConvert.SerializeObject(job, Formatting.Indented); }
            catch (System.Exception)
            { return null; }
        }
        public static string GenerateJobOutputFilename(EncodeJob job)
        {
            string result = Path.GetFileNameWithoutExtension(job.VideoFileName);
            result += (job.IsChunk) ? ".chunk" + job.ChunkNumber : string.Empty;
            result += ".attempt" + (job.Attempts.Count + 1);
            result += ".mkv";
            return result;
        }
        #endregion
    }
}