#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
	/// <summary>
	/// Class for managing the WorkQueue thread pool.
	/// </summary>
	/// <remarks>
	/// This class is used to keep track of the current types of WorkQueue entries being processed and 
	/// for requested new queue entries based on the current types being processed.
	/// </remarks>
	public class WorkQueueThreadPoolManager
	{
		#region Private Members
		private readonly object _syncLock = new object();
		private readonly int _highPriorityThreadLimit;
		private readonly int _memoryLimitedThreadLimit;

		private int _memoryLimitedCount = 0;
		private int _highPriorityCount = 0;
		private int _totalThreadCount = 0;
		private readonly Dictionary<WorkQueueTypeEnum,WorkQueueTypeEnum> _nonMemoryLimitedList = new Dictionary<WorkQueueTypeEnum, WorkQueueTypeEnum>();
		#endregion

		#region Contructors
		public WorkQueueThreadPoolManager(int highPriorityCount, int memoryLimitedThreadLimit, IList<WorkQueueTypeEnum> nonMemoryLimitedList )
		{
			_highPriorityThreadLimit = highPriorityCount;
			_memoryLimitedThreadLimit = memoryLimitedThreadLimit;
			foreach (WorkQueueTypeEnum type in nonMemoryLimitedList)
				_nonMemoryLimitedList.Add(type, type);
		}
		#endregion

		#region Private Methods
		private string GetWorkQueueTypeString()
		{
			string typeString = String.Empty;
			if (_nonMemoryLimitedList.Count > 0)
			{
				foreach (WorkQueueTypeEnum type in _nonMemoryLimitedList.Values)
				{
					if (typeString.Length > 0)
						typeString += "," + type.Enum;
					else
						typeString = type.Enum.ToString();
				}
			}
			return typeString;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Override.
		/// </summary>
		/// <returns>
		/// A string description of the thread pool with the number of queue items in use of various types.
		/// </returns>
		public override string ToString()
		{
			lock (_syncLock)
			{
				return
					String.Format("WorkQueueThreadPool: {0} high priority, {1} of {2} memory limited, {3} total threads in use",
					              _highPriorityCount, _memoryLimitedCount, _memoryLimitedThreadLimit,
					              _totalThreadCount);
			}
		}

		/// <summary>
		/// Method called when a <see cref="WorkQueue"/> item completes.
		/// </summary>
		/// <param name="queueItem">The queue item completing.</param>
		public void QueueItemComplete(Model.WorkQueue queueItem)
		{
			lock (_syncLock)
			{
				if (queueItem.WorkQueuePriorityEnum.Equals(WorkQueuePriorityEnum.High))
					_highPriorityCount--;

				if (!_nonMemoryLimitedList.ContainsKey(queueItem.WorkQueueTypeEnum))
					_memoryLimitedCount--;

				_totalThreadCount--;
			}
		}


		/// <summary>
		/// Method for getting next <see cref="WorkQueue"/> entry.
		/// </summary>
		/// <param name="processorId">The Id of the processor.</param>
		/// <remarks>
		/// </remarks>
		/// <returns>
		/// A <see cref="WorkQueue"/> entry if found, or else null;
		/// </returns>
		public Model.WorkQueue GetWorkQueueItem(string processorId)
		{
			bool bHighPriorityThreadsAvailable;
			bool bMemoryLimitedThreadsAvailable;

			lock (_syncLock)
			{
				bHighPriorityThreadsAvailable = _highPriorityCount < _highPriorityThreadLimit;
				bMemoryLimitedThreadsAvailable = _memoryLimitedCount < _memoryLimitedThreadLimit;
			}

			Model.WorkQueue queueListItem = null;

			// If we don't have the max high priority threads in use,
			// first see if there's any available
			if (bHighPriorityThreadsAvailable)
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
					parms.ProcessorID = processorId;
					parms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;

					queueListItem = select.FindOne(parms);

					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// If we didn't find a high priority work queue item, and we have threads 
			// available for memory limited work queue items, query for the next queue item available.
			if (queueListItem == null
			    && bMemoryLimitedThreadsAvailable)
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
					parms.ProcessorID = processorId;

					queueListItem = select.FindOne(parms);

					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// This logic only accessed if memory limited and priority threads are used up 
			if (queueListItem == null)
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
					parms.ProcessorID = processorId;
					parms.WorkQueueTypeEnumList = GetWorkQueueTypeString();

					queueListItem = select.FindOne(parms);

					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// Update internal counts
			if (queueListItem != null)
			{
				lock (_syncLock)
				{
					if (queueListItem.WorkQueuePriorityEnum.Equals(WorkQueuePriorityEnum.High))
						_highPriorityCount++;
					if (!_nonMemoryLimitedList.ContainsKey(queueListItem.WorkQueueTypeEnum))
						_memoryLimitedCount++;

					_totalThreadCount++;
				}
			}
			return queueListItem;
		}

		#endregion
	}
}
