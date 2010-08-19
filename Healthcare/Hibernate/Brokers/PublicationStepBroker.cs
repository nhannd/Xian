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

using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class PublicationStepBroker : EntityBroker<PublicationStep, PublicationStepSearchCriteria>, IPublicationStepBroker
	{
		public IList<PublicationStep> FindUnprocessedSteps(int failedItemRetryDelay, SearchResultPage page)
		{
			var query = new HqlProjectionQuery(new HqlFrom(typeof(PublicationStep).Name, "ps"));
			query.Conditions.Add(new HqlCondition("ps.State = ?", ActivityStatus.SC));
			query.Conditions.Add(new HqlCondition("ps.Scheduling.Performer.Staff is not null"));
			query.Conditions.Add(new HqlCondition("ps.Scheduling.StartTime < ?", Platform.Time));
			query.Conditions.Add(new HqlCondition("(ps.LastFailureTime is null or ps.LastFailureTime < ?)", Platform.Time.AddSeconds(-failedItemRetryDelay)));
			query.Sorts.Add(new HqlSort("ps.Scheduling.StartTime", true, 0));
			query.Page = page;
			return ExecuteHql<PublicationStep>(query);
		}

	}
}