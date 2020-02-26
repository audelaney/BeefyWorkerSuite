#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SvtTestSuite.Models
{
    public class SvtTestSuccess : ISvtTest
    {
        public int TestId { get; set; }
        public int ClipNumber { get; set; }
        public DateTime TestDate { get; set; }
        public ulong FileSize { get; set; }
        public double AvgYPSNR { get; set; }
        public double OverallYPSNR { get; set; }
        public TimeSpan EncodingTime { get; set; }
        public double AvgSpeed { get; set; }
        public double Bitrate { get; set; }
        public double AvgQP { get; set; }
        public string? ConfigFilePath { get; set; }
        public string? AdditionalCommands { get; set; }
        public string? Remarks { get; set; }
        public string? OutputPath { get; set; } = "";
        public double VMAF { get; set; }
#nullable disable //This prop absolutely ruins intellisense when Nullable enabled...
        public EncodedPictureStat[] FrameLogs { get; set; }
#nullable enable
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append("Test ID: " + TestId);
            output.AppendLine();
            output.Append("Clip number: " + ClipNumber);
            output.AppendLine();
            output.Append("Test Date: " + TestDate);
            output.AppendLine();
            output.Append("File size: " + FileSize);
            output.AppendLine();
            output.Append("Average y psnr: " + AvgYPSNR);
            output.AppendLine();
            output.Append("Overall y psnr: " + OverallYPSNR);
            output.AppendLine();
            output.Append("Encoding time: " + EncodingTime);
            output.AppendLine();
            output.Append("Encoding time: " + Bitrate);
            output.AppendLine();
            output.Append("Average speed: " + AvgSpeed);
            output.AppendLine();
            output.Append("Config file path: " + ConfigFilePath);
            output.AppendLine();
            output.Append("Additional commands: " + AdditionalCommands);
            output.AppendLine();
            output.Append("Remarks: " + Remarks);
            output.AppendLine();
            output.Append("Output path: " + OutputPath);
            output.AppendLine();
            if (FrameLogs == null || FrameLogs.Length == 0)
            {
                output.Append("No frame logs to output.");
                output.AppendLine();
            }
            else
            {
                output.Append("Frame log count: " + FrameLogs.Length);
                output.AppendLine();

                output.Append("Outputting: ");
                output.AppendLine();

                foreach (EncodedPictureStat pic in FrameLogs)
                {
                    output.Append(pic.ToString());
                    output.AppendLine();
                }
            }
            return output.ToString();
        }

        private Dictionary<string, int> _paramsAndArgsInAddtlCommands = new Dictionary<string, int>();
        public string[] ParamsInCmmd
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AdditionalCommands))
                { System.Console.WriteLine("Additional commands is empty"); return new string[0]; }

                if (_paramsAndArgsInAddtlCommands.Count() == 0)
                { _paramsAndArgsInAddtlCommands = GetParamsAndArgsFromCommand(AdditionalCommands); }

                return _paramsAndArgsInAddtlCommands.Select(p => p.Key).ToArray();
            }
        }

        public Dictionary<string, int> ParamsAndArgsInCommand
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AdditionalCommands))
                { return new Dictionary<string, int>(); }

                if (!(AdditionalCommands.Split(' ').Length / 2 == _paramsAndArgsInAddtlCommands.Count))
                { _paramsAndArgsInAddtlCommands = GetParamsAndArgsFromCommand(AdditionalCommands); }

                return _paramsAndArgsInAddtlCommands;
            }
        }

        private static Dictionary<string, int> GetParamsAndArgsFromCommand(string command)
        {
            var cmdArray = command.Split(' ');
            Dictionary<string, int> output = new Dictionary<string, int>();
            for (int i = 0; i < cmdArray.Length; i += 2)
            {
                if (int.TryParse(s: cmdArray[i + 1].Replace(" ", ""), result: out int arg))
                { output[cmdArray[i]] = arg; }
            }
            return output;
        }
    }
}