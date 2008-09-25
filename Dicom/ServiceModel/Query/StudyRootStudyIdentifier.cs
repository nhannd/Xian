using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
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

		public StudyRootStudyIdentifier()
		{
		}

		public StudyRootStudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		#region Public Properties

		[DicomField(DicomTags.PatientId, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		[DicomField(DicomTags.PatientsName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		[DicomField(DicomTags.PatientsBirthDate, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsBirthDate
		{
			get { return _patientsBirthDate; }
			set { _patientsBirthDate = value; }
		}

		[DicomField(DicomTags.PatientsBirthTime, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsBirthTime
		{
			get { return _patientsBirthTime; }
			set { _patientsBirthTime = value; }
		}

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
