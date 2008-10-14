using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	//NOTE: internal for now because we don't actually implement IPatientRootQuery anywhere.
	
	[DataContract(Namespace = QueryNamespace.Value)]
	internal class PatientRootStudyIdentifier : StudyIdentifier
	{
		public PatientRootStudyIdentifier()
		{
		}

		public PatientRootStudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}
	}
}
