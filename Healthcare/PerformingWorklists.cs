using System.Collections;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
	[WorklistCategory("WorklistCategoryPerforming")]
	public abstract class PerformingWorklist : Worklist
    {
        public override IList GetWorklistItems(IWorklistQueryContext wqc)
        {
            return (IList)wqc.GetBroker<IModalityWorklistItemBroker>().GetWorklistItems(this, wqc);
        }

        public override int GetWorklistItemCount(IWorklistQueryContext wqc)
        {
            return wqc.GetBroker<IModalityWorklistItemBroker>().CountWorklistItems(this, wqc);
        }
    }
	
    /// <summary>
	/// PerformingScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingScheduledWorklistDescription")]
	public class PerformingScheduledWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureCheckIn.CheckInTime.IsNull(); // not checked in
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingWorklistCheckedInWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingWorklistCheckedInWorklistDescription")]
	public class PerformingCheckedInWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureCheckIn.CheckInTime.IsNotNull(); // checked-in
            criteria.ProcedureCheckIn.CheckOutTime.IsNull(); // but not checked-out
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);    // and not started
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureCheckInTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingWorklistInProgessWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingWorklistInProgressWorklistDescription")]
	public class PerformingInProgressWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingWorklistCompletedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingWorklistCompletedWorklistDescription")]
	public class PerformingCompletedWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingCancelledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingCancelledWorklistDescription")]
	public class PerformingCancelledWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingUndocumentedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingUndocumentedWorklistDescription")]
	public class PerformingUndocumentedWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(DocumentationProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
