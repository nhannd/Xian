using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
    [WorklistCategory("WorklistCategoryTechnologist")]
    public abstract class TechnologistWorklist : Worklist
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
    /// TechnologistScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistClassDescription("TechnologistScheduledWorklistDescription")]
    public class TechnologistScheduledWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureCheckIn.CheckInTime.IsNull(); // not checked in
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistCheckedInWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistClassDescription("TechnologistCheckedInWorklistDescription")]
    public class TechnologistCheckedInWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureCheckIn.CheckInTime.IsNotNull(); // checked-in
            criteria.ProcedureCheckIn.CheckOutTime.IsNull(); // but not checked-out
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);    // and not started
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureCheckInTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistInProgessWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistClassDescription("TechnologistInProgressWorklistDescription")]
    public class TechnologistInProgressWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistCompletedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistClassDescription("TechnologistCompletedWorklistDescription")]
    public class TechnologistCompletedWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistCancelledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistClassDescription("TechnologistCancelledWorklistDescription")]
    public class TechnologistCancelledWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistUndocumentedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistClassDescription("TechnologistUndocumentedWorklistDescription")]
    public class TechnologistUndocumentedWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(DocumentationProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
