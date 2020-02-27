#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SvtTestSuite.Models;

namespace SvtTestSuite.Infrastructure
{
    /// <summary>
    /// For use in parsing files output by the SVT-AV1 sample application.
    /// </summary>
    public static class SvtEncAppAccessor
    {
        private static readonly int MIN_REQUIRED_FILE_LINES = 20;
        private static readonly Regex FirstLineNewEncode = new Regex("Input #0,.*, from '.*':|Starting\\.\\.\\..*$");
        private static readonly Regex InputLineFfmpeg = new Regex("Input #0,.*clip-");

		/// <summary>
		/// Gets all successful runs from an output file composed of a capture of SVT encoder output
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
        public static IEnumerable<SvtTestSuccess> GetSuccessulTestsFromOutputFile(string file)
        {
            if (!File.Exists(file))
            {
                Exception throwing = new ApplicationException("File specified to be searched for Svt test results does not exist.");
                throwing.Data["path"] = file;
                throw throwing;
            }

            List<SvtTestSuccess> results = new List<SvtTestSuccess>();

            StreamReader? streamReader = null;
            try
            {
                streamReader = new StreamReader(file);

                Queue<string> fileLines = new Queue<string>(streamReader.ReadToEnd().Split('\n').Select(l => l.Trim()));
                if (fileLines.Count < MIN_REQUIRED_FILE_LINES)
                {
                    var up = new ApplicationException("File does not contain enough lines. Minimum is " + MIN_REQUIRED_FILE_LINES);
                    up.Data["path"] = file;
                    throw up;
                }

                //Use the line that's just "SVT-AV1 Encoder" as your signal that a test was run
                int numTests = fileLines.Count(l => l == "SVT-AV1 Encoder");

                for (int i = 0; i < numTests; i++)
                {
                    //In case there is a header, go ahead and dequeue until you hit a first line match
                    while (!fileLines.Peek().Contains("Starting..."))
                    { _ = fileLines.Dequeue(); }
                    List<string> lines = new List<string>
                    {
                        fileLines.Dequeue()
                    };
                    while (fileLines.Count > 0 && !fileLines.Peek().Contains("Starting..."))
                    { lines.Add(fileLines.Dequeue()); }
                    try
                    {
                        SvtTestSuccess? parsedTest = ParseOutputLinesToTest(lines);
                        if (parsedTest is SvtTestSuccess) { results.Add((parsedTest as SvtTestSuccess)!); }
                    }
                    catch (FormatException fe)
                    {
                        //lines.ForEach(l => System.Console.WriteLine(l));
                        throw fe;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (ApplicationException up)
            {
                throw up;
            }
            catch (Exception ex)
            {
                var up = new ApplicationException("Unknown error occurred while reading test output from file.", ex);
                up.Data["path"] = file;
                throw up;
            }
            finally
            {
                streamReader?.Close();
            }

            return results;
        }

        private static SvtTestSuccess? ParseOutputLinesToTest(List<string> lines)
        {
            SvtTestSuccess? output = null;
            Regex summaryLineRegex = new Regex("SUMMARY [-]* Channel \\d [-]*");

            try
            {
                //If there is a summary line, we probably have a successful test
                string? summaryLine = lines.Find(l => summaryLineRegex.IsMatch(l));
                if (summaryLine != null)
                {
                    int summaryLineStart = lines.IndexOf(summaryLine);
                    IEnumerable<string> summaryLines = lines.GetRange(summaryLineStart, lines.Count - summaryLineStart);
                    var statLine2 = summaryLines.ElementAt(6).ParseNoWhiteSpace();
                    output = new SvtTestSuccess();
                    try
                    {
                        output.ClipNumber = int.Parse(Regex.Replace(lines[1], InputLineFfmpeg.ToString(), "").Replace(".mkv':", ""));
                    }
                    catch
                    {
                        System.Console.WriteLine(("Cannot parse clip number from line: " + lines.ToArray()[1] + "| parse input: " +
                                                            Regex.Replace(lines[1], InputLineFfmpeg.ToString(), "").Replace(".mkv':", "")));
                    }
                    //Sometimes, I move the videos and logs into a different directory.
                    string possibleOutputPath = "";
                    try
                    {
                        possibleOutputPath = lines.Last(l => l.Contains("File finished: ")).ParseNoWhiteSpace()[2];

                        if (!File.Exists(possibleOutputPath))
                        {
                            System.Console.WriteLine(possibleOutputPath + " does not exist. Searching manually for real output path...");
                            var directories = Directory.GetDirectories(Path.GetDirectoryName(possibleOutputPath));
                            if (directories.Length == 0)
                            {
                                output.OutputPath = possibleOutputPath;
                            }
                            else
                            {
                                string newPossiblePath = Path.Combine(Path.GetDirectoryName(possibleOutputPath)!, "clip", Path.GetFileName(possibleOutputPath));
                                if (File.Exists(newPossiblePath))
                                {
                                    output.OutputPath = newPossiblePath;
                                }
                                else
                                {
                                    output.OutputPath = possibleOutputPath;
                                }
                            }
                        }
                        else
                        {
                            output.OutputPath = possibleOutputPath;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception occurred setting output path.");
                        System.Console.WriteLine(ex.ToString());
                        System.Console.WriteLine(ex.StackTrace?.ToString());
                        output.OutputPath = output.OutputPath ?? possibleOutputPath;
                    }
                    try
                    {
                        output.FileSize = uint.Parse(summaryLines.ElementAt(2).ParseNoWhiteSpace()[3]);
                        output.Bitrate = double.Parse(summaryLines.ElementAt(2).ParseNoWhiteSpace()[4]);
                        output.AvgQP = double.Parse(statLine2[0]);
                        output.AvgYPSNR = double.Parse(summaryLines.ElementAt(6).ParseNoWhiteSpace()[1]);
                        output.OverallYPSNR = double.Parse(statLine2[5]);
                        output.AvgSpeed = double.Parse(summaryLines.ElementAt(10).ParseNoWhiteSpace()[2]);
                        output.EncodingTime = TimeSpan.FromMilliseconds(double.Parse(summaryLines.ElementAt(11).ParseNoWhiteSpace()[3]));
                        output.VMAF = double.Parse(summaryLines.First(l => l.Contains("VMAF score = ")).Replace("VMAF score = ", ""));
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("=============");
                        summaryLines.ToList().ForEach(l => System.Console.WriteLine(l));
                        System.Console.WriteLine("=============");
                        System.Console.WriteLine(summaryLines.ElementAt(2).ParseNoWhiteSpace()[3]);
                        System.Console.WriteLine(summaryLines.ElementAt(6).ParseNoWhiteSpace()[1]);
                        System.Console.WriteLine(summaryLines.ElementAt(10).ParseNoWhiteSpace()[2]);
                        System.Console.WriteLine(summaryLines.ElementAt(11).ParseNoWhiteSpace()[3]);
                        System.Console.WriteLine("=============");
                        throw ex;
                    }
                    output.AdditionalCommands = summaryLines.Last(l => l.Contains("Cmmd finished: ")).Replace("Cmmd finished: ", "");

                    if (File.Exists(output.OutputPath))
                    {
                        FileInfo fileInfo = new FileInfo(output.OutputPath);
                        if (fileInfo.Directory.Name == "clip")
                        {
                            string masterTestDirectory = Path.GetDirectoryName(fileInfo.DirectoryName)!;
                            string realLogPath = Path.Combine(masterTestDirectory, "logs", Path.GetFileName(output.OutputPath)) + ".log";
                            output.FrameLogs = GetPictureStatsFromStatFile(realLogPath);
                        }
                        else
                        {
                            output.FrameLogs = GetPictureStatsFromStatFile(output.OutputPath + ".log");
                        }
                    }
                }
            }
            catch (FormatException fe)
            {
                System.Console.WriteLine(fe.StackTrace);
                throw fe;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }

		/// <summary>
		/// Gets the stats for each encoded frame in a video output by the SVT logger
		/// </summary>
		/// <param name="statFilePath">Path to the file</param>
		/// <returns>Collection of stats for each frame, or empty if none are found.</returns>
        public static EncodedPictureStat[] GetPictureStatsFromStatFile(string statFilePath)
        {
            if (string.IsNullOrWhiteSpace(statFilePath))
            { throw new ArgumentException("Invalid path."); }
            if (!File.Exists(statFilePath))
            { throw new ArgumentException("Path does not lead to existing file."); }

            EncodedPictureStat[] output = new EncodedPictureStat[0];

            using (StreamReader reader = new StreamReader(statFilePath))
            {
                //Sample line
                //Picture Number: 2065	 QP:   29  [ PSNR-Y: 51.04 dB,	PSNR-U: 56.16 dB,	PSNR-V: 57.76 dB,	MSE-Y: 0.51,	MSE-U: 0.16,	MSE-V: 0.11 ]	   1561 bits
                //Get only the lines that match this
                string pattern = @"Picture Number: \d+\t QP:   \d+(.*)PSNR-Y: \d+\.\d+ dB,\tPSNR-U: \d+\.\d+ dB,\tPSNR-V: \d+\.\d+ dB,\tMSE-Y: \d+\.\d+,\tMSE-U: \d+\.\d+,\tMSE-V: \d+\.\d+(.*)\d+ bits";
                //Still debugging this pattern...
                pattern = @"Picture Number:(.*)";
                Regex picLineRgx = new Regex(pattern);
                string[] lines = reader.ReadToEnd().Split('\n').Where(l => picLineRgx.IsMatch(l)).ToArray();

                //After getting the matches, sort through them and get the stats, adding to list when successful
                List<EncodedPictureStat> collectedStats = new List<EncodedPictureStat>();
                foreach (string line in lines)
                {
                    try
                    {
                        string[] pieces = line.ParseNoWhiteSpace();
                        collectedStats.Add(new EncodedPictureStat
                        {
                            PictureNumber = uint.Parse(pieces[2]),
                            Qp = byte.Parse(pieces[4]),
                            YPsnr = double.Parse(pieces[7]),
                            Bits = uint.Parse(pieces[22])
                        });
                    }
                    catch (Exception)
                    {
                        System.Console.WriteLine("==============");
                        System.Console.WriteLine("Failed to add: " + line);
                        try
                        {
                            int itemNum = -1;
                            foreach (string item in line.ParseNoWhiteSpace())
                            {
                                System.Console.Write(itemNum++ + ": ");
                                System.Console.WriteLine(item);
                            }
                        }
                        catch { }
                    }
                }

                output = collectedStats.ToArray();
            }

            return output;
        }
        private static string[] ParseNoWhiteSpace(this string input)
        {
            input = input.Replace('\t', ' ');
            while (input.Contains("  ") || input.Contains('\t'))
            {
                input = input.Replace("  ", " ");
            }
            var output = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return output;
        }
    }
}