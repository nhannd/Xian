using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
namespace ClearCanvas.ImageViewer.Layout.Basic
{
	//TODO: at some point in the future, expand to a full blown auto reconciler that just wraps the Ris' reconciliation service.

	[Cloneable(true)]
	public class PatientInformation
	{
		private string _patientId;
		//private string _patientsName;
		//private string _patientsBirthDate;
		//private string _patientsBirthTime;
		//private string _patientsSex;

		internal PatientInformation()
		{
		}

		internal PatientInformation(Patient patient)
		{
			_patientId = patient.PatientId;
			//_patientsName = patient.PatientsName;
			//_patientsSex = patient.PatientsSex;
		}

		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		//public string PatientsName
		//{
		//    get { return _patientsName; }
		//    set { _patientsName = value; }
		//}

		//public string PatientsBirthDate
		//{
		//    get { return _patientsBirthDate; }
		//    set { _patientsBirthDate = value; }
		//}

		//public string PatientsBirthTime
		//{
		//    get { return _patientsBirthTime; }
		//    set { _patientsBirthTime = value; }
		//}

		//public string PatientsSex
		//{
		//    get { return _patientsSex; }
		//    set { _patientsSex = value; }
		//}

		public PatientInformation Clone()
		{
			return CloneBuilder.Clone(this) as PatientInformation;
		}
	}
	
	internal interface IPatientAutoReconciliationService
	{
		PatientInformation ReconcilePatient(PatientInformation patient);
	}
}
