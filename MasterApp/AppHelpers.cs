using System;
using System.IO;

namespace MasterApp
{
    public static class AppHelpers
    {
        /// <summary>
        /// Outputs a message and gets a valid file from the user
        /// </summary>
        /// <returns>A valid file path, or null</returns>
        public static string? GetFileInput(string message)
        {
            System.Console.Write(message);
            string file = System.Console.ReadLine();
            if (File.Exists(file))
            { return file; }
            else
            { return null; }
        }

        public static string? GetDirectoryInput(string message)
        {
            System.Console.Write(message);
            string directory = System.Console.ReadLine();
            if (Directory.Exists(directory))
            { return directory; }
            else
            { return null; }
        }
    }
}