#nullable enable
using DataObjects.TestSuite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.TestSuite
{
	public interface ISvtTestDao
	{
		bool AddSuccessfulTest(SvtTestSuccess test);
		bool AddSuccessfulTests(IEnumerable<SvtTestSuccess> tests);
	}
}