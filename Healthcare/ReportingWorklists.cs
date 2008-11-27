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

            ApplyTimeCriteria(assignedToMe, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);

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
			criteria.ReportPart.Interpreter.EqualTo(wqc.Staff);
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
			ReportingWorklistItemSearchCriteria criteriaNotEqual = new ReportingWorklistItemSearchCriteria();
			criteriaNotEqual.ProcedureStepClass = typeof(VerificationStep);
			criteriaNotEqual.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteriaNotEqual.ReportPart.Interpreter.EqualTo(wqc.Staff);
			criteriaNotEqual.ProcedureStep.Scheduling.Performer.Staff.NotEqualTo(wqc.Staff);
			ApplyTimeCriteria(criteriaNotEqual, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);

			ReportingWorklistItemSearchCriteria criteriaNull = new ReportingWorklistItemSearchCriteria();
			criteriaNull.ProcedureStepClass = typeof(VerificationStep);
			criteriaNull.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteriaNull.ReportPart.Interpreter.EqualTo(wqc.Staff);
			criteriaNull.ProcedureStep.Scheduling.Performer.Staff.IsNull();

			return new WorklistItemSearchCriteria[] { criteriaNotEqual, criteriaNull };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingVerifiedWorklistDescription")]
	public class ReportingVerifiedWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria unsupervised = new ReportingWorklistItemSearchCriteria();
			unsupervised.ProcedureStepClass = typeof(PublicationStep);
			unsupervised.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
			unsupervised.ReportPart.Verifier.EqualTo(wqc.Staff);
			ApplyTimeCriteria(unsupervised, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeNewestItems, wqc);

			ReportingWorklistItemSearchCriteria supervised = new ReportingWorklistItemSearchCriteria();
			supervised.ProcedureStepClass = typeof(PublicationStep);
			supervised.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
			supervised.ReportPart.Verifier.NotEqualTo(wqc.Staff);
			supervised.ReportPart.Interpreter.EqualTo(wqc.Staff);

			return new WorklistItemSearchCriteria[] { unsupervised, supervised };
		}
	}
}
