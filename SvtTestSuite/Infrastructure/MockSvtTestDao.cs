#nullable enable
using System;
using System.Linq;
using System.Collections.Generic;
using SvtTestSuite.Models;

namespace SvtTestSuite.Infrastructure
{
	/// <summary>
	/// Mock class, dummy returns
	/// </summary>
    public class MockSvtTestDao : ISvtTestDao
    {
		/// <summary></summary>
        public bool AddSuccessfulTest(SvtTestSuccess test)
        {
            System.Console.WriteLine(test.ToString());
            return true;
        }

		/// <summary></summary>
		public bool AddSuccessfulTests(IEnumerable<SvtTestSuccess> tests)
        {
            return AddSuccessfulTest(tests.First());
        }
    }
}