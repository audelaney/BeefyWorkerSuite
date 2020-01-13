using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataObjects.TestSuite;

namespace SvtTestSuite
{
    public class Builder
    {

        public static List<string> BuildTestBashFile(string inputFile)
        {
            List<string> lines = new List<string>();

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("File doesnt exist");
                return lines;
            }

            TestConfig.ClipName = Path.GetFileNameWithoutExtension(inputFile);

            try
            {
                int testsBuilt = 0;

                foreach (int encMode in TestConfig.svtParams.First(p => p.ParamTitle == "EncoderMode").ActiveValues.Reverse())
                {
                    foreach (int idrDistance in TestConfig.svtParams.First(p => p.ParamTitle == "IntraPeriod").ActiveValues)
                    {
                        foreach (int rateControl in TestConfig.svtParams.First(p => p.ParamTitle == "RateControlMode").ActiveValues)
                        {
                            foreach (int qp in TestConfig.svtParams.First(p => p.ParamTitle == "QP").ActiveValues)
                            {
                                foreach (int screenContent in TestConfig.svtParams.First(p => p.ParamTitle == "ScreenContentMode").ActiveValues)
                                {
                                    foreach (int altRefStrength in TestConfig.svtParams.First(p => p.ParamTitle == "AltRefStrength").ActiveValues)
                                    {
                                        foreach (int altRefCount in TestConfig.svtParams.First(p => p.ParamTitle == "AltRefNFrames").ActiveValues)
                                        {
                                            foreach (int hierarchicalLevel in TestConfig.svtParams.First(p => p.ParamTitle == "HierarchicalLevels").ActiveValues)
                                            {
                                                foreach (int adaptiveQuantization in TestConfig.svtParams.First(p => p.ParamTitle == "AdaptiveQuantization").ActiveValues)
                                                {
                                                    foreach (int bitRate in TestConfig.svtParams.First(p => p.ParamTitle == "TargetBitRate").ActiveValues)
                                                    {
                                                        string command = BuildCommand(encMode,
                                                                                        idrDistance,
                                                                                        rateControl,
                                                                                        qp,
                                                                                        screenContent,
                                                                                        altRefStrength,
                                                                                        altRefCount,
                                                                                        hierarchicalLevel,
                                                                                        adaptiveQuantization,
																						bitRate);

                                                        if (lines.Contains("echo Starting..." + command))
                                                        { continue; }
                                                        testsBuilt++;
                                                        string outputName = TestConfig.OutputFile;
                                                        lines.Add("echo Starting..." + command);
                                                        lines.Add(CommandLine(command, inputFile, outputName));
                                                        lines.Add(VmafLine(inputFile, outputName));
                                                        lines.Add("echo File finished: " + outputName);
                                                        lines.Add("echo Cmmd finished: " + command);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Tests built: " + testsBuilt);
            }
            catch
            {
                Console.WriteLine("Something went wrong while generating tests");
            }

            return lines;
        }

        private static string VmafLine(string inputFile, string outputName)
        {
            string output = TestConfig.vmafFfmpegLocation;

            output += " -r 23.976 -i " + inputFile;
            output += " -r 23.976 -i " + outputName;
            output += " -lavfi libvmaf -f null -";

            return output;
        }

        public static void WriteTestBashFile(List<string> lines)
        {
            string outputDirectory;
            try
            {
                //Make the directory to put the files in
                outputDirectory = TestConfig.svtTestExecutionDirectory;
                //if the directory already exists and has more than one file, quit with error message
                if (Directory.Exists(outputDirectory) && Directory.GetFiles(outputDirectory).Length > 1)
                {
                    Console.WriteLine("Directory exists and is populated by more than a command file.");
                    System.Console.Write("'Y' to wipe the folder and continue, anything else to quit:");
                    if (!(Console.ReadKey().KeyChar.ToString()?.ToLower() == "y"))
                    {
                        System.Console.WriteLine();
                        return;
                    }
                    else
                    {
                        System.Console.WriteLine("\nDeleting...");
                        Directory.Delete(outputDirectory, true);
                    }
                }
                Directory.CreateDirectory(outputDirectory);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                Console.WriteLine("Failed to make or find specified directory");

                return;
            }
            try
            {
                string commandFile = Path.Combine(outputDirectory, "command.sh");
                using StreamWriter writer = new StreamWriter(commandFile);
                foreach (var line in lines)
                {
                    writer.Write(line);
                    writer.WriteLine();
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong while writing tests to file.");
                return;
            }

            System.Console.WriteLine(lines.Count(l => l.Contains("Starting...")) + " tests written to command file in directory " + outputDirectory);
        }

        public static string CommandLine(string command, string inputFile, string outputFile)
        {
            return "sh " + TestConfig.encodeScriptLocation +
                    " " + inputFile +
                    " " + outputFile +
                    " \"" + command + "\"";
        }

        private static string BuildCommand(int encMode,
                                            int idrDistance,
                                            int rateControl,
                                            int qp,
                                            int screenContent,
                                            int altRefStrength,
                                            int altRefCount,
                                            int hierarchicalLevels,
                                            int adaptiveQuantization,
											int bitRate)
        {
            return "-enc-mode " + encMode +
				" -tbr " + bitRate +
                " -intra-period " + idrDistance +
                " -rc " + rateControl +
                " -q " + qp +
                " -scm " + screenContent +
                " -altref-strength " + altRefStrength +
                " -altref-nframes " + altRefCount +
                " -hierarchical-levels " + hierarchicalLevels +
                " -adaptive-quantization " + adaptiveQuantization;
        }

        private static string BuildCommand(List<KeyValuePair<SvtParam, int>> paramsAndArgs)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<SvtParam, int> pair in paramsAndArgs)
            {
                sb.Append(pair.Key.ParamCmd + " " + pair.Value.ToString() + " ");
            }

            return sb.ToString().TrimEnd();
        }
    }
}