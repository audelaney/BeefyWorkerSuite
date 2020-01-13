using System;

namespace DataObjects
{
    public class FrameRate
    {
        public byte FPS { get; set; }
        public uint FrameRateNumerator {get;set;}
        public uint FrameRateDenominator {get;set;}
        public bool IsValid{get;}
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}