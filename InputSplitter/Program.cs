#nullable enable
using AppLogic;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataObjects;

namespace InputSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                HelpStatement();
                return;
            }

            string videoInputPath = args[0];
            string videoOutputDirectory = args[1];
            string videoName = Path.GetFileNameWithoutExtension(videoInputPath);

            System.Console.Write(string.Format("Checking file {0} exists... ", videoInputPath));
            System.Console.WriteLine(File.Exists(videoInputPath));
            System.Console.Write(string.Format("Checking dir {0} exists... ", videoOutputDirectory));
            System.Console.WriteLine(Directory.Exists(videoOutputDirectory));

            System.Console.WriteLine("Using ffmepg to search for ideal points of scene change...");
            //Analyze the video and find scene changes
            var scenes = SceneManager.AnalyzeVideoInput(videoInputPath);
            System.Console.WriteLine(string.Format("Ended operation with {0} chunks. Transofrming into jobs...", scenes.Count()));

            List<EncodeJob> jobs = new List<EncodeJob>();

            try
            {
                jobs = ConvertToUnNumberedJobs(scenes, Path.GetFileName(videoInputPath));
            }
            catch (ApplicationException)
            {
                System.Console.WriteLine("User has quit manually, exiting without writing....");
                return;
            }
            catch (Exception up)
            {
                throw up;
            }

            System.Console.WriteLine(jobs.Count + " jobs successfully built. Writing out to files...");

            for (int i = 0; i < jobs.Count; i++)
            {
                try
                {
                    jobs[i].ChunkNumber = i + 1;
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

        private static void HelpStatement()
        {
            System.Console.WriteLine("Argument 1: Input file");
            System.Console.WriteLine("Argument 2: Output directory");
        }

        private static List<EncodeJob> ConvertToUnNumberedJobs(Scene[] chunks, string filename)
        {
            System.Console.WriteLine("Job requirements will now be prompted. Leave prompt empty to use default, and enter 'quit' at any time to quit and exit.");
            System.Console.WriteLine("[] denotes valid range, [[]] denotes default.");
            int userMaxAttempts = 3;
            int userPriority = 3;
            double userMinVmaf = 90;
            double userMinPsnr = 35;
            string userConfigPath = "/var/local/svt-config/hi.cfg";
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
                while (true)
                {
                    System.Console.Write(string.Format("Please enter a path for the config file [[{0}]]: ", userConfigPath));
                    string input = System.Console.ReadLine();
                    if (input.Trim().ToLower() == "quit")
                    { throw new ApplicationException("User has quit."); }
                    if (string.IsNullOrEmpty(input)) { break; }
                    else if (File.Exists(input))
                    {
                        userConfigPath = input;
                        break;
                    }
                    System.Console.WriteLine("\nInvalid input, file does not exist, try again.");
                }
                System.Console.WriteLine("Please note, there is no validation in place for user entered additional commands,");
                System.Console.WriteLine(" I severely recommend you don't fuck this part up.");

                System.Console.Write("Enter additional commands now: ");
                string commands = System.Console.ReadLine();
                if (commands.Trim().ToLower() == "quit")
                { throw new ApplicationException("User has quit."); }
                else if (!string.IsNullOrWhiteSpace(commands))
                {
                    userAddtlCommands = commands;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            EncodeJob masterJob = new EncodeJob
            {
                MaxAttempts = userMaxAttempts,
                Priority = userPriority,
                MinVmaf = userMinVmaf,
                MinPsnr = userMinPsnr,
                ConfigFilePath = userConfigPath,
                AdditionalCommandArguments = userAddtlCommands,
                AdjustmentFactor = (AdjustmentFactor)userAdjustmentFactor,
                VideoFileName = filename
            };

            var output = chunks.Select(c =>
            {
                var job = (EncodeJob)masterJob.Clone();
                job.InputInterval = string.Format("{0}-{1}", c.StartTime, c.EndTime);
                return job;
            }).ToList();

            return output;
        }
    }
}