using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	[WorklistProcedureTypeGroupClass(typeof(ReadingGroup))]
	[WorklistCategory("WorklistCategoryTranscription")]
	public abstract class TranscriptionWorklist : ReportingWorklist
	{
	}

	/// <summary>
	/// TranscriptionToBeTranscribedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[WorklistClassDescription("TranscriptionToBeTranscribedWorklistDescription")]
	public class TranscriptionToBeTranscribedWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(TranscriptionStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionDraftWorklistDescription")]
	public class TranscriptionDraftWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(TranscriptionStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.IP });
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionToBeReviewedWorklistDescription")]
	public class TranscriptionToBeReviewedWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(TranscriptionStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionAwaitingReviewWorklistDescription")]
	public class TranscriptionAwaitingReviewWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(TranscriptionStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.IP, ActivityStatus.SC });
			criteria.ProcedureStep.Scheduling.Performer.Staff.NotEqualTo(wqc.Staff);
			criteria.ReportPart.Transcriber.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionCompletedWorklistDescription")]
	public class TranscriptionCompletedWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(TranscriptionStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);

			return new WorklistItemSearchCriteria[] { criteria };
		}
	}
}