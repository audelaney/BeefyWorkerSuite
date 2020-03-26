using System;
using DataObjects;

namespace AppLogic
{
	internal class MockEncoderManager : EncoderManager
	{
		public override void BeginEncodeJobAttempts(EncodeJob job, string encoderType)
		{
			var outpath = EncodeJob.GenerateJobOutputFilename(job);
			var attempt = new EncodeAttempt(outpath)
			{
				StartTime = DateTime.Now.AddMinutes(1),
				FileSize = 1000000L,
				EndTime = DateTime.Now,
				VmafResult = job.MinVmaf + 1
			};
			job.Attempts.Add(attempt);
		}

		public override void CombineSuccessfulEncodes(EncodeJob[] jobs, string outputFileName)
		{
		}
	}
}