#nullable enable
using SvtTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SvtTestSuite.Infrastructure
{
    /// <summary>Data store operations for <see cref="DataObjects.TestSuite.SvtTestSuccess" /></summary>
    public interface ISvtTestDao
    {
        /// <summary>Adds a single test to in use datastore</summary>
		bool AddSuccessfulTest(SvtTestSuccess test);
        /// <summary>Adds a collection of successful tests to in use datastore</summary>
		bool AddSuccessfulTests(IEnumerable<SvtTestSuccess> tests);
    }
}