using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppLogic
{
	internal class MockEncodeJobManager : EncodeJobManager
	{
		private readonly EncodeJob[] jobs;
		internal MockEncodeJobManager()
		{
			jobs = new EncodeJob[9];
			for (int i = 0; i < 3; i++)
			{
				jobs[i] = new EncodeJob("billy.mp4")
				{
					Id = Guid.NewGuid(),
					ChunkNumber = (uint)i + 1,
					Chunk = new Scene(i * 10, (i + 1) * 10),
					VideoDirectoryPath = AppConfig.AppConfigManager.Model.ProcessedBucketPath,
					IngestDateTime = DateTime.Now.AddDays(-1)
				};
			}
			for (int i = 0; i < 3; i++)
			{
				jobs[i+3] = new EncodeJob("bobby.mp4")
				{
					Id = Guid.NewGuid(),
					ChunkNumber = (uint)i + 1,
					Chunk = new Scene(i * 10, (i + 1) * 10),
					VideoDirectoryPath = AppConfig.AppConfigManager.Model.ActiveBucketPath,
					IngestDateTime = DateTime.Now.AddDays(-1),
					CheckedOutTime = DateTime.Now
				};
			}
			for (int i = 0; i < 3; i++)
			{
				jobs[i + 6] = new EncodeJob("buddy.mp4")
				{
					Id = Guid.NewGuid(),
					ChunkNumber = (uint)i + 1,
					Chunk = new Scene(i * 10, (i + 1) * 10),
					VideoDirectoryPath = AppConfig.AppConfigManager.Model.ActiveBucketPath,
					Completed = true,
					IngestDateTime = DateTime.Now.AddDays(-1),
					CheckedOutTime = DateTime.Now.AddHours(-12)

				};
			}
		}
		public override bool AddEncodeJobToQueue(EncodeJob job)
		{
			if (!job.IsValid)
			{ throw new ArgumentException("Cannot add invalid job to queue: " + job.ToString()); }
			return true;
		}

		public override EncodeJob? FindEncodeJob(Guid id)
		{
			if (Guid.Empty == id)
			{ throw new ArgumentException("Cannot find job for empty guid."); }

			return jobs.FirstOrDefault(j => j.Id == id);
		}

		public override IEnumerable<EncodeJob> GetIncompleteEncodeJobs(int priority)
		{
			return GetIncompleteEncodeJobs().Where(j => j.Priority == priority);
		}

		public override IEnumerable<EncodeJob> GetIncompleteEncodeJobs()
		{
			return jobs.Where(j => !j.Completed);
		}

		public override IEnumerable<EncodeJob> GetIncompleteUncheckedOutEncodeJobs(int priority)
		{
			return GetIncompleteUncheckedOutEncodeJobs().Where(j => j.Priority == priority);
		}

		public override IEnumerable<EncodeJob> GetIncompleteUncheckedOutEncodeJobs()
		{
			return jobs.Where(j => !j.Completed && (j.CheckedOutTime == null));
		}

		public override IEnumerable<EncodeJob> GetJobsByVideoName(string videoName)
		{
			return jobs.Where(j => j.VideoFileName == videoName);
		}

		public override bool MarkJobCheckedOut(EncodeJob job, bool checkedOutStatus)
		{
			if (!job.IsValid)
			{ throw new ArgumentException($"Job is invalid: {job.ToString()}"); }
			return true;
		}

		public override bool MarkJobCheckedOut(Guid id, bool checkedOutStatus)
		{
			if (Guid.Empty == id)
			{ throw new ArgumentException("Cannot accept empty guid."); }
			return true;
		}

		public override bool MarkJobComplete(EncodeJob job, bool completedStatus)
		{
			var matchingJob = jobs.FirstOrDefault(j => j.Equals(job));

			if (matchingJob == null)
			{ return false; }
			else
			{
				matchingJob.Completed = completedStatus;
				return true;
			}
		}

		public override bool MarkJobComplete(Guid id, bool completedStatus)
		{
			if (Guid.Empty == id)
			{ throw new ArgumentException("Cannot mark complete for empty guid."); }
			return true;
		}

		public override bool UpdateJob(EncodeJob oldJob, EncodeJob job)
		{
			if (!job.IsValid)
			{ throw new ArgumentException($"Job invalid: {job.ToString()}"); }
			return true;
		}
	}
}