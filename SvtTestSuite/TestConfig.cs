using System;
using System.IO;
using System.Collections.Generic;
using DataObjects.TestSuite;

namespace SvtTestSuite
{
    public static class TestConfig
    {
        public readonly static string vmafFfmpegLocation = Path.Combine(@"ffmpeg");
        public readonly static string encodeScriptLocation = Path.Combine(@"/home/userbeef/server-files/encode-items/svt-encode-hi.sh");
        public readonly static string svtTestExecutionDirectory = Path.Combine(@"/home/userbeef/anime/svt-" + DateTime.Now.ToShortDateString().Replace('/', '-') + "-tests");
        private static string _clipName = "clip";
        public static string ClipName
        {
            get
            {
                return _clipName;
            }
            set
            {
                _clipNum = 0;
                _clipName = value;
            }
        }
        private static int _clipNum = 0;
        public static readonly SvtParam[] svtParams =
        {
            new SvtParam{ParamTitle = "IntraPeriod", ParamCmd = "-intra-period", ActiveValues = new[]{ -2 , 23, 47, 71, 119, 239 }, AllValues = new[]{ -2, 23, 47, 71, 119, 239 }}
            ,new SvtParam{ParamTitle = "EncoderMode", ParamCmd = "-enc-mode", ActiveValues = new[]{ 3,4,5,6 }, AllValues = new[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8 }}
            ,new SvtParam{ParamTitle = "RateControlMode", ParamCmd = "-rc", ActiveValues = new[]{ 1 }, AllValues = new[]{ 0, 1, 2 }}
            ,new SvtParam{ParamTitle = "QP", ParamCmd = "-q", ActiveValues = new[]{ 15 }, AllValues = new[]{ 5, 15, 20, 25, 35, 50 }}
            ,new SvtParam{ParamTitle = "ScreenContentMode", ParamCmd = "-scm", ActiveValues = new[]{ 0 }, AllValues = new[]{ 0, 1, 2 }}
            ,new SvtParam{ParamTitle = "AltRefStrength", ParamCmd = "-altref-strength", ActiveValues = new[]{ 6 }, AllValues = new[]{ 0, 1, 2, 3, 4, 5, 6 }}
            ,new SvtParam{ParamTitle = "AltRefNFrames", ParamCmd = "-altref-nframes", ActiveValues = new[]{ 2 }, AllValues = new[]{ 2, 4, 6, 8, 10 }}
            ,new SvtParam{ParamTitle = "HierarchicalLevels", ParamCmd = "-hierarchical-levels", ActiveValues = new[]{ 3 }, AllValues = new[]{ 3, 4 }}
            ,new SvtParam{ParamTitle = "AdaptiveQuantization", ParamCmd = "-adaptive-quantization", ActiveValues = new[]{ 0 }, AllValues = new[]{ 0, 1, 2 }}
            ,new SvtParam{ParamTitle = "TargetBitRate", ParamCmd = "-tbr", ActiveValues = new[]{ 1000, 1250, 1500 }, AllValues = new[]{ 500,750,1000,1250,1500,1750,2000 }}
        };
        public static string OutputFile
        {
            get
            {
                _clipNum++;
                return Path.Combine(svtTestExecutionDirectory, _clipName + "." + _clipNum + ".av1.mkv");
            }
        }
    }
}