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

using System;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	[WorklistCategory("WorklistCategoryReportingTracking")]
	[WorklistSupportsReportingStaffRoleFilter(true)]
	public abstract class ReportingTrackingWorklist : ReportingWorklist
	{
	}

	/// <summary>
	/// ReportingTrackingActiveWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingTrackingActiveWorklistDescription")]
	public class ReportingTrackingActiveWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var performerCriteria = BuildCommonCriteria(wqc);
			performerCriteria.ProcedureStep.Performer.Staff.IsNotNull();

			var scheduledPerformerCriteria = BuildCommonCriteria(wqc);
			scheduledPerformerCriteria.ProcedureStep.Scheduling.Performer.Staff.IsNotNull();

			return new[] { performerCriteria, scheduledPerformerCriteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(ReportingProcedureStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}

		private static ReportingWorklistItemSearchCriteria BuildCommonCriteria(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP, ActivityStatus.SU });
			return criteria;
		}
	}

	/// <summary>
	/// ReportingTrackingDraftWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingTrackingDraftWorklistDescription")]
	public class ReportingTrackingDraftWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.Report.Status.EqualTo(ReportStatus.D);
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(ReportingProcedureStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ReportPartPreliminaryTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// ReportingTrackingPreliminaryWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingTrackingPreliminaryWorklistDescription")]
	public class ReportingTrackingPreliminaryWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.Report.Status.EqualTo(ReportStatus.P);
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new [] { typeof(ReportingProcedureStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ReportPartPreliminaryTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// ReportingTrackingFinalWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingTrackingFinalWorklistDescription")]
	public class ReportingTrackingFinalWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.Report.Status.EqualTo(ReportStatus.F);
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(PublicationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ReportPartCompletedTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// ReportingTrackingCorrectedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingTrackingCorrectedWorklistDescription")]
	public class ReportingTrackingCorrectedWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.Report.Status.EqualTo(ReportStatus.C);
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(PublicationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ReportPartCompletedTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}
}
