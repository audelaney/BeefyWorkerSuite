using System;

namespace DataObjects.TestSuite
{
    public class SvtParam
	{
		public string? ParamTitle { get; set; }
		public string? ParamCmd { get; set; }
		public int[]? ActiveValues { get; set; }
		public int[]? AllValues { get; set; }
	}
}