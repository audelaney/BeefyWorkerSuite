using System;
using System.Collections.Generic;
using System.Text;

namespace SvtTestSuite.Models
{
	public class EncodedPictureStat
	{
		public uint PictureNumber { get; set; }
		public byte Qp { get; set; }
		public double YPsnr { get; set; }
		public double YMse { get; set; }
		public uint Bits { get; set; }

		public override string ToString()
		{
			string output = "Picture num: " + PictureNumber;
			output += ", QP: " + Qp;
			output += ", YPsnr: " + YPsnr;
			output += ", YMse: " + YMse;
			output += ", Bits: " + Bits;

			return output;
		}
	}
}