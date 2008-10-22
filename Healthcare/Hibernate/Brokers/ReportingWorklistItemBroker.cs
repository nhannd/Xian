#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IReportingWorklistItemBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ReportingWorklistItemBroker : WorklistItemBrokerBase<WorklistItem>, IReportingWorklistItemBroker
	{
		#region HQL Constants

		private static readonly HqlSelect SelectReport = new HqlSelect("r");
		private static readonly HqlSelect SelectReportPart = new HqlSelect("rpp");
		private static readonly HqlJoin JoinReportPart = new HqlJoin("ps.ReportPart", "rpp", HqlJoinMode.Left);
		private static readonly HqlJoin JoinReport = new HqlJoin("rpp.Report", "r", HqlJoinMode.Left);

		private static readonly HqlCondition ConditionMostRecentProtocolAssignmentStepIfRejected = new HqlCondition(
			"((pr.Status not in (?)) or (ps.EndTime = (select max(ps2.EndTime) from ProcedureStep ps2 where ps.Protocol = ps2.Protocol)))", ProtocolStatus.RJ);

		private static readonly HqlCondition ConditionMostRecentPublicationStep = new HqlCondition(
			"(ps.Scheduling.StartTime = (select max(ps2.Scheduling.StartTime) from ProcedureStep ps2 where ps.ReportPart.Report = ps2.ReportPart.Report and ps2.State not in (?)))", ActivityStatus.DC);

		#endregion

		#region IReportingWorklistItemBroker Members

		/// <summary>
		/// Maps the specified set of reporting steps to a corresponding set of reporting worklist items.
		/// </summary>
		/// <param name="reportingSteps"></param>
		/// <returns></returns>
		public IList<WorklistItem> GetWorklistItems(IEnumerable<ReportingProcedureStep> reportingSteps)
		{
			ReportingWorklistItemSearchCriteria[] worklistItemCriteria =
				CollectionUtils.Map<ReportingProcedureStep, ReportingWorklistItemSearchCriteria>(reportingSteps,
				delegate(ReportingProcedureStep ps)
				{
					ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
					criteria.ProcedureStepClass = typeof(ReportingProcedureStep);
					criteria.ProcedureStep.EqualTo(ps);
					criteria.TimeField = WorklistTimeField.ProcedureStartTime;
					return criteria;
				}).ToArray();


			HqlProjectionQuery query = CreateBaseItemQuery(worklistItemCriteria);
			AddConditions(query, worklistItemCriteria, true, true);

			return DoQuery(query);
		}

		/// <summary>
		/// Maps the specified set of protocolling steps to a corresponding set of reporting worklist items.
		/// </summary>
		/// <param name="protocollingSteps"></param>
		/// <returns></returns>
		public IList<WorklistItem> GetWorklistItems(IEnumerable<ProtocolProcedureStep> protocollingSteps)
		{
			ReportingWorklistItemSearchCriteria[] worklistItemCriteria =
				CollectionUtils.Map<ProtocolProcedureStep, ReportingWorklistItemSearchCriteria>(protocollingSteps,
				delegate(ProtocolProcedureStep ps)
				{
					ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
					criteria.ProcedureStepClass = typeof(ProtocolProcedureStep);
					criteria.ProcedureStep.EqualTo(ps);
					criteria.TimeField = WorklistTimeField.ProcedureStartTime;
					return criteria;
				}).ToArray();

			HqlProjectionQuery query = CreateBaseItemQuery(worklistItemCriteria);
			AddConditions(query, worklistItemCriteria, true, true);

			return DoQuery(query);
		}

		/// <summary>
		/// Obtains a set of interpretation steps that are candidates for linked reporting to the specified interpretation step.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="interpreter"></param>
		/// <returns></returns>
		public IList<InterpretationStep> GetLinkedInterpretationCandidates(InterpretationStep step, Staff interpreter)
		{
			NHibernate.IQuery q = this.Context.GetNamedHqlQuery("linkedInterpretationCandidates");
			q.SetParameter(0, step);
			q.SetParameter(1, interpreter);
			return q.List<InterpretationStep>();
		}

		/// <summary>
		/// Obtains a set of interpretation steps that are candidates for linked reporting to the specified interpretation step.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="author"></param>
		/// <returns></returns>
		public IList<ProtocolAssignmentStep> GetLinkedProtocolCandidates(ProtocolAssignmentStep step, Staff author)
		{
			NHibernate.IQuery q = this.Context.GetNamedHqlQuery("linkedProtocolCandidates");
			q.SetParameter(0, step);
			q.SetParameter(1, author);
			return q.List<ProtocolAssignmentStep>();
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Creates an <see cref="HqlProjectionQuery"/> that queries for worklist items based on the specified
		/// procedure-step class.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		/// <remarks>
		/// Subclasses may override this method to customize the query or return an entirely different query.
		/// </remarks>
		protected override HqlProjectionQuery CreateBaseItemQuery(WorklistItemSearchCriteria[] criteria)
		{
			Type procedureStepClass = CollectionUtils.FirstElement(criteria).ProcedureStepClass;
			HqlProjectionQuery query = base.CreateBaseItemQuery(criteria);
			ModifyQuery(query, procedureStepClass, false);
			return query;
		}

		/// <summary>
		/// Creates an <see cref="HqlProjectionQuery"/> that queries for the count of worklist items based on the specified
		/// procedure-step class.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		/// <remarks>
		/// Subclasses may override this method to customize the query or return an entirely different query.
		/// </remarks>
		protected override HqlProjectionQuery CreateBaseCountQuery(WorklistItemSearchCriteria[] criteria)
		{
			Type procedureStepClass = CollectionUtils.FirstElement(criteria).ProcedureStepClass;
			HqlProjectionQuery query = base.CreateBaseCountQuery(criteria);
			ModifyQuery(query, procedureStepClass, true);
			return query;
		}

		protected override HqlProjectionQuery BuildWorklistItemSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery)
		{
			// need to set the correct time field
			// ProcedureStartTime seems like a reasonable choice for rad homepage search,
			// as it gives a general sense of when the procedure occured in time, regardless of the procedure step
			CollectionUtils.ForEach(where,
				delegate(WorklistItemSearchCriteria sc)
				{
					sc.TimeField = WorklistTimeField.ProcedureStartTime;
				});

			HqlProjectionQuery query = countQuery ? CreateBaseCountQuery(where) : CreateBaseItemQuery(where);

			// active steps only
			query.Conditions.Add(ConditionActiveProcedureStep);

			AddConditions(query, where, true, !countQuery);

			return query;
		}

		#endregion

		#region Helpers

		private void ModifyQuery(HqlProjectionQuery query, Type stepClass, bool isCountQuery)
		{
			if (!isCountQuery)
				query.Selects.Add(SelectProcedureStepState);

			HqlFrom from = query.Froms[0];
			if (stepClass == typeof(ProtocolAssignmentStep) || stepClass == typeof(ProtocolProcedureStep))
			{
				from.Joins.Add(JoinProtocol);

				// when querying for Rejected protocols, only show the most recent ProtocolAssignmentStep, as there may be many of them
				query.Conditions.Add(ConditionMostRecentProtocolAssignmentStepIfRejected);
			}
			else if (stepClass == typeof(ProtocolResolutionStep))
			{
				from.Joins.Add(JoinProtocol);
			}
			else
			{
				// if this is a reporting step, rather than a protocoling step, include the report object
				from.Joins.Add(JoinReportPart);
				from.Joins.Add(JoinReport);

				if(stepClass == typeof(PublicationStep))
					query.Conditions.Add(ConditionMostRecentPublicationStep);

				if (!isCountQuery)
				{
					query.Selects.Add(SelectReport);
					query.Selects.Add(SelectReportPart);
				}
			}
		}

		#endregion
	}
}
