#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	/// Implementation of <see cref="IReportingWorklistItemBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ReportingWorklistItemBroker : WorklistItemBrokerBase, IReportingWorklistItemBroker
	{
		public ReportingWorklistItemBroker()
			: base(new ReportingWorklistItemQueryBuilder())
		{
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		/// <param name="worklistItemQueryBuilder"></param>
		/// <param name="procedureSearchQueryBuilder"></param>
		/// <param name="patientSearchQueryBuilder"></param>
		protected ReportingWorklistItemBroker(IWorklistItemQueryBuilder worklistItemQueryBuilder,
			IQueryBuilder procedureSearchQueryBuilder, IQueryBuilder patientSearchQueryBuilder)
			:base(worklistItemQueryBuilder, procedureSearchQueryBuilder, patientSearchQueryBuilder)
		{
		}

		#region IReportingWorklistItemBroker Members

		/// <summary>
		/// Maps the specified set of reporting steps to a corresponding set of reporting worklist items.
		/// </summary>
		/// <param name="reportingSteps"></param>
		/// <returns></returns>
		public IList<ReportingWorklistItem> GetWorklistItems(IEnumerable<ReportingProcedureStep> reportingSteps, WorklistItemField timeField)
		{
			var worklistItemCriteria =
				CollectionUtils.Map(reportingSteps,
				delegate(ReportingProcedureStep ps)
				{
					var criteria = new ReportingWorklistItemSearchCriteria();
					criteria.ProcedureStep.EqualTo(ps);
					return criteria;
				}).ToArray();

			var projection = WorklistItemProjection.GetReportingProjection(timeField);
			var args = new SearchQueryArgs(typeof(ReportingProcedureStep), worklistItemCriteria, projection);
			var query = this.BuildWorklistSearchQuery(args);

			return DoQuery<ReportingWorklistItem>(query, this.WorklistItemQueryBuilder, args);
		}

		/// <summary>
		/// Obtains a set of interpretation steps that are candidates for linked reporting to the specified interpretation step.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="interpreter"></param>
		/// <returns></returns>
		public IList<InterpretationStep> GetLinkedInterpretationCandidates(InterpretationStep step, Staff interpreter)
		{
			var q = this.Context.GetNamedHqlQuery("linkedInterpretationCandidates");
			q.SetParameter(0, step);
			q.SetParameter(1, interpreter);
			return q.List<InterpretationStep>();
		}

		#endregion
	}
}
