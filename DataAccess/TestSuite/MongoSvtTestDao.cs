using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects.TestSuite;

namespace DataAccess.TestSuite
{
	public class MongoSvtTestDao : ISvtTestDao
	{
		private static readonly string _dbName = "SvtTest";
		private static readonly string _successCollectionName = "";
		public bool AddSuccessfulTest(SvtTestSuccess test)
		{
			throw new NotImplementedException();
		}

		public bool AddSuccessfulTests(IEnumerable<SvtTestSuccess> tests)
		{
			var client = new MongoClient();
			var db = client.GetDatabase(_dbName);
			var coll = db.GetCollection<SvtTestSuccess>(_successCollectionName);
			coll.InsertMany(tests);

			throw new NotImplementedException();
		}
	}
}
