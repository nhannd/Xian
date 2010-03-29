using System;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public class SearchQueryArgs : QueryBuilderArgs
	{
		public SearchQueryArgs(Type procedureStepClass, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection)
			: base(new [] { procedureStepClass }, criteria, projection, null)
		{
		}

		public SearchQueryArgs(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection)
			: base(procedureStepClasses, criteria, projection, null)
		{
		}
	}
}
