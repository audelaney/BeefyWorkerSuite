#nullable enable
using System;
using System.IO;
using DataObjects;
using AppLogic;
using System.Linq;
using System.Collections.Generic;

namespace MasterApp
{
    public class InputSplitter
    {
        public static void Run(string? videoInputPathInput = null, string? videoOutputDirectoryInput = null)
        {
            videoInputPathInput = AppHelpers.GetFileInput("Input video file to be split: ", videoInputPathInput);
            videoOutputDirectoryInput = AppHelpers.GetDirectoryInput("Output directory for video jobs: ", videoOutputDirectoryInput);

            if (null == videoInputPathInput || null == videoOutputDirectoryInput)
            {
                System.Console.WriteLine("One or both paths do not exist... exiting input splitter.");
                return;
            }

            string videoInputPath = videoInputPathInput;
            string videoOutputDirectory = videoOutputDirectoryInput;

            string videoName = Path.GetFileNameWithoutExtension(videoInputPath);

            System.Console.WriteLine("Using ffmepg to search for ideal points of scene change...");
            //Analyze the video and find scene changes
            var scenes = VideoManager.GetScenesFromVideo(videoInputPath); 
            System.Console.WriteLine($"Ended operation with {scenes.Count()} chunks. Transforming into jobs...");

            List<EncodeJob> jobs = new List<EncodeJob>();

            try
            { jobs = ConvertToUnNumberedJobs(scenes, Path.GetFileName(videoInputPath)); }
            catch (ApplicationException)
            {
                System.Console.WriteLine("User has quit manually, exiting without writing....");
                return;
            }
            catch (Exception up)
            { throw up; }

            System.Console.WriteLine($"{jobs.Count} jobs successfully built. Writing out to files...");

            for (int i = 0; i < jobs.Count; i++)
            {
                try
                {
                    jobs[i].ChunkNumber = (uint) i + 1;
                    using (StreamWriter sw = new StreamWriter(Path.Combine(videoOutputDirectory, videoName + ".chunk" + (i + 1) + ".json")))
                    {
                        sw.Write(EncodeJob.ToJson(jobs[i]));
                        sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                }
            }

            System.Console.WriteLine("All done, have a good day.");
        }

        private static List<EncodeJob> ConvertToUnNumberedJobs(Scene[] chunks, string filename)
        {
            System.Console.WriteLine("Job requirements will now be prompted. Leave prompt empty to use default, and enter 'quit' at any time to quit and exit.");
            System.Console.WriteLine("[] denotes valid range, [[]] denotes default.");
            int userMaxAttempts = 3;
            int userPriority = 3;
            double userMinVmaf = 90;
            double userMinPsnr = 35;
            string userAddtlCommands = "";
            int userAdjustmentFactor = 0;

            try
            {
                while (true)
                {
                    System.Console.Write("Please enter a number for job's max attempts [1-20][[3]]: ");
                    string input = System.Console.ReadLine();
                    if (input.Trim().ToLower() == "quit")
                    { throw new ApplicationException("User has quit."); }
                    if (string.IsNullOrEmpty(input)) { break; }
                    else if (int.TryParse(input, out int result))
                    {
                        if (0 < result && 20 >= result)
                        {
                            userMaxAttempts = result;
                            break;
                        }
                    }
                    System.Console.WriteLine("\nInvalid input, try again.");
                }
                while (true)
                {
                    System.Console.Write("Please enter a number for job's priority [1-3][[3]]: ");
                    string input = System.Console.ReadLine();
                    if (input.Trim().ToLower() == "quit")
                    { throw new ApplicationException("User has quit."); }
                    if (string.IsNullOrEmpty(input)) { break; }
                    else if (int.TryParse(input, out int result))
                    {
                        if (0 < result && 3 >= result)
                        {
                            userPriority = result;
                            break;
                        }
                    }
                    System.Console.WriteLine("\nInvalid input, try again.");
                }
                while (true)
                {
                    System.Console.Write("Please enter a number for job's adjustment factor [0-1][[0]]: ");
                    string input = System.Console.ReadLine();
                    if (input.Trim().ToLower() == "quit")
                    { throw new ApplicationException("User has quit."); }
                    if (string.IsNullOrEmpty(input)) { break; }
                    else if (int.TryParse(input, out int result))
                    {
                        if (0 <= result && 1 >= result)
                        {
                            userAdjustmentFactor = result;
                            break;
                        }
                    }
                    System.Console.WriteLine("\nInvalid input, try again.");
                }
                while (true)
                {
                    System.Console.Write("Please enter a number for job's minimum VMAF score [50.0-99.0][[90.0]]: ");
                    string input = System.Console.ReadLine();
                    if (input.Trim().ToLower() == "quit")
                    { throw new ApplicationException("User has quit."); }
                    if (string.IsNullOrEmpty(input)) { break; }
                    else if (double.TryParse(input, out double result))
                    {
                        if (50 <= result && 99 >= result)
                        {
                            userMinVmaf = result;
                            break;
                        }
                    }
                    System.Console.WriteLine("\nInvalid input, try again.");
                }
                while (true)
                {
                    System.Console.Write("Please enter a number for job's minimum PSNR score [20.0-50.0][[35.0]]: ");
                    string input = System.Console.ReadLine();
                    if (input.Trim().ToLower() == "quit")
                    { throw new ApplicationException("User has quit."); }
                    if (string.IsNullOrEmpty(input)) { break; }
                    else if (double.TryParse(input, out double result))
                    {
                        if (20 <= result && 50 >= result)
                        {
                            userMinPsnr = result;
                            break;
                        }
                    }
                    System.Console.WriteLine("\nInvalid input, try again.");
                }
                System.Console.WriteLine("Please note, there is no validation in place for user entered additional commands,");
                System.Console.WriteLine(" I severely recommend you don't fuck this part up.");

                System.Console.Write("Enter additional commands now: ");
                string commands = System.Console.ReadLine();
                if (commands.Trim().ToLower() == "quit")
                { throw new ApplicationException("User has quit."); }
                else if (!string.IsNullOrWhiteSpace(commands))
                { userAddtlCommands = commands; }
            }
            catch (Exception ex)
            { throw ex; }

            EncodeJob masterJob = new EncodeJob
            {
                MaxAttempts = userMaxAttempts,
                Priority = userPriority,
                MinVmaf = userMinVmaf,
                MinPsnr = userMinPsnr,
                AdditionalCommandArguments = userAddtlCommands,
                AdjustmentFactor = (AdjustmentFactor)userAdjustmentFactor,
                VideoFileName = filename
            };

            var output = chunks.Select(c =>
            {
                var job = (EncodeJob)masterJob.Clone();
                job.Chunk = new Scene(c.StartTime, c.EndTime);
                return job;
            }).ToList();

            return output;
        }
    }
}