using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public interface IWorklistItemQueryBuilder : IQueryBuilder
	{
		void AddFilters(HqlProjectionQuery query, WorklistQueryArgs args);
		void AddActiveProcedureStepConstraint(HqlProjectionQuery query, QueryBuilderArgs args);
	}
}
