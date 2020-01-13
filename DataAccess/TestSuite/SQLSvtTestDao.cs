#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using DataObjects.TestSuite;

namespace DataAccess.TestSuite
{
    public class SQLSvtTestDao : ISvtTestDao
    {
        private readonly string _connString;

        public SQLSvtTestDao(string connString)
        {
            _connString = connString;
        }

        public bool AddSuccessfulTest(SvtTestSuccess test)
        {
            throw new NotImplementedException();
        }

        public bool AddSuccessfulTests(IEnumerable<SvtTestSuccess> tests)
        {
            throw new NotImplementedException();
        }
    }
}