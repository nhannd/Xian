#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Specialization of <see cref="QueryBuilderArgs"/> used for worklist item queries.
	/// </summary>
	public class WorklistQueryArgs : QueryBuilderArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="wqc"></param>
		/// <param name="countQuery"></param>
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

		/// <summary>
		/// Gets the worklist that is generating the query.
		/// </summary>
		public Worklist Worklist { get; private set; }

		/// <summary>
		/// Gets the worklist query context.
		/// </summary>
		public IWorklistQueryContext QueryContext { get; private set; }

		/// <summary>
		/// Gets the worklist filter criteria.
		/// </summary>
		public WorklistItemSearchCriteria[] FilterCriteria { get; private set; }
	}
}
