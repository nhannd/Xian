using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// Query identifier for a study.
	/// </summary>
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

		/// <summary>
		/// Default constructor.
		/// </summary>
		public StudyIdentifier()
		{
		}

		/// <summary>
		/// Creates an instance of <see cref="StudyIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
		/// </summary>
		public StudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the level of the query - STUDY.
		/// </summary>
		public override string QueryRetrieveLevel
		{
			get { return "STUDY"; }
		}

		/// <summary>
		/// Gets or sets the Study Instance Uid of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the modalities in the identified study.
		/// </summary>
		[DicomField(DicomTags.ModalitiesInStudy, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string[] ModalitiesInStudy
		{
			get { return _modalitiesInStudy; }
			set { _modalitiesInStudy = value; }
		}

		/// <summary>
		/// Gets or sets the study description of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyDescription, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		/// <summary>
		/// Gets or sets the study id of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyId, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string StudyId
		{
			get { return _studyId; }
			set { _studyId = value; }
		}

		/// <summary>
		/// Gets or sets the study date of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyDate, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string StudyDate
		{
			get { return _studyDate; }
			set { _studyDate = value; }
		}

		/// <summary>
		/// Gets or sets the study time of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyTime, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string StudyTime
		{
			get { return _studyTime; }
			set { _studyTime = value; }
		}

		/// <summary>
		/// Gets or sets the accession number of the identified study.
		/// </summary>
		[DicomField(DicomTags.AccessionNumber, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value; }
		}

		/// <summary>
		/// Gets or sets the number of series belonging to the identified study.
		/// </summary>
		[DicomField(DicomTags.NumberOfStudyRelatedSeries, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public int? NumberOfStudyRelatedSeries
		{
			get { return _numberOfStudyRelatedSeries; }
			set { _numberOfStudyRelatedSeries = value; }
		}

		/// <summary>
		/// Gets or sets the number of composite object instances belonging to the identified study.
		/// </summary>
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
