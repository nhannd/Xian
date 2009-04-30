#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Abstract base class for protocoling worklists.
	/// </summary>
	[WorklistCategory("WorklistCategoryRadiologistAdmin")]
	public abstract class RadiologistAdminWorklist : ReportingWorklist
	{
	}

	/// <summary>
	/// ReportingAdminToBeReportedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingAdminUnreportedWorklistDescription")]
	public class ReportingAdminUnreportedWorklist : RadiologistAdminWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(InterpretationStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();	// only want steps that are actually scheduled
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// ReportingAdminAssignedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingAdminAssignedWorklistDescription")]
	public class ReportingAdminAssignedWorklist : RadiologistAdminWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria performerCriteria = BuildCommonCriteria(wqc);
			performerCriteria.ProcedureStep.Performer.Staff.IsNotNull();

			ReportingWorklistItemSearchCriteria scheduledPerformerCriteria = BuildCommonCriteria(wqc);
			scheduledPerformerCriteria.ProcedureStep.Scheduling.Performer.Staff.IsNotNull();

			return new ReportingWorklistItemSearchCriteria[] { performerCriteria, scheduledPerformerCriteria };
		}

		private ReportingWorklistItemSearchCriteria BuildCommonCriteria(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ReportingProcedureStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP, ActivityStatus.SU });
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return criteria;
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ProtocollingAdminAssignedWorklist")]
	public class ProtocollingAdminAssignedWorklist : RadiologistAdminWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria performerCriteria = BuildCommonCriteria(wqc);
			performerCriteria.ProcedureStep.Performer.Staff.IsNotNull();

			ReportingWorklistItemSearchCriteria scheduledPerformerCriteria = BuildCommonCriteria(wqc);
			scheduledPerformerCriteria.ProcedureStep.Scheduling.Performer.Staff.IsNotNull();

			return new ReportingWorklistItemSearchCriteria[] { performerCriteria, scheduledPerformerCriteria };
		}

		private ReportingWorklistItemSearchCriteria BuildCommonCriteria(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP, ActivityStatus.SU });
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return criteria;
		}
	}
}
