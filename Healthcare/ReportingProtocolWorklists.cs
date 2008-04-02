using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
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
    public class ReportingToBeProtocolledWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof (ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.OldestItemsFirst);
            return new ReportingWorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistSingleton(true)]
    public class ReportingDraftProtocolWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.OldestItemsFirst);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistSingleton(true)]
    public class ReportingCompletedProtocolWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.NewestItemsFirst);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistSingleton(true)]
    public class ReportingRejectedProtocolWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.NewestItemsFirst);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistSingleton(true)]
    public class ReportingSuspendedProtocolWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.SU);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, null, WorklistOrdering.NewestItemsFirst);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
