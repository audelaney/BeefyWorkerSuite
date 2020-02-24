using System;
using System.IO;
using AppLogic;

namespace MasterApp
{
    public class AppHelpers
    {
        /// <summary>
        /// Outputs a message and gets a valid file from the user
        /// </summary>
        /// <returns>A valid file path, or null</returns>
        public static string? GetFileInput(string message, string? preExistingPath = null)
        {
            if (string.IsNullOrEmpty(preExistingPath))
            {
                System.Console.Write(message);
                string file = System.Console.ReadLine();
                if (File.Exists(file))
                { return file; }
                else
                { return null; }
            }
            else
            {
                if (File.Exists(preExistingPath))
                { return preExistingPath; }
                else
                { return GetFileInput(message); }
            }
        }

        public static string? GetDirectoryInput(string message, string? preExistingPath = null)
        {
            if (string.IsNullOrEmpty(preExistingPath))
            {
                System.Console.Write(message);
                string directory = System.Console.ReadLine();
                if (Directory.Exists(directory))
                { return directory; }
                else
                { return null; }
            }
            else
            {
                if (Directory.Exists(preExistingPath))
                { return preExistingPath; }
                else
                { return GetDirectoryInput(message); }
            }
        }

        public static void GetIngesterConfigInput()
        {
            while (true)
            {
                var input = AppHelpers.GetFileInput("Input a config file path: ");
                if (input == null) { continue; }
                try
                {
                    System.Console.WriteLine("Not working");
                    break;
                }
                catch { }
            }
        }

        public static void GetOverseerConfigInput()
        {
            while (true)
            {
                var input = AppHelpers.GetFileInput("Input a config file path: ");
                if (input == null) { continue; }
                try
                {
                    System.Console.WriteLine("Not working");
                    break;
                }
                catch { }
            }
        }

        public static Guid GetGuidInput()
        {
            while (true)
            {
                System.Console.WriteLine("Please enter a valid GUID, or q/quit to exit.");
                var input = Console.ReadLine().Trim();
                if (input.ToLower() == "q" || input.ToLower() == "quit")
                { return Guid.Empty; }
                else
                {
                    try
                    {
                        var id = new Guid(g: input);
                        return id;
                    }
                    catch { }
                }
            }
        }
    }
}