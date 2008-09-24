
using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	[DataContract(Namespace = QueryNamespace.Value)]
	public class ImageIdentifier : Identifier
	{
		private string _studyInstanceUid;
		private string _seriesInstanceUid;
		private string _sopInstanceUid;
		private int? _instanceNumber;

		public ImageIdentifier()
		{
		}

		public ImageIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		public override string QueryRetrieveLevel
		{
			get { return "IMAGE"; }
		}

		[DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DicomField(DicomTags.SeriesInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value; }
		}

		[DicomField(DicomTags.SopInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SopInstanceUid
		{
			get { return _sopInstanceUid; }
			set { _sopInstanceUid = value; }
		}

		[DicomField(DicomTags.SopInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public int? InstanceNumber
		{
			get { return _instanceNumber; }
			set { _instanceNumber = value; }
		}
	}
}
