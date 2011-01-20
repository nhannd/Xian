#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IOrderBroker"/>. See OrderBroker.hbm.xml for queries.
	/// </summary>
	public partial class OrderBroker : IOrderBroker
	{
		#region IOrderBroker Members

		public Order FindDocumentOwner(AttachedDocument document)
		{
			var q = this.GetNamedHqlQuery("documentOrderOwner");
			q.SetParameter(0, document);
			return (Order) q.UniqueResult(); 
		}

		public IList<Order> FindByOrderingPractitioner(ExternalPractitioner practitioner)
		{
			var q = this.GetNamedHqlQuery("ordersForOrderingPractitioner");
			q.SetParameter(0, practitioner);
			return CollectionUtils.Unique(q.List<Order>());
		}

		public IList<Order> FindByResultRecipient(ResultRecipientSearchCriteria recipientSearchCriteria, OrderSearchCriteria orderSearchCriteria)
		{
			var hqlFrom = new HqlFrom(typeof(Order).Name, "o");
			hqlFrom.Joins.Add(new HqlJoin("o.ResultRecipients", "rr"));

			var query = new HqlProjectionQuery(hqlFrom);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("rr", recipientSearchCriteria));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("o", orderSearchCriteria));
			return ExecuteHql<Order>(query);
		}

		#endregion
	}
}
