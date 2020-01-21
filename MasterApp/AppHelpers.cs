using System;
using System.IO;
using AppLogic;

namespace MasterApp
{
    public static class AppHelpers
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

        public static void GetConfig()
        {
            while (true)
            {
                var input = AppHelpers.GetFileInput("Input a config file path: ");
                if (input == null) { continue; }
                try
                {
                    AppConfigManager.SetConfig(input);
                    var testing = AppConfigManager.Instance.InputBucketPath;
                    var testingAgain = AppConfigManager.Instance.DBTypeAndString;
                    break;
                }
                catch { }
            }
        }
    }
}