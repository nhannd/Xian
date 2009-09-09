
namespace ClearCanvas.Dicom.Iod
{
	public interface ISopInstanceData
	{
		/// <summary>
		/// Gets the Study Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid)]
		string StudyInstanceUid { get; }

		/// <summary>
		/// Gets the Series Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SeriesInstanceUid)]
		string SeriesInstanceUid { get; }

		/// <summary>
		/// Gets the Sop Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SopInstanceUid)]
		string SopInstanceUid { get; }

		/// <summary>
		/// Gets the Sop Class Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SopClassUid)]
		string SopClassUid { get; }

		/// <summary>
		/// Gets the Instance Number of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.InstanceNumber)]
		int InstanceNumber { get; }
	}
}