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
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ProcedureStepClass = typeof (ProtocolAssignmentStep);
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);	//bug #3498: exclude procedures that are no longer in SC status 
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
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
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);	//bug #3498: exclude procedures that are no longer in SC status 

            // any procedures with pending protocol assignment, where the procedure scheduled start time is filtered
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureScheduledStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems, wqc);
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
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolResolutionStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
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
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);

            // only unscheduled procedures should be in this list
            criteria.Procedure.ScheduledStartTime.IsNull();
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, null, WorklistOrdering.PrioritizeNewestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
