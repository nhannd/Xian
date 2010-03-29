using System;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public class QueryBuilderArgs
	{
		protected QueryBuilderArgs()
		{
		}

		public QueryBuilderArgs(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page)
		{
			Initialize(procedureStepClasses, criteria, projection, page);
		}

		protected void Initialize(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page)
		{
			this.Criteria = criteria;
			this.Projection = projection;
			this.Page = page;
			this.ProcedureStepClasses = procedureStepClasses;
		}

		public WorklistItemSearchCriteria[] Criteria { get; private set; }

		public WorklistItemProjection Projection { get; private set; }

		public bool CountQuery { get { return this.Projection == null; } }

		public Type[] ProcedureStepClasses { get; private set; }

		public SearchResultPage Page { get; private set; }
	}
}
