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
    [WorklistClassDescription("ReportingToBeProtocolledWorklistDescription")]
    public class ReportingToBeProtocolledWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof (ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new ReportingWorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingDraftProtocolWorklistDescription")]
    public class ReportingDraftProtocolWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingCompletedProtocolWorklistDescription")]
    public class ReportingCompletedProtocolWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingRejectedProtocolWorklistDescription")]
    public class ReportingRejectedProtocolWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingSuspendedProtocolWorklistDescription")]
    public class ReportingSuspendedProtocolWorklist : ProtocolingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.SU);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, null, WorklistOrdering.PrioritizeNewestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
