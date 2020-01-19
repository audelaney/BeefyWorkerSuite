#nullable enable
using AppLogic;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataObjects;

namespace MasterApp
{
    class Program
    {
        public enum MainMenuOptions
        {
            Quit,
            InputSplitter,
            OutputCombiner,
            SuiteDeployer
        }

        static void Main(string[] args)
        {
            WelcomePrint();

            GetConfig();

            bool running = true;

            do
            {
                var input = GetMainMenuOptionChoice();
                switch (input)
                {
                    case 1:
                        InputSplitter.Run();
                        break;
                    case 2:
                        OutputCombiner.Run();
                        break;
                    case 3:

                        break;
                    case 0:
                    default:
                        ExitPrint();
                        running = false;
                        break;
                }
            } while (running);
        }

        private static void GetConfig()
        {
            while (true)
            {
                var input = AppHelpers.GetFileInput("Input a config file path: ");
                if (input == null) { continue; }
                try
                {
                    AppConfigManager.SetConfig(input);
                    var testing = AppConfigManager.Instance.CompletedBucketPath;
                    var testingAgain = AppConfigManager.Instance.DBTypeAndString;
                    break;
                }
                catch { }
            }
        }

        private static uint GetMainMenuOptionChoice()
        {
            var options = (MainMenuOptions[])Enum.GetValues(typeof(MainMenuOptions));

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

        #region PrintHelpers

        private static void ExitPrint()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Exiting...");
            System.Console.WriteLine();
        }

        private static void WelcomePrint()
        {
            System.Console.WriteLine("Welcome");
            System.Console.WriteLine();
        }

        #endregion
    }
}