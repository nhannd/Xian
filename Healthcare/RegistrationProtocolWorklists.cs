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
    [WorklistClassDescription("RegistrationPendingProtocolWorklistDescription")]
    public class RegistrationPendingProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ProcedureStepClass = typeof (ProtocolAssignmentStep);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationAsapPendingProtocolWorklist entity 
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistClassDescription("RegistrationAsapPendingProtocolWorklistDescription")]
    public class RegistrationAsapPendingProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);

            // any procedures with pending protocol assignment, where the procedure scheduled start time is filtered
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureScheduledStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationRejectedProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistClassDescription("RegistrationRejectedProtocolWorklistDescription")]
    public class RegistrationRejectedProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolResolutionStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationSuspendedProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistClassDescription("RegistrationSuspendedProtocolWorklistDescription")]
    public class RegistrationSuspendedProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolResolutionStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.Protocol.Status.EqualTo(ProtocolStatus.SU);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationCompletedProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistClassDescription("RegistrationCompletedProtocolWorklistDescription")]
    public class RegistrationCompletedProtocolWorklist : RegistrationProtocolWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);

            // only unscheduled procedures should be in this list
            criteria.Procedure.ScheduledStartTime.IsNull();
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, null, WorklistOrdering.PrioritizeNewestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
