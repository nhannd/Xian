using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	//Note: the patient and study root study information are actually different, which is why this
	//class exists - to leave room for them to differ in the future.
	[DataContract(Namespace = QueryNamespace.Value)]
	public class StudyRootStudyIdentifier : StudyIdentifier
	{
		public StudyRootStudyIdentifier()
		{
		}

		public StudyRootStudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}
	}
}
