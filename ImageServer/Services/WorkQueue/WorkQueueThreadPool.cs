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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
	/// <summary>
	/// Delegate used with <see cref="WorkQueueThreadPool"/> class.
	/// </summary>
	/// <param name="processor">The WorkQueue processor.</param>
	/// <param name="queueItem">The actual WorkQueue item.</param>
	public delegate void WorkQueueThreadDelegate(IWorkQueueItemProcessor processor, Model.WorkQueue queueItem);

	/// <summary>
	/// Class used to pass parameters to threads in the <see cref="WorkQueueThreadPool"/>.
	/// </summary>
	public class WorkQueueThreadParameter
	{
		private readonly IWorkQueueItemProcessor _processor;
		private readonly Model.WorkQueue _item;
		private readonly WorkQueueThreadDelegate _del;

		public WorkQueueThreadParameter(IWorkQueueItemProcessor processor, Model.WorkQueue item, WorkQueueThreadDelegate del)
		{
			_item = item;
			_processor = processor;
			_del = del;
		}

		public IWorkQueueItemProcessor Processor
		{
			get { return _processor; }
		}

		public Model.WorkQueue Item
		{
			get { return _item; }
		}

		public WorkQueueThreadDelegate Delegate
		{
			get { return _del; }
		}
	}

	/// <summary>
	/// Class for managing the WorkQueue thread pool.
	/// </summary>
	/// <remarks>
	/// This class is used to keep track of the current types of WorkQueue entries being processed and 
	/// for requested new queue entries based on the current types being processed.
	/// </remarks>
	public class WorkQueueThreadPool : ItemProcessingThreadPool<WorkQueueThreadParameter>
	{
		#region Private Members
		private readonly object _syncLock = new object();
		private readonly int _highPriorityThreadLimit;
		private readonly int _memoryLimitedThreadLimit;
		private int _memoryLimitedCount = 0;
		private int _highPriorityCount = 0;
		private int _totalThreadCount = 0;
		private readonly Dictionary<WorkQueueTypeEnum,WorkQueueTypeEnum> _nonMemoryLimitedList = new Dictionary<WorkQueueTypeEnum, WorkQueueTypeEnum>();
		private readonly List<WorkQueueThreadParameter> _queuedItems;
		#endregion

		#region Properties
		/// <summary>
		/// Are there threads available for queueing?
		/// </summary>
		public bool CanQueueItem
		{
			get
			{
				return (QueueCount + ActiveCount) < Concurrency;
			}
		}

		/// <summary>
		/// High Priority threads available.
		/// </summary>
		public bool HighPriorityThreadsAvailable
		{
			get
			{
				lock (_syncLock)
				{
					return _highPriorityCount < _highPriorityThreadLimit;
				}
			}
		}

		/// <summary>
		/// Memory limited threads available.
		/// </summary>
		public bool MemoryLimitedThreadsAvailable
		{
			get
			{
				lock (_syncLock)
				{
					return _memoryLimitedCount < _memoryLimitedThreadLimit;
				}
			}
		}
		#endregion

		#region Contructors
		/// <summary>
		/// Constructors.
		/// </summary>
		/// <param name="totalThreadCount">Total threads to be put in the thread pool.</param>
		/// <param name="highPriorityCount">Maximum high priority threads.</param>
		/// <param name="memoryLimitedThreadLimit">Maximum memory limited threads.</param>
		/// <param name="nonMemoryLimitedList">List of WorkQueue types that are non-memory limited.</param>
		public WorkQueueThreadPool(int totalThreadCount, int highPriorityCount, int memoryLimitedThreadLimit, IList<WorkQueueTypeEnum> nonMemoryLimitedList )
			: base(totalThreadCount)
		{
			_highPriorityThreadLimit = highPriorityCount;
			_memoryLimitedThreadLimit = memoryLimitedThreadLimit;
			foreach (WorkQueueTypeEnum type in nonMemoryLimitedList)
				_nonMemoryLimitedList.Add(type, type);
			_queuedItems = new List<WorkQueueThreadParameter>(totalThreadCount + 1);
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Override of OnStop method.
		/// </summary>
		/// <param name="completeBeforeStop"></param>
		/// <returns></returns>
		protected override bool OnStop(bool completeBeforeStop)
		{
			if (!base.OnStop(completeBeforeStop))
				return false;
			lock (_syncLock)
			{
				foreach (WorkQueueThreadParameter queuedItem in _queuedItems)
				{
					ICancelable cancel = queuedItem.Processor as ICancelable;
					if (cancel != null)
						cancel.Cancel();
				}
			}
			return true;
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
		private void QueueItemComplete(Model.WorkQueue queueItem)
		{
			lock (_syncLock)
			{
				if (queueItem.WorkQueuePriorityEnum.Equals(WorkQueuePriorityEnum.High))
					_highPriorityCount--;

				if (!_nonMemoryLimitedList.ContainsKey(queueItem.WorkQueueTypeEnum))
					_memoryLimitedCount--;

				_totalThreadCount--;

				foreach(WorkQueueThreadParameter queuedItem in _queuedItems)
				{
					if (queuedItem.Item.Key.Equals(queueItem.Key))
					{
						_queuedItems.Remove(queuedItem);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Enqueue a WorkQueue entry for processing.
		/// </summary>
		/// <param name="processor"></param>
		/// <param name="item"></param>
		/// <param name="del"></param>
		public void Enqueue(IWorkQueueItemProcessor processor, Model.WorkQueue item, WorkQueueThreadDelegate del)
		{
			WorkQueueThreadParameter parameter = new WorkQueueThreadParameter(processor, item, del);

			lock (_syncLock)
			{
				if (item.WorkQueuePriorityEnum.Equals(WorkQueuePriorityEnum.High))
					_highPriorityCount++;
				if (!_nonMemoryLimitedList.ContainsKey(item.WorkQueueTypeEnum))
					_memoryLimitedCount++;

				_totalThreadCount++;

				_queuedItems.Add(parameter);
			}
			
			Enqueue(parameter,delegate(WorkQueueThreadParameter threadParameter)
			                       	{
			                       		threadParameter.Delegate(threadParameter.Processor, threadParameter.Item);

										QueueItemComplete(threadParameter.Item);
			                       	});
		}
		#endregion
	}
}
