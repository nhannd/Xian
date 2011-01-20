#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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