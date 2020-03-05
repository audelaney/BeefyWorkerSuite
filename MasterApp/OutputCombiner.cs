#nullable enable
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using AppLogic;
using DataObjects;

namespace MasterApp
{
    public class OutputCombiner
    {
        public static void Run(string? originalVideo = null, string? outputFileName = null)
        {
            // Get the video you want to make the combined source for
            if (string.IsNullOrWhiteSpace(originalVideo))
            {
                System.Console.Write("Please input the path for the source video: ");
                originalVideo = Console.ReadLine().Trim();
            }
            if (string.IsNullOrWhiteSpace(outputFileName))
            {
                System.Console.Write("Please input the filename for the output video: ");
                outputFileName = Console.ReadLine().Trim();
            }
            if (null == originalVideo || null == outputFileName)
            {
                System.Console.WriteLine("Invalid input");
                return;
            }

            // Get all related encode jobs for the video
            IEnumerable<EncodeJob> jobs = EncodeJobManager.Instance.GetJobsByVideoName(originalVideo);

            // Shoot them to the encode manager
            if (jobs.Count() > 0)
            { EncoderManager.Instance.CombineSuccessfulEncodes(jobs.ToArray(), outputFileName); }
        }
    }
}