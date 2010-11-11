#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
			// See if the item needs to be rescheduled for another processing before completing it.
			DateTime rescheduleTime;
			if (ShouldReschedule(item, null, out rescheduleTime))
				item.Reschedule(rescheduleTime);
			else
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
			if (ShouldReschedule(item, error, out retryTime))
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
		//protected abstract bool ShouldRetry(WorkQueueItem item, Exception error, out DateTime retryTime);

		/// <summary>
		/// Called after a work item is processed, regardless of whether it succeeded or failed,
		/// to determine whether it should be rescheduled for further processing.
		/// </summary>
		/// <remarks>
		/// If the item failed, it may need to rescheduled to try again.  If the item succeeded, it may need to be
		/// rescheduled for further work.
		/// </remarks>
		/// <param name="item">Item in question.</param>
		/// <param name="error">If processing failed, the exception that it failed with.  Otherwise null.</param>
		/// <param name="rescheduleTime">Time for which processing should be rescheduled.</param>
		/// <returns></returns>
		/// <remarks>
		/// Subclasses should override this method if it takes several stages to completely process an item.
		/// </remarks>
		protected virtual bool ShouldReschedule(WorkQueueItem item, Exception error, out DateTime rescheduleTime)
		{
			// By default, an item should not be rescheduled
			rescheduleTime = DateTime.MinValue;
			return false;
		}
	}
}
