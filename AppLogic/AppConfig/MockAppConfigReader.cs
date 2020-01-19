using System.Collections.Generic;

namespace AppLogic
{
    internal class MockAppConfigReader : AppConfigManager
    {
		internal MockAppConfigReader(string? mockDatabaseType)
		{ _type = mockDatabaseType ?? "mock"; }

        public override string InputBucketPath => "/home/videouser/input";

        public override string ProcessedBucketPath => "/home/videouser/processed";

        public override string ActiveBucketPath => "/home/videouser/jobs/active";

        public override string CompletedBucketPath => "/home/videouser/jobs/completed";

        public override string LogFilePath => "/home/videouser/app.log";

        private readonly string _type;
		public override KeyValuePair<string, string> DBTypeAndString => new KeyValuePair<string, string>(_type, "");

		public override int MaxRunningLocalJobs => 5;

		public override string FailedBucketPath => "/home/videouser/jobs/failed";
	}
}