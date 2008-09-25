using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	[DataContract(Namespace = QueryNamespace.Value)]
	public class PatientRootStudyIdentifier : StudyIdentifier
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
