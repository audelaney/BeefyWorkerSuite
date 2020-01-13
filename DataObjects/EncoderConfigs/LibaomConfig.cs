using System;

namespace DataObjects
{
    public class LibaomConfig
    {
        public LibaomConfig()
        {
            TargetBitrateKbps = 1500;
            CRF = 33;
        }
        public int GopMaxSize {get;set;}
        public int GopMinSize { get; set; }
        public byte SpeedSetting { get; set; }
        public int ArnrMaxFrames { get; set; }
        public int ReferencesFrames { get; set; }
        public int LagInFrames { get; set; }
        public int AutoAltRef { get; set; }
        public byte TileColumns { get; set; }
        public byte TileRows { get; set; }
        public uint MaxBitrateKbps { get; set; }
        public bool RowMultithread { get; set; }
        public uint TargetBitrateKbps { get; set; }
        public uint CRF { get; set; }
        public FrameRate FPS { get; set; }
        public string ToAomencCommandLine()
        {
            throw new NotImplementedException();
        }
        public string ToFfmpegCommandLine()
        {
            throw new NotImplementedException();
        }
    }
}