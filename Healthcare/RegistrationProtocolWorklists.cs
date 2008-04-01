using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
    [WorklistCategory("WorklistCategoryBooking")]
    public abstract class RegistrationProtocolWorklist : RegistrationWorklist
    {
    }

    /// <summary>
    /// RegistrationPendingProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    public class RegistrationPendingProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });

            ApplyTimeRange(criteria.ProcedureStep.Scheduling.StartTime, null);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolAssignmentStep); }
        }
    }

    /// <summary>
    /// RegistrationAsapPendingProtocolWorklist entity 
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    public class RegistrationAsapPendingProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });

            // any procedures with pending protocol assignment, where the procedure scheduled start time is filtered
            ApplyTimeRange(criteria.Procedure.ScheduledStartTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolAssignmentStep); }
        }
    }

    /// <summary>
    /// RegistrationRejectedProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    public class RegistrationRejectedProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolResolutionStep); }
        }
    }

    /// <summary>
    /// RegistrationSuspendedProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    public class RegistrationSuspendedProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.SU);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolResolutionStep); }
        }
    }

    /// <summary>
    /// RegistrationCompletedProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    public class RegistrationCompletedProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);

            // no time bounds, but only unscheduled procedures should be in this list
            criteria.Procedure.ScheduledStartTime.IsNull();     
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override Type ProcedureStepType
        {
            get { return typeof(ProtocolAssignmentStep); }
        }
    }
}
