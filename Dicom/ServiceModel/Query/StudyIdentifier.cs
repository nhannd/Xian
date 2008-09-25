using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	[DataContract(Namespace = QueryNamespace.Value)]
	public class StudyIdentifier : Identifier
	{
		#region Private Fields

		private string _studyInstanceUid;
		private string[] _modalitiesInStudy;
		private string _studyDescription;
		private string _studyId;
		private string _studyDate;
		private string _studyTime;
		private string _accessionNumber;
		private int? _numberOfStudyRelatedSeries;
		private int? _numberOfStudyRelatedInstances;

		#endregion

		#region Public Constructors

		public StudyIdentifier()
		{
		}

		public StudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		#region Public Properties

		public override string QueryRetrieveLevel
		{
			get { return "STUDY"; }
		}

		[DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DicomField(DicomTags.ModalitiesInStudy, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string[] ModalitiesInStudy
		{
			get { return _modalitiesInStudy; }
			set { _modalitiesInStudy = value; }
		}

		[DicomField(DicomTags.StudyDescription, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		[DicomField(DicomTags.StudyId, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string StudyId
		{
			get { return _studyId; }
			set { _studyId = value; }
		}

		[DicomField(DicomTags.StudyDate, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string StudyDate
		{
			get { return _studyDate; }
			set { _studyDate = value; }
		}

		[DicomField(DicomTags.StudyTime, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string StudyTime
		{
			get { return _studyTime; }
			set { _studyTime = value; }
		}

		[DicomField(DicomTags.AccessionNumber, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value; }
		}

		[DicomField(DicomTags.NumberOfStudyRelatedSeries, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public int? NumberOfStudyRelatedSeries
		{
			get { return _numberOfStudyRelatedSeries; }
			set { _numberOfStudyRelatedSeries = value; }
		}

		[DicomField(DicomTags.NumberOfStudyRelatedInstances, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public int? NumberOfStudyRelatedInstances
		{
			get { return _numberOfStudyRelatedInstances; }
			set { _numberOfStudyRelatedInstances = value; }
		}

		#endregion
	}
}
