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
	[WorklistCategory("WorklistCategoryProtocoling")]
	public abstract class ProtocolingWorklist : ReportingWorklist
	{
	}

	/// <summary>
	/// ReportingToBeProtocolledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingToBeProtocolledWorklistDescription")]
	public class ReportingToBeProtocolledWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);	//bug #3498: exclude procedures that are no longer in SC status 
			criteria.Protocol.Status.EqualTo(ProtocolStatus.PN);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedProtocolWorklistDescription")]
	public class ReportingAssignedProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.PN);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingToBeReviewedProtocolWorklistDescription")]
	public class ReportingToBeReviewedProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.Protocol.Status.EqualTo(ProtocolStatus.AA);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedReviewProtocolWorklistDescription")]
	public class ReportingAssignedReviewProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.AA);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingDraftProtocolWorklistDescription")]
	public class ReportingDraftProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingCompletedProtocolWorklistDescription")]
	public class ReportingCompletedProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.PR);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingRejectedProtocolWorklistDescription")]
	public class ReportingRejectedProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAwaitingApprovalProtocolWorklistDescription")]
	public class ReportingAwaitingApprovalProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.Protocol.Author.EqualTo(wqc.Staff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.AA);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, null, WorklistOrdering.PrioritizeNewestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}
}
