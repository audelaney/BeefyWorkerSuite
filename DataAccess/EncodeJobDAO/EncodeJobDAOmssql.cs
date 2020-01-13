using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataObjects;

namespace DataAccess
{
	public class EncodeJobDAOmssql : IEncodeJobDAO
	{
		public EncodeJobDAOmssql(string dBConnectionString)
		{
			sqlConnectionString = dBConnectionString;
		}

		private readonly string sqlConnectionString;
		private SqlConnection DBConnection
		{
			get
			{
				var conn = new SqlConnection(sqlConnectionString);

				return conn;
			}
		}

		public bool AddEncodeJobToQueue(EncodeJob job)
		{
			bool result = false;

			var conn = DBConnection;
			string cmdText = @"";
			var cmd = new SqlCommand(cmdText, conn)
			{
				CommandType = CommandType.StoredProcedure
			};

			cmd.Parameters.AddWithValue("",job.AdditionalCommandArguments);
			cmd.Parameters.AddWithValue("",job.ConfigFilePath);
			cmd.Parameters.AddWithValue("",job.MaxAttempts);
			cmd.Parameters.AddWithValue("",job.MinPsnr);
			cmd.Parameters.AddWithValue("",job.Priority);
			cmd.Parameters.AddWithValue("",job.VideoFileName);

			try
			{
				conn.Open();
				result = (1 == cmd.ExecuteNonQuery());
			}
			catch (SqlException sqlex)
			{
				throw sqlex;
			}
			finally
			{
				conn.Close();
				cmd.Dispose();
			}

			return result;
		}

        public bool RemoveEncodeJobFromQueue(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EncodeJob> RetrieveCompleteEncodeJobs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs(int priority)
        {
            throw new NotImplementedException();
        }

        public bool MarkEncodeJobCheckedOut(Guid id, bool completed)
        {
            throw new NotImplementedException();
        }

        public bool MarkEncodeJobCheckedOut(EncodeJob job, bool completed)
        {
            throw new NotImplementedException();
        }

        public bool MarkJobCompletedStatus(Guid id, bool completed)
        {
            throw new NotImplementedException();
        }

        public bool MarkJobCompletedStatus(EncodeJob job, bool completed)
        {
            throw new NotImplementedException();
        }

        public EncodeJob RetrieveEncodeJob(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateJob(EncodeJob oldData, EncodeJob newData)
        {
            throw new NotImplementedException();
        }
    }
}