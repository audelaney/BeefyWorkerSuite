#nullable enable
using System;
using System.Linq;
using System.Collections.Generic;
using DataObjects.TestSuite;

namespace DataAccess.TestSuite
{
    public class MockSvtTestDao : ISvtTestDao
    {
        public bool AddSuccessfulTest(SvtTestSuccess test)
        {
            System.Console.WriteLine(test.ToString());
            return true;
        }

        public bool AddSuccessfulTests(IEnumerable<SvtTestSuccess> tests)
        {
            return AddSuccessfulTest(tests.First());
        }
    }
}