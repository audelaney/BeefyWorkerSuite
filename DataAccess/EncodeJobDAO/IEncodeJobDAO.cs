#nullable enable
using DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
	public interface IEncodeJobDAO
	{
		/// <summary>
		/// Adds an encode job to the queue of pending encodes.
		/// </summary>
		bool AddEncodeJobToQueue(EncodeJob job);
		/// <summary>
		/// I don't exactly know what this is supposed to do
		/// </summary>
		bool RemoveEncodeJobFromQueue(Guid id);
		/// <summary>
		/// Gets the collection of all completed encode jobs from database
		/// </summary>
		IEnumerable<EncodeJob> RetrieveCompleteEncodeJobs();
		/// <summary>
		/// Gets the collection of all incompleted encode jobs from database
		/// </summary>
		IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs();
		/// <summary>
		/// Gets the collection of all incompleted encode jobs from database
		/// that match a certain priority
		/// </summary>
		IEnumerable<EncodeJob> RetrieveIncompleteEncodeJobs(int priority);
		/// <summary>
		/// Changes the passed in jobs status to "Checked out", implying that
		/// the job is currently in progress or undergoing interrim evaluation.
		/// </summary>
		bool MarkEncodeJobCheckedOut(Guid id, bool completed);
		/// <summary>
		/// Changes the passed in jobs status to "Checked out", implying that
		/// the job is currently in progress or undergoing interrim evaluation.
		/// </summary>
		bool MarkEncodeJobCheckedOut(EncodeJob job, bool completed);
		/// <summary>
		/// Marks a designated encode job as completed
		/// </summary>
		bool MarkJobCompletedStatus(Guid id, bool completed);
		/// <summary>
		/// Marks a designated encode job as completed
		/// </summary>
		bool MarkJobCompletedStatus(EncodeJob job, bool completed);
		/// <summary>
		/// Gets the matching encode job for specified GUID
		/// </summary>
		EncodeJob RetrieveEncodeJob(Guid id);
		/// <summary>
		/// Update/overwrite the data for a job
		/// </summary>
		/// <exception cref="System.Exception">Thrown when encountering an unknown and unhandled exception.</exception>
		bool UpdateJob(EncodeJob oldData, EncodeJob newData);
	}
}