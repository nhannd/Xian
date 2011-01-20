#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IProtocolWorklistItemBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ProtocolWorklistItemBroker : WorklistItemBrokerBase, IProtocolWorklistItemBroker
	{
		public ProtocolWorklistItemBroker()
			: base(new ProtocolWorklistItemQueryBuilder())
		{
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		/// <param name="worklistItemQueryBuilder"></param>
		/// <param name="procedureSearchQueryBuilder"></param>
		/// <param name="patientSearchQueryBuilder"></param>
		protected ProtocolWorklistItemBroker(IWorklistItemQueryBuilder worklistItemQueryBuilder,
			IQueryBuilder procedureSearchQueryBuilder, IQueryBuilder patientSearchQueryBuilder)
			:base(worklistItemQueryBuilder, procedureSearchQueryBuilder, patientSearchQueryBuilder)
		{
		}

		#region IProtocolWorklistItemBroker Members

		/// <summary>
		/// Maps the specified set of protocolling steps to a corresponding set of reporting worklist items.
		/// </summary>
		/// <param name="protocollingSteps"></param>
		/// <param name="timeField"></param>
		/// <returns></returns>
		public IList<ReportingWorklistItem> GetWorklistItems(IEnumerable<ProtocolProcedureStep> protocollingSteps, WorklistItemField timeField)
		{
			var worklistItemCriteria =
				CollectionUtils.Map(protocollingSteps,
				delegate(ProtocolProcedureStep ps)
				{
					var criteria = new ReportingWorklistItemSearchCriteria();
					criteria.ProcedureStep.EqualTo(ps);
					return criteria;
				}).ToArray();

			var projection = WorklistItemProjection.GetDefaultProjection(timeField);
			var args = new SearchQueryArgs(typeof (ProtocolProcedureStep), worklistItemCriteria, projection);
			var query = this.BuildWorklistSearchQuery(args);
			return DoQuery<ReportingWorklistItem>(query, this.WorklistItemQueryBuilder, args);
		}

		/// <summary>
		/// Obtains a set of interpretation steps that are candidates for linked reporting to the specified interpretation step.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="author"></param>
		/// <returns></returns>
		public IList<ProtocolAssignmentStep> GetLinkedProtocolCandidates(ProtocolAssignmentStep step, Staff author)
		{
			var q = this.GetNamedHqlQuery("linkedProtocolCandidates");
			q.SetParameter(0, step);
			q.SetParameter(1, author);
			return q.List<ProtocolAssignmentStep>();
		}

		#endregion
	}
}
