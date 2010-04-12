using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Defines an interface for building queries.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This interface provides a number of methods that assemble different parts of a query,
	/// allowing for a number of different variations of the same query to be constructed by
	/// calling different combinations of methods.  The order in which the methods are called
	/// generally does not matter, with the exception that <see cref="AddRootQuery"/> must
	/// always be called, and must be called first.
	/// </para>
	/// <para>
	/// The <see cref="PreProcessResult"/> method is called to pre-process result tuples, which 
	/// allows the query builder the freedom to create a 'select' clause that differs from what
	/// is expected by the broker.
	/// </para>
	/// </remarks>
	public interface IQueryBuilder
	{
		/// <summary>
		/// Establishes the root query (the 'from' clause and any 'join' clauses).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds criteria to the query (the 'where' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddCriteria(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds ordering to the query (the 'rder by' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddOrdering(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds a count projection to the query (e.g. 'select count(*)').
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddCountProjection(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds an item projection to the query (the 'select' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddItemProjection(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds a paging restriction to the query (the 'top' or 'limit' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddPagingRestriction(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Query result tuples are passed through this method in order to perform
		/// any pre-processing.
		/// </summary>
		/// <param name="tuple"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		object[] PreProcessResult(object[] tuple, QueryBuilderArgs args);
	}
}
