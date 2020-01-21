#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataObjects.TestSuite;
using DataAccess;
using DataAccess.TestSuite;

namespace SvtTestSuite
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Working");
                return;
            }
            else if (args[0] == "test")
            {
                Console.WriteLine("Execution directory: " + TestConfig.svtTestExecutionDirectory);
                Console.WriteLine("Encode script location: " + TestConfig.encodeScriptLocation);
                Console.WriteLine(Builder.CommandLine("", "/home/user/somefile", "/home/user/someotherfile"));
                Console.WriteLine("Looking for the anime folder...");
                string[] animeFolderContents = Directory.GetDirectories(@"/home/userbeef/anime/");
                foreach (var item in animeFolderContents)
                {
                    Console.WriteLine(item);
                }
                int plannedTests = TestConfig.svtParams.First(p => p.ParamTitle == "IntraPeriod").ActiveValues.Length *
                                    TestConfig.svtParams.First(p => p.ParamTitle == "EncoderMode").ActiveValues.Length *
                                    TestConfig.svtParams.First(p => p.ParamTitle == "RateControlMode").ActiveValues.Length *
                                    TestConfig.svtParams.First(p => p.ParamTitle == "QP").ActiveValues.Length *
                                    TestConfig.svtParams.First(p => p.ParamTitle == "ScreenContentMode").ActiveValues.Length *
                                    TestConfig.svtParams.First(p => p.ParamTitle == "AltRefStrength").ActiveValues.Length *
                                    TestConfig.svtParams.First(p => p.ParamTitle == "AltRefNFrames").ActiveValues.Length *
                                    TestConfig.svtParams.First(p => p.ParamTitle == "HierarchicalLevels").ActiveValues.Length *
                                    TestConfig.svtParams.First(p => p.ParamTitle == "AdaptiveQuantization").ActiveValues.Length;
                Console.WriteLine("Calculated planned test executions: " + plannedTests);
                if (args.Length == 2 && File.Exists(args[1]))
                {
                    _ = Builder.BuildTestBashFile(args[1]);
                }
            }
            else if (args[0] == "build")
            {
                string inputFile = args[1];
                string[] inputFiles = args.Skip(1).ToArray();
                List<string> testLines = new List<string>();
                foreach(string file in inputFiles)
                {
                    try{
                        testLines = testLines.Concat(Builder.BuildTestBashFile(file)).ToList();
                    } catch {}
                }
                Builder.WriteTestBashFile(testLines);
            }
            else if (args[0] == "analyze")
            {
                try
                {
                    if (args.Length < 3)
                    { System.Console.WriteLine("Not enough args"); return; }
                    string fileToAnalyze = args[1];
                    string dataStoreOutput = args[2];
                    string connString = args.Length > 3 ? args[3] : ""; //filename for csv

                    IEnumerable<SvtTestSuccess> successes = SvtEncAppAccessor.GetSuccessulTestsFromOutputFile(fileToAnalyze);

                    if (0 == successes.Count()) { System.Console.WriteLine("No successes"); return; }

                    ISvtTestDao dao = dataStoreOutput.ToLower() switch
                    {
                        "mock" => new MockSvtTestDao(),
                        "csv" => new CsvSvtTestDao(connString),
                        _ => throw new InvalidOperationException()
                    };

                    bool result = dao.AddSuccessfulTests(successes);

                    if (result)
                    {
                        System.Console.WriteLine(successes.Count() + " successful tests added.");
                    }
                    else
                    {
                        System.Console.WriteLine("Error while adding success to database");
                    }
                }
                catch (ApplicationException apex)
                {
                    System.Console.WriteLine(apex.ToString());
                    System.Console.WriteLine(apex.StackTrace);
                    System.Console.WriteLine(apex.InnerException?.ToString());
                    System.Console.WriteLine(apex.InnerException?.StackTrace);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                    System.Console.WriteLine(ex.StackTrace?.ToString());
                }

                return;
            }
        }
    }
}