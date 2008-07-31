using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	[WorklistProcedureTypeGroupClass(typeof(ReadingGroup))]
	[WorklistCategory("WorklistCategoryReporting")]
	public abstract class ReportingWorklist : Worklist
	{
		public override IList GetWorklistItems(IWorklistQueryContext wqc)
		{
			return (IList)wqc.GetBroker<IReportingWorklistItemBroker>().GetWorklistItems(this, wqc);
		}

		public override int GetWorklistItemCount(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IReportingWorklistItemBroker>().CountWorklistItems(this, wqc);
		}
	}

	/// <summary>
	/// ReportingToBeReportedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[WorklistClassDescription("ReportingToBeReportedWorklistDescription")]
	public class ReportingToBeReportedWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(InterpretationStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// ReportingAssignedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedWorklistDescription")]
	public class ReportingAssignedWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(InterpretationStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// ReportingToBeReviewedReportWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[WorklistClassDescription("ReportingToBeReviewedReportWorklistDescription")]
	public class ReportingToBeReviewedReportWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(VerificationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });

			criteria.ReportPart.Interpreter.NotEqualTo(wqc.Staff);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.ReportPart.Supervisor.IsNull();

			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// ReportingAssignedReviewWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedReviewWorklistDescription")]
	public class ReportingAssignedReviewWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria assignedToMe = BaseCriteria();
			assignedToMe.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);

			ReportingWorklistItemSearchCriteria bySupervisor = BaseCriteria();
			bySupervisor.ReportPart.Supervisor.EqualTo(wqc.Staff);

			ApplyTimeCriteria(assignedToMe, WorklistTimeField.ProcedureStepScheduledStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);

			return new ReportingWorklistItemSearchCriteria[] { assignedToMe, bySupervisor };
		}

		private ReportingWorklistItemSearchCriteria BaseCriteria()
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(VerificationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			return criteria;
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingDraftWorklistDescription")]
	public class ReportingDraftWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(InterpretationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingInTranscriptionWorklistDescription")]
	public class ReportingInTranscriptionWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(TranscriptionStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAwaitingReviewWorklistDescription")]
	public class ReportingAwaitingReviewWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(VerificationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.NotEqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	public abstract class ReportingVerifiedWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(PublicationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
			GetStaffSearchCriteria(criteria).EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeNewestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected abstract StaffSearchCriteria GetStaffSearchCriteria(ReportingWorklistItemSearchCriteria criteria);
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingRadiologistVerifiedWorklistDescription")]
	public class ReportingRadiologistVerifiedWorklist : ReportingVerifiedWorklist
	{
		protected override StaffSearchCriteria GetStaffSearchCriteria(ReportingWorklistItemSearchCriteria criteria)
		{
			return criteria.ReportPart.Verifier;
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingResidentVerifiedWorklistDescription")]
	public class ReportingResidentVerifiedWorklist : ReportingVerifiedWorklist
	{
		protected override StaffSearchCriteria GetStaffSearchCriteria(ReportingWorklistItemSearchCriteria criteria)
		{
			return criteria.ReportPart.Interpreter;
		}
	}

}
