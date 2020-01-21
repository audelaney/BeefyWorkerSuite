#nullable enable
using System;
using System.IO;
using System.Collections.Generic;
using DataObjects.TestSuite;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace DataAccess.TestSuite
{
    /// <summary>
    /// Mostly for writing test cases to a csv file.
    /// </summary>
    public class CsvSvtTestDao : ISvtTestDao
    {
        private readonly string _csvFilePath;
        /// <summary></summary>
        public CsvSvtTestDao(string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath)) { throw new ArgumentException("Invalid path for CSV."); }
            _csvFilePath = filePath;
        }
        /// <summary></summary>
        public bool AddSuccessfulTest(SvtTestSuccess test)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a list of tests to the local csvFilePath, does not append. Returns false if there is an issue with the
        /// path. Tests are written in blocks based on their ParamsInCmmd property
        /// </summary>
        public bool AddSuccessfulTests(IEnumerable<SvtTestSuccess> tests)
        {
            bool result = true;
            try
            {
                Regex standardCommandRgx =
                    new Regex(@"-enc-mode \d -intra-period .?\d+ -rc \d -q \d+ -scm \d+ -altref-strength \d+ -altref-nframes \d+ -hierarchical-levels \d -adaptive-quantization \d");

                var firstBatch = tests.Where(t => standardCommandRgx.IsMatch(t.AdditionalCommands));

                StringBuilder writeOutToFile = new StringBuilder();

                var firstBatchParams = firstBatch.First().ParamsInCmmd;
                for (int i = 0; i < firstBatchParams.Length; i++)
                {
                    if (0 != i) { writeOutToFile.Append(","); }
                    writeOutToFile.Append(firstBatchParams[i]);
                }
                writeOutToFile.Append(",Average speed");
                writeOutToFile.Append(",Average PSNR-Y");
                writeOutToFile.Append(",Overall PSNR-Y");
                writeOutToFile.Append(",VMAF");
                writeOutToFile.Append(",File size");
                writeOutToFile.Append(",Encoding time");
                writeOutToFile.Append(",Bitrate");
                writeOutToFile.Append(",Average QP");
                writeOutToFile.Append(",File name");

                writeOutToFile.AppendLine();

                foreach (SvtTestSuccess item in firstBatch)
                {
                    for (int i = 0; i < firstBatchParams.Length; i++)
                    {
                        if (0 != i) { writeOutToFile.Append(","); }
                        writeOutToFile.Append(item.ParamsAndArgsInCommand[firstBatchParams[i]]);
                    }
                    writeOutToFile.Append("," + item.AvgSpeed);
                    writeOutToFile.Append("," + item.AvgYPSNR);
                    writeOutToFile.Append("," + item.OverallYPSNR);
                    writeOutToFile.Append("," + item.VMAF);
                    writeOutToFile.Append("," + item.FileSize);
                    writeOutToFile.Append("," + item.EncodingTime);
                    writeOutToFile.Append("," + item.Bitrate);
                    writeOutToFile.Append("," + item.AvgQP);
                    writeOutToFile.Append("," + Path.GetFileName(item.OutputPath));

                    writeOutToFile.AppendLine();
                }

                WriteDataToFile(writeOutToFile.ToString());
            }
            catch { result = false; }

            return result;
        }

        private void WriteDataToFile(string data, bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(_csvFilePath, append))
            {
                sw.Write(data);
                sw.Close();
            }
        }
    }
}