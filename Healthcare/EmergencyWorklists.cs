using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// EmergencyScheduledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyScheduledWorklistDescription")]
	public class EmergencyScheduledWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			// this is slightly different than the registration scheduled worklist, because we include
			// 'checked in' items here, rather than having a separate 'checked in' worklist
			RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);
			//criteria.Order.Status.EqualTo(OrderStatus.SC);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureScheduledStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// EmergencyInProgressWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyInProgressWorklistDescription")]
	public class EmergencyInProgressWorklist : RegistrationInProgressWorklist
	{
	}

	/// <summary>
	/// EmergencyCompletedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyCompletedWorklistDescription")]
	public class EmergencyCompletedWorklist : RegistrationCompletedWorklist
	{
	}

	/// <summary>
	/// EmergencyCancelledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyCancelledWorklistDescription")]
	public class EmergencyCancelledWorklist : RegistrationCancelledWorklist
	{
	}
}
