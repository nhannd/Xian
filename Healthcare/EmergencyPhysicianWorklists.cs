using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

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
		// These should not be hard-coded for these soft enums.
		private readonly string EmergencyPatientClassCode = "E";
		private readonly string EmergencyPatientTypeCode = "E";
		private readonly string EmergencyAdmissionTypeCode = "E";

		public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
		{
			RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();

			try
			{
				PatientClassEnum emergePatientClass = wqc.GetBroker<IEnumBroker>().Find<PatientClassEnum>(EmergencyPatientClassCode);
				criteria.Order.Visit.PatientClass.EqualTo(emergePatientClass);

				// TODO: it's not entirely obvious which of these is correct

				//PatientTypeEnum emergePatientType = wqc.GetBroker<IEnumBroker>().Find<PatientTypeEnum>(EmergencyPatientTypeCode);
				//criteria.Order.Visit.PatientType.EqualTo(emergePatientType);

				//AdmissionTypeEnum emergeAdmissionType = wqc.GetBroker<IEnumBroker>().Find<AdmissionTypeEnum>(EmergencyAdmissionTypeCode);
				//criteria.Order.Visit.AdmissionType.EqualTo(emergeAdmissionType);
			}
			catch(EnumValueNotFoundException e)
			{
				Platform.Log(LogLevel.Warn, e, "Unable to find code {0} for PatientClassEnum", EmergencyPatientClassCode);
			}

			WorklistTimeRange lastTwoDays = new WorklistTimeRange(
				new WorklistTimePoint(new TimeSpan(-2, 0, 0, 0), WorklistTimePoint.Resolutions.Day),
				WorklistTimePoint.Today);
			ApplyTimeCriteria(criteria, WorklistTimeField.OrderSchedulingRequestTime, lastTwoDays, WorklistOrdering.PrioritizeOldestItems);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}
}
