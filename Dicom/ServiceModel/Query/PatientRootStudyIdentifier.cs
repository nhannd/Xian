using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	//Note: the patient and study root study information are actually different, which is why this
	//class exists - to leave room for them to differ in the future.
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
