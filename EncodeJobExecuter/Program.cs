#nullable enable
using AppLogic;
using DataObjects;
using System;
using System.IO;

namespace EncodeJobExecuter
{
    public class Program
    {
        static void Main(string[] args)
        {
            // {target directory} {database type} {conn string}
            if (args.Length < 2 ||
                !Directory.Exists(args[0]))
            { PrintHelp(); return; }

            //Execution config file? TODO

            string targetDir = args[0];
            string dataStoreType = args[1];
            string? connString = null;
            try
            {
                connString = args[2];
            }
            catch
            {

            }

            Guid jobId = EncodeJobManager.GetGuidFromWorkingDirectory(targetDir);

            EncodeJob activeJob = dataStoreType switch
            {
                "mock" => BuildFakeJob(targetDir),
                _ => EncodeJobManager.Instance.FindEncodeJob(jobId)
                    ?? throw new ApplicationException("Job not found, check logs.")
            };

            if (!activeJob.IsValid)
            { PrintInvalidJob(activeJob); return; }

            EncoderManager.StartJob(activeJob, "vp9test");
        }
        public static void RunEncode(object? job)
        {
            EncodeJob? activeJob = job as EncodeJob;
            if (null == activeJob)
            { return; }

            EncodeJobManager.Instance.MarkJobCheckedOut(activeJob.Id, true);
            //Do the encode
            string encoderConfig = "LibaomFfmpeg";

            EncoderManager.StartJob(activeJob,encoderConfig);
        }

        public static EncodeJob BuildFakeJob(string vidDir)
        {
            return new EncodeJob
            {
                VideoFileName = "test-file.mkv",
                VideoDirectoryPath = vidDir,
                AdditionalCommandArguments = "-crf 30 -cpu-used 5",
                MinVmaf = 98,
                MaxAttempts = 5
            };
        }


        #region PrintHelpers
        private static void PrintInvalidJob(EncodeJob activeJob)
        {
            System.Console.WriteLine("Found invalid job: " + activeJob.ToString());
        }
        private static void PrintHelp()
        {
            System.Console.WriteLine("Normal run requires 2/3 arguments in the following order:");
            System.Console.WriteLine("{target directory} {database type} {conn string}");
            System.Console.WriteLine();
            System.Console.WriteLine("If a mock implementation is used the app will:");
            System.Console.WriteLine("1. Change to the specified directory");
            System.Console.WriteLine("2. Look for a file named " + BuildFakeJob(@"/home/").VideoFileName);
            System.Console.WriteLine("3. Run a shitty VP9 encode on it");
        }
        #endregion
    }
}