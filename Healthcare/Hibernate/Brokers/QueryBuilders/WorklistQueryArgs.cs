namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public class WorklistQueryArgs : QueryBuilderArgs
	{
		public WorklistQueryArgs(Worklist worklist, IWorklistQueryContext wqc, bool countQuery)
		{
			this.Worklist = worklist;
			this.QueryContext = wqc;
			this.FilterCriteria = worklist.GetFilterCriteria(wqc);

			// init base class
			Initialize(
				worklist.GetProcedureStepSubclasses(),
				worklist.GetInvariantCriteria(wqc),
				countQuery ? null : worklist.GetProjection(),
				wqc.Page);
		}

		public Worklist Worklist { get; private set; }

		public IWorklistQueryContext QueryContext { get; private set; }

		public WorklistItemSearchCriteria[] FilterCriteria { get; private set; }
	}
}
