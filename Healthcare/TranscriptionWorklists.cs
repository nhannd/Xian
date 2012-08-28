#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	[WorklistProcedureTypeGroupClass(typeof(ReadingGroup))]
	[WorklistCategory("WorklistCategoryTranscription")]
	public abstract class TranscriptionWorklist : ReportingWorklist
	{
		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(TranscriptionStep) };
		}
	}

	/// <summary>
	/// TranscriptionToBeTranscribedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("TranscriptionToBeTranscribedWorklistDescription")]
	public class TranscriptionToBeTranscribedWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			return new[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems,
				true);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionDraftWorklistDescription")]
	public class TranscriptionDraftWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.IP });
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionToBeReviewedWorklistDescription")]
	public class TranscriptionToBeReviewedWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems,
				true);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionAwaitingReviewWorklistDescription")]
	public class TranscriptionAwaitingReviewWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.IP, ActivityStatus.SC });
			criteria.ProcedureStep.Scheduling.Performer.Staff.NotEqualTo(wqc.ExecutingStaff);
			criteria.ReportPart.Transcriber.EqualTo(wqc.ExecutingStaff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionCompletedWorklistDescription")]
	public class TranscriptionCompletedWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.ExecutingStaff);

			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepEndTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}
}