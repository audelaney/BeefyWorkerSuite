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
        public static void Run()
        {
            // Get the video you want to make the combined source for
            var input = AppHelpers.GetFileInput("Please input the path for the source video");
            var outputFileName = AppHelpers.GetFileInput("Please input the path for the output video");
            if (null == input || null == outputFileName)
            {
                System.Console.WriteLine("Invalid input");
                return;
            }

            string originalVideo = input;

            // Get all related encode jobs for the video
            IEnumerable<EncodeJob> jobs = EncodeJobManager.Instance.GetJobsByVideoName(input);

            // Shoot them to the encode manager
            if (jobs.Count() > 0)
            { EncoderManager.Instance.Combine(jobs.ToArray(), outputFileName); }
        }
    }
}