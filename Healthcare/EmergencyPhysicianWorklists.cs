using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	[WorklistCategory("WorklistCategoryEmergencyPhysician")]
	public abstract class EmergencyPhysicianWorklist : RegistrationWorklist
	{
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("EmergencyPhysicianEmergencyOrdersWorklistDescription")]
	public class EmergencyPhysicianEmergencyOrdersWorklist : EmergencyPhysicianWorklist
	{
		public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
		{
			RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
			//criteria.Order.Visit.PatientType.EqualTo();

			ApplyTimeCriteria(criteria, WorklistTimeField.OrderSchedulingRequestTime, null, WorklistOrdering.PrioritizeOldestItems);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}
}
