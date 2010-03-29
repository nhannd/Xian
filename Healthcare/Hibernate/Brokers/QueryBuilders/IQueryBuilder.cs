using System;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public interface IQueryBuilder
	{
		void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args);
		void AddCriteria(HqlProjectionQuery query, QueryBuilderArgs args);
		void AddOrdering(HqlProjectionQuery query, QueryBuilderArgs args);
		void AddCountProjection(HqlProjectionQuery query, QueryBuilderArgs args);
		void AddItemProjection(HqlProjectionQuery query, QueryBuilderArgs args);
		void AddPagingRestriction(HqlProjectionQuery query, QueryBuilderArgs args);
		object[] PreProcessTuple(object[] tuple, QueryBuilderArgs args);
	}
}
