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

        public override Type ProcedureStepType
        {
            get { return typeof(ModalityProcedureStep); }
        }
    }

    /// <summary>
    /// TechnologistScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    public class TechnologistScheduledWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureCheckIn.CheckInTime.IsNull(); // not checked in
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            ApplyTimeRange(criteria.ProcedureStep.Scheduling.StartTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistCheckedInWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    public class TechnologistCheckedInWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureCheckIn.CheckInTime.IsNotNull(); // checked-in
            criteria.ProcedureCheckIn.CheckOutTime.IsNull(); // but not checked-out
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);    // and not started
            ApplyTimeRange(criteria.ProcedureStep.Scheduling.StartTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistInProgessWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    public class TechnologistInProgressWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            ApplyTimeRange(criteria.ProcedureStep.StartTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistCompletedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    public class TechnologistCompletedWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
            ApplyTimeRange(criteria.ProcedureStep.EndTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistCancelledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    public class TechnologistCancelledWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            ApplyTimeRange(criteria.ProcedureStep.EndTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// TechnologistUndocumentedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    public class TechnologistUndocumentedWorklist : TechnologistWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);

            // no time bounds
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(DocumentationProcedureStep); }
        }
    }
}
