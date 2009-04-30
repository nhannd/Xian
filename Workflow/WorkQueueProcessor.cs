#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common.Shreds;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow.Brokers;

namespace ClearCanvas.Workflow
{
	/// <summary>
	/// A specialization of <see cref="QueueProcessor{TItem}"/> that operates on items of type <see cref="WorkQueueItem"/>.
	/// </summary>
	public abstract class WorkQueueProcessor : EntityQueueProcessor<WorkQueueItem>
	{
		protected WorkQueueProcessor(int batchSize, TimeSpan sleepTime)
			:base(batchSize, sleepTime)
		{
		}

		/// <summary>
		/// Gets the next batch of items from the queue.
		/// </summary>
		/// <remarks>
		/// Subclasses should not need to override this method.
		/// </remarks>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		protected override IList<WorkQueueItem> GetNextEntityBatch(int batchSize)
		{
			return PersistenceScope.CurrentContext.GetBroker<IWorkQueueItemBroker>().GetPendingItems(WorkQueueItemType, batchSize);
		}

		/// <summary>
		/// Called when <see cref="QueueProcessor{TItem}.ProcessItem"/> succeeds.
		/// </summary>
		/// <remarks>
		/// Subclasses should not need to override this method.
		/// </remarks>
		/// <param name="item"></param>
		protected override void OnItemSucceeded(WorkQueueItem item)
		{
			item.Complete();
		}

		/// <summary>
		/// Called when <see cref="QueueProcessor{TItem}.ProcessItem"/> throws an exception.
		/// </summary>
		/// <remarks>
		/// Subclasses should not need to override this method.
		/// </remarks>
		/// <param name="item"></param>
		/// <param name="error"></param>
		protected override void OnItemFailed(WorkQueueItem item, Exception error)
		{
			// mark item as failed
			item.Fail(error.Message);

			// optionally reschedule the item
			DateTime retryTime;
			if (ShouldRetry(item, error, out retryTime))
				item.Reschedule(retryTime);
		}

		/// <summary>
		/// Gets the type of work queue item that this processor operates on.
		/// </summary>
		protected abstract string WorkQueueItemType { get; }

		/// <summary>
		/// Called after a work item fails, to determine whether it should be re-tried.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="error"></param>
		/// <param name="retryTime"></param>
		/// <returns></returns>
		protected abstract bool ShouldRetry(WorkQueueItem item, Exception error, out DateTime retryTime);

	}
}
