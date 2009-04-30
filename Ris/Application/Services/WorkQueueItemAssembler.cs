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

using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services
{
	public class WorkQueueItemAssembler
	{
		public WorkQueueItemSummary CreateWorkQueueItemSummary(WorkQueueItem workQueueItem, IPersistenceContext context)
		{
			WorkQueueItemSummary summary = new WorkQueueItemSummary();

			summary.WorkQueueItemRef = workQueueItem.GetRef();
			summary.CreationTime = workQueueItem.CreationTime;
			summary.ScheduledTime = workQueueItem.ScheduledTime;
			summary.ExpirationTime = workQueueItem.ExpirationTime;
			summary.User = workQueueItem.User;
			summary.Type = workQueueItem.Type;
			summary.Status = EnumUtils.GetEnumValueInfo(workQueueItem.Status, context);
			summary.ProcessedTime = workQueueItem.ProcessedTime;
			summary.FailureCount = workQueueItem.FailureCount;
			summary.FailureDescription = workQueueItem.FailureDescription;

			return summary;
		}

		public WorkQueueItemDetail CreateWorkQueueItemDetail(WorkQueueItem workQueueItem, IPersistenceContext context)
		{
			WorkQueueItemDetail detail = new WorkQueueItemDetail();

			detail.WorkQueueItemRef = workQueueItem.GetRef();
			detail.CreationTime = workQueueItem.CreationTime;
			detail.ScheduledTime = workQueueItem.ScheduledTime;
			detail.ExpirationTime = workQueueItem.ExpirationTime;
			detail.User = workQueueItem.User;
			detail.Type = workQueueItem.Type;
			detail.Status = EnumUtils.GetEnumValueInfo(workQueueItem.Status, context);
			detail.ProcessedTime = workQueueItem.ProcessedTime;
			detail.FailureCount = workQueueItem.FailureCount;
			detail.FailureDescription = workQueueItem.FailureDescription;
			detail.ExtendedProperties = new Dictionary<string, string>(workQueueItem.ExtendedProperties);

			return detail;
		}
	}
}