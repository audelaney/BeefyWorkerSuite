using System;
using System.Linq;
using AppLogic;
using DataObjects;

namespace MasterApp
{
    public class JobModifier
    {
        public enum JobMenuOptions
        {
            FullJobReset,
            SetJobCheckedOut,
            PickAttemptToUseForCompletion
        }
        public static void Run()
        {
            AppHelpers.GetConfigInput();
            
            var keepGoing = true;
            while (keepGoing)
            {
                var id = AppHelpers.GetGuidInput();

                // if you get an empty ID, quit.
                if (Guid.Empty == id)
                { keepGoing = false; }
                else
                {
                    var job = EncodeJobManager.Instance.FindEncodeJob(id);
                    if (null == job)
                    { System.Console.WriteLine("Unable to locate job."); }
                    else
                    {
                        System.Console.WriteLine("Found job: " + job.ToString());

                        var choice = GetMainMenuOptionChoice();

                        switch (choice)
                        {
                            case 0:
                                break;
                            case 1:
                                System.Console.WriteLine("Unchecking out a job...");
                                if (EncodeJobManager.Instance.MarkJobCheckedOut(job, false))
                                { System.Console.WriteLine("Successful."); }
                                else
                                { System.Console.WriteLine("Failed."); }
                                break;
                            case 2:
                                break;                            
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private static uint GetMainMenuOptionChoice()
        {
            var options = (JobMenuOptions[])Enum.GetValues(typeof(JobMenuOptions));

            for (int i = 0; i < options.Length; i++)
            { System.Console.WriteLine(i + ") " + options[i]); }
            uint result = 0;

            try
            {
                string input = Console.ReadKey().KeyChar.ToString();
                System.Console.WriteLine();
                if (uint.TryParse(input, out uint parsedInput))
                {
                    if (parsedInput < options.Length)
                    { result = parsedInput; }
                    else
                    { System.Console.WriteLine("Option not available"); }
                }
                else
                { System.Console.WriteLine("Invalid int detected"); }
            }
            catch
            { }

            return result;
        }
    }
}