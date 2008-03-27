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
    /// ReportingToBeProtocolledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingToBeProtocolledWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class ReportingToBeProtocolledWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
            ApplyTimeRange(criteria.ProcedureStep.Scheduling.StartTime, null);
            return new ReportingWorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolAssignmentStep); }
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingDraftProtocolWorklist")]
    [WorklistSupportsTimeFilter(false)]
    public class ReportingDraftProtocolWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolAssignmentStep); }
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingCompletedProtocolWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class ReportingCompletedProtocolWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeRange(criteria.ProcedureStep.EndTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolAssignmentStep); }
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingRejectedProtocolWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class ReportingRejectedProtocolWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeRange(criteria.ProcedureStep.EndTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolAssignmentStep); }
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingSuspendedProtocolWorklist")]
    [WorklistSupportsTimeFilter(false)]
    public class ReportingSuspendedProtocolWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.SU);
            criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.Staff);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolAssignmentStep); }
        }
    }
}
