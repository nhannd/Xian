using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Extends <see cref="IQueryBuilder"/> with functionality specific to worklist item queries.
	/// </summary>
	public interface IWorklistItemQueryBuilder : IQueryBuilder
	{
		/// <summary>
		/// Adds worklist filters to the query (affects the 'from' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddFilters(HqlProjectionQuery query, WorklistQueryArgs args);

		/// <summary>
		/// Adds the "active procedure step" constraint (affects the 'from' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddActiveProcedureStepConstraint(HqlProjectionQuery query, QueryBuilderArgs args);
	}
}
