using System;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Holds arguments to <see cref="IQueryBuilder"/> methods.
	/// </summary>
	public class QueryBuilderArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="criteria"></param>
		/// <param name="projection"></param>
		/// <param name="page"></param>
		public QueryBuilderArgs(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page)
		{
			Initialize(procedureStepClasses, criteria, projection, page);
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected QueryBuilderArgs()
		{
		}

		/// <summary>
		/// Gets the search criteria.
		/// </summary>
		public WorklistItemSearchCriteria[] Criteria { get; private set; }

		/// <summary>
		/// Gets the worlist item projection, or null if this is a count query.
		/// </summary>
		public WorklistItemProjection Projection { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this is a count query.
		/// </summary>
		public bool CountQuery { get { return this.Projection == null; } }

		/// <summary>
		/// Gets the procedure step classes that are included in the query.
		/// </summary>
		public Type[] ProcedureStepClasses { get; private set; }

		/// <summary>
		/// Gets the result set page.
		/// </summary>
		public SearchResultPage Page { get; private set; }

		/// <summary>
		/// Initializes this object.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="criteria"></param>
		/// <param name="projection"></param>
		/// <param name="page"></param>
		protected void Initialize(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page)
		{
			this.Criteria = criteria;
			this.Projection = projection;
			this.Page = page;
			this.ProcedureStepClasses = procedureStepClasses;
		}
	}
}
