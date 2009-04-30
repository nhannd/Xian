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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.Workflow.Hibernate.Brokers
{
	public partial class WorkQueueItemBroker
	{
		private static readonly HqlFrom FromWorkQueueItem = new HqlFrom("WorkQueueItem", "item");

		private static readonly HqlSelect SelectType = new HqlSelect("item.Type");

		#region IWorkQueueItemBroker Members

		public IList<string> GetTypes()
		{
			HqlProjectionQuery query = new HqlProjectionQuery(
				FromWorkQueueItem,
				new HqlSelect[] { SelectType });

			query.SelectDistinct = true;

			return ExecuteHql<string>(query);
		}

		public IList<WorkQueueItem> GetPendingItems(string type, int maxItems)
		{
			HqlQuery query = new HqlQuery("from WorkQueueItem item");
			query.Conditions.Add(new HqlCondition("item.Type = ?", type));
			query.Conditions.Add(new HqlCondition("item.Status = ?", WorkQueueStatus.PN));

			DateTime now = Platform.Time;
			query.Conditions.Add(new HqlCondition("(item.ScheduledTime is null or item.ScheduledTime < ?)", now));
			query.Conditions.Add(new HqlCondition("(item.ExpirationTime is null or item.ExpirationTime > ?)", now));
            query.Sorts.Add(new HqlSort("item.ScheduledTime", true, 0));
			query.Page = new SearchResultPage(0, maxItems);

			return ExecuteHql<WorkQueueItem>(query);
		}

		#endregion
	}
}
