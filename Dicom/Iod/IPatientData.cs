
namespace ClearCanvas.Dicom.Iod
{
	//internal for now b/c the patient root query stuff is
	internal interface IPatientRootData : IPatientData
	{
		[DicomField(DicomTags.NumberOfPatientRelatedStudies)]
		int? NumberOfPatientRelatedStudies { get; }

		[DicomField(DicomTags.NumberOfPatientRelatedSeries)]
		int? NumberOfPatientRelatedSeries { get; }

		[DicomField(DicomTags.NumberOfPatientRelatedInstances)]
		int? NumberOfPatientRelatedInstances { get; }
	}

	public interface IPatientData
	{
		[DicomField(DicomTags.PatientId)]
		string PatientId { get; }

		[DicomField(DicomTags.PatientsName)]
		string PatientsName { get; }

		[DicomField(DicomTags.PatientsBirthDate)]
		string PatientsBirthDate { get; }

		[DicomField(DicomTags.PatientsBirthTime)]
		string PatientsBirthTime { get; }

		[DicomField(DicomTags.PatientsSex)]
		string PatientsSex { get; }
	}
}