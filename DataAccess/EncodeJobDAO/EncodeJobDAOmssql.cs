using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataObjects;

namespace DataAccess
{
	/// <summary></summary>
	public class EncodeJobDAOmssql : IEncodeJobDAO
	{
		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
        public bool RemoveEncodeJobFromQueue(Guid id)
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveCompleteEncodeJobs()
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs()
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs(int priority)
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public bool MarkEncodeJobCheckedOut(Guid id, bool completed)
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public bool MarkEncodeJobCheckedOut(EncodeJob job, bool completed)
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public bool MarkJobCompletedStatus(Guid id, bool completed)
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public bool MarkJobCompletedStatus(EncodeJob job, bool completed)
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public EncodeJob RetrieveEncodeJob(Guid id)
        {
            throw new NotImplementedException();
        }

		/// <summary></summary>
        public bool UpdateJob(EncodeJob oldData, EncodeJob newData)
        {
            throw new NotImplementedException();
        }
    }
}