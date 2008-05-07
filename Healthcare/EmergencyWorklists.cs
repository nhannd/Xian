using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	[WorklistCategory("WorklistCategoryEmergency")]
	public abstract class EmergencyWorklist : RegistrationWorklist
	{
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("EmergencyOrdersWorklistDescription")]
	[WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
	public class EmergencyOrdersWorklist : EmergencyWorklist
	{
		public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
		{
			RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();

			ApplyTimeCriteria(criteria, WorklistTimeField.OrderSchedulingRequestTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}
}
