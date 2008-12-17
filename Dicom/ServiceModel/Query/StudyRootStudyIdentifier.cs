using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// Study Root Query identifier for a study.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class StudyRootStudyIdentifier : StudyIdentifier
	{
		#region Private Fields

		private string _patientId;
		private string _patientsName;
		private string _patientsBirthDate;
		private string _patientsBirthTime;
		private string _patientsSex;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public StudyRootStudyIdentifier()
		{
		}

		/// <summary>
		/// Creates an instance of <see cref="StudyRootStudyIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
		/// </summary>
		public StudyRootStudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the patient id of the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientId, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		/// <summary>
		/// Gets or sets the patient's name for the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientsName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		/// <summary>
		/// Gets or sets the patient's birth date for the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientsBirthDate, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsBirthDate
		{
			get { return _patientsBirthDate; }
			set { _patientsBirthDate = value; }
		}

		/// <summary>
		/// Gets or sets the patient's birth time for the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientsBirthTime, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsBirthTime
		{
			get { return _patientsBirthTime; }
			set { _patientsBirthTime = value; }
		}

		/// <summary>
		/// Gets or sets the patient's sex for the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientsSex, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsSex
		{
			get { return _patientsSex; }
			set { _patientsSex = value; }
		}

		#endregion
	}
}
