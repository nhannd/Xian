#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A study item.
	/// </summary>
	public class StudyItem : IStudyRootStudyIdentifier
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
		[Obsolete("Use StudyItem(string, object, string) instead.")]
		public StudyItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
		public StudyItem(StudyItem other)
			: this(other, other.Server, other.StudyLoaderName)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
		public StudyItem(string studyInstanceUid, object server, string studyLoaderName)
			: this(server, studyLoaderName)
		{
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");

			_studyInstanceUid = studyInstanceUid;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
		public StudyItem(IStudyRootData other, object server, string studyLoaderName)
			: this(other, other, server, studyLoaderName)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
		public StudyItem(IPatientData patient, IStudyData study, object server, string studyLoaderName)
			: this(server, studyLoaderName)
		{
			Platform.CheckForNullReference(patient, "patient");
			Platform.CheckForNullReference(study, "study");

			Platform.CheckForEmptyString(study.StudyInstanceUid, "study.StudyInstanceUid");

			CopyFrom(patient);
			CopyFrom(study);
		}

		private StudyItem(object server, string studyLoaderName)
		{
			_server = server;
			_studyLoaderName = studyLoaderName;
		}

		private void CopyFrom(IStudyData other)
		{
			this.ReferringPhysiciansName = new PersonName(other.ReferringPhysiciansName);
			AccessionNumber = other.AccessionNumber;
			StudyDescription = other.StudyDescription;
			StudyId = other.StudyId;
			StudyDate = other.StudyDate;
			StudyTime = other.StudyTime;
			ModalitiesInStudy = other.ModalitiesInStudy;
			StudyInstanceUid = other.StudyInstanceUid;
			NumberOfStudyRelatedSeries = other.NumberOfStudyRelatedSeries;
			NumberOfStudyRelatedInstances = other.NumberOfStudyRelatedInstances;
		}

		private void CopyFrom(IPatientData other)
		{
			PatientId = other.PatientId;
			PatientsName = new PersonName(other.PatientsName);
			PatientsBirthDate = other.PatientsBirthDate;
			PatientsBirthTime = other.PatientsBirthTime;
			PatientsSex = other.PatientsSex;

			PatientSpeciesDescription = other.PatientSpeciesDescription;
			PatientSpeciesCodeSequenceCodingSchemeDesignator = other.PatientSpeciesCodeSequenceCodingSchemeDesignator;
			PatientSpeciesCodeSequenceCodeValue = other.PatientSpeciesCodeSequenceCodeValue;
			PatientSpeciesCodeSequenceCodeMeaning = other.PatientSpeciesCodeSequenceCodeMeaning;
			PatientBreedDescription = other.PatientBreedDescription;
			PatientBreedCodeSequenceCodingSchemeDesignator = other.PatientBreedCodeSequenceCodingSchemeDesignator;
			PatientBreedCodeSequenceCodeValue = other.PatientBreedCodeSequenceCodeValue;
			PatientBreedCodeSequenceCodeMeaning = other.PatientBreedCodeSequenceCodeMeaning;
			ResponsiblePerson = new PersonName(other.ResponsiblePerson);
			ResponsiblePersonRole = other.ResponsiblePersonRole;
			ResponsibleOrganization = other.ResponsibleOrganization;
		}

		#region IPatientData Members

		/// <summary>
		/// Gets or sets the patient ID.
		/// </summary>
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		/// <summary>
		/// Gets or sets the patient's name.
		/// </summary>
		public PersonName PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		string IPatientData.PatientsName
		{
			get { return (_patientsName ?? ""); }
		}

		/// <summary>
		/// Gets or sets the patient's birthdate.
		/// </summary>
		public string PatientsBirthDate
		{
			get { return _patientsBirthDate; }
			set { _patientsBirthDate = value; }
		}

		/// <summary>
		/// Gets or sets the patient's birthtime.
		/// </summary>
		public string PatientsBirthTime
		{
			get { return _patientsBirthTime; }
			set { _patientsBirthTime = value; }
		}

		/// <summary>
		/// Gets or sets the patient's sex.
		/// </summary>
		public string PatientsSex
		{
			get { return _patientsSex; }
			set { _patientsSex = value; }
		}

		#region Species

		/// <summary>
		/// Gets the patient's species description.
		/// </summary>
		public string PatientSpeciesDescription
		{
			get { return _patientSpeciesDescription; }
			set { _patientSpeciesDescription = value; }
		}

		/// <summary>
		/// Gets the coding scheme designator of the patient species code sequence.
		/// </summary>
		public string PatientSpeciesCodeSequenceCodingSchemeDesignator
		{
			get { return _patientSpeciesCodeSequenceCodingSchemeDesignator; }
			set { _patientSpeciesCodeSequenceCodingSchemeDesignator = value; }
		}

		/// <summary>
		/// Gets the code value of the patient species code sequence.
		/// </summary>
		public string PatientSpeciesCodeSequenceCodeValue
		{
			get { return _patientSpeciesCodeSequenceCodeValue; }
			set { _patientSpeciesCodeSequenceCodeValue = value; }
		}

		/// <summary>
		/// Gets the code meaning of the patient species code sequence.
		/// </summary>
		public string PatientSpeciesCodeSequenceCodeMeaning
		{
			get { return _patientSpeciesCodeSequenceCodeMeaning; }
			set { _patientSpeciesCodeSequenceCodeMeaning = value; }
		}

		#endregion

		#region Breed

		/// <summary>
		/// Gets the patient's breed description.
		/// </summary>
		public string PatientBreedDescription
		{
			get { return _patientBreedDescription; }
			set { _patientBreedDescription = value; }
		}

		/// <summary>
		/// Gets the coding scheme designator of the patient breed code sequence.
		/// </summary>
		public string PatientBreedCodeSequenceCodingSchemeDesignator
		{
			get { return _patientBreedCodeSequenceCodingSchemeDesignator; }
			set { _patientBreedCodeSequenceCodingSchemeDesignator = value; }
		}

		/// <summary>
		/// Gets the code value of the patient breed code sequence.
		/// </summary>
		public string PatientBreedCodeSequenceCodeValue
		{
			get { return _patientBreedCodeSequenceCodeValue; }
			set { _patientBreedCodeSequenceCodeValue = value; }
		}

		/// <summary>
		/// Gets the code meaning of the patient breed code sequence.
		/// </summary>
		public string PatientBreedCodeSequenceCodeMeaning
		{
			get { return _patientBreedCodeSequenceCodeMeaning; }
			set { _patientBreedCodeSequenceCodeMeaning = value; }
		}

		#endregion

		#region Responsible Person/Organization

		/// <summary>
		/// Gets the responsible person for this patient.
		/// </summary>
		public PersonName ResponsiblePerson
		{
			get { return _responsiblePerson; }
			set { _responsiblePerson = value; }
		}

		string IPatientData.ResponsiblePerson
		{
			get { return (_responsiblePerson ?? ""); }
		}

		/// <summary>
		/// Gets the role of the responsible person for this patient.
		/// </summary>
		public string ResponsiblePersonRole
		{
			get { return _responsiblePersonRole; }
			set { _responsiblePersonRole = value; }
		}

		/// <summary>
		/// Gets the organization responsible for this patient.
		/// </summary>
		public string ResponsibleOrganization
		{
			get { return _responsibleOrganization; }
			set { _responsibleOrganization = value; }
		}

		#endregion

		#endregion

		#region IStudyData Members

		/// <summary>
		/// Gets or sets the referring physician's name.
		/// </summary>
		public PersonName ReferringPhysiciansName
		{
			get { return _referringPhysiciansName; }
			set { _referringPhysiciansName = value; }
		}

		string IStudyData.ReferringPhysiciansName
		{
			get { return _referringPhysiciansName; }
		}
		
		/// <summary>
		/// Gets or sets the patient's accession number.
		/// </summary>
		public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

		/// <summary>
		/// Gets or sets the study description.
		/// </summary>
		public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

		/// <summary>
		/// Gets or sets the study ID.
		/// </summary>
		public string StudyId
		{
			get { return _studyId; }
			set { _studyId = value; }
		}

		/// <summary>
		/// Gets or sets the study date.
		/// </summary>
		public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

		/// <summary>
		/// Gets or sets the study time.
		/// </summary>
		public string StudyTime
		{
			get { return _studyTime; }
			set { _studyTime = value; }
		}

		/// <summary>
		/// Gets or sets the modalities in the study.
		/// </summary>
		public string[] ModalitiesInStudy
		{
			get { return _modalitiesInStudy ?? new string[0]; }
			set { _modalitiesInStudy = value ?? new string[0]; }
		}

		/// <summary>
		/// Gets or sets the Study Instance UID.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the number of series belonging to the study.
		/// </summary>
		public int? NumberOfStudyRelatedSeries
		{
			get { return _numberOfStudyRelatedSeries; }
			set { _numberOfStudyRelatedSeries = value; }
		}

		/// <summary>
		/// Gets or sets the number of study related instances.
		/// </summary>
		public int? NumberOfStudyRelatedInstances
        {
            get { return _numberOfStudyRelatedInstances; }
            set { _numberOfStudyRelatedInstances = value; }
        }

		#endregion

		#region IIdentifier Members

		/// <summary>
		/// Gets or sets the specific character set.
		/// </summary>
		public string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set { _specificCharacterSet = value; }
        }

		/// <summary>
		/// Gets or sets the Instance Availability of the study.
		/// </summary>
		public string InstanceAvailability
		{
			get { return _instanceAvailability; }
			set { _instanceAvailability = value; }
		}

		string IIdentifier.RetrieveAeTitle
		{
			get
			{
                if (_server != null && _server is IApplicationEntity)
					return ((IApplicationEntity)_server).AETitle;
				else
					return "";
			}
		}

		#endregion

	    // TODO (Marmot): Change to IDicomServiceNode
		/// <summary>
		/// Gets or sets the server.
		/// </summary>
		public object Server
		{
			get { return _server; }
			set { _server = value; }
		}

	    // TODO (Marmot): Get rid of this.
		/// <summary>
		/// Gets or sets the study loader name.
		/// </summary>
		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
			set { _studyLoaderName = value; }
		}

		/// <summary>
		/// Converts this <see cref="StudyItem"/> into a <see cref="StudyRootStudyIdentifier"/>.
		/// </summary>
		public StudyRootStudyIdentifier ToStudyRootIdentifier()
		{
			return new StudyRootStudyIdentifier(this);
		}

		/// <summary>
		/// Converts this <see cref="StudyItem"/> into a <see cref="StudyRootStudyIdentifier"/>.
		/// </summary>
		public static explicit operator StudyRootStudyIdentifier(StudyItem item)
		{
			return item.ToStudyRootIdentifier();
		}

		/// <summary>
		/// Returns the patient's name, patient ID and study date associated
		/// with the <see cref="StudyItem"/> in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			DateTime studyDate;
			DateParser.Parse(this.StudyDate, out studyDate);

			return String.Format("{0}; {1}; {2}",
				this.PatientsName,
				this.PatientId,
				studyDate.ToString(Format.DateFormat));
		}

        #region Private Members
        private string _patientId;
        private string _patientsBirthDate;
        private string _accessionNumber;
        private string _studyDescription;
		private string _studyId;
		private string _studyDate;
		private string _studyTime;
        private string _studyInstanceUid;
        private string _studyLoaderName;
        private string[] _modalitiesInStudy;
        private int? _numberOfStudyRelatedInstances;
		private int? _numberOfStudyRelatedSeries;
		private string _specificCharacterSet;
		private object _server;
		private string _instanceAvailability;
		private PersonName _patientsName;
		private string _patientsBirthTime;
		private string _patientsSex;
		private PersonName _referringPhysiciansName;

		private string _patientSpeciesDescription;
		private string _patientSpeciesCodeSequenceCodingSchemeDesignator;
		private string _patientSpeciesCodeSequenceCodeValue;
		private string _patientSpeciesCodeSequenceCodeMeaning;

		private string _patientBreedDescription;
		private string _patientBreedCodeSequenceCodingSchemeDesignator;
		private string _patientBreedCodeSequenceCodeValue;
		private string _patientBreedCodeSequenceCodeMeaning;

		private PersonName _responsiblePerson;
		private string _responsiblePersonRole;
		private string _responsibleOrganization;
		#endregion
	}

	/// <summary>
	/// A list of <see cref="StudyItem"/> objects.
	/// </summary>
	public class StudyItemList : List<StudyItem>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyItemList"/>.
		/// </summary>
		public StudyItemList()
		{
		}

		public StudyItemList(IEnumerable<StudyItem> items)
			: base(items)
		{
		}
	}

	/// <summary>
	/// A map of query parameters.
	/// </summary>
	public class QueryParameters : IDictionary<string, string>
	{
	    private readonly Dictionary<string, string> _parameters;

		/// <summary>
		/// Initializes a new instance of <see cref="QueryParameters"/>.
		/// </summary>
		public QueryParameters()
		{
		    _parameters = new Dictionary<string, string>();
            this["PatientsName"] = String.Empty;
            this["ReferringPhysiciansName"] = String.Empty;
            this["PatientId"] = String.Empty;
            this["AccessionNumber"] = String.Empty;
            this["StudyDescription"] = String.Empty;
            this["ModalitiesInStudy"] = String.Empty;
            this["StudyDate"] = String.Empty;
            this["StudyInstanceUid"] = String.Empty;
        }

        /// <summary>
        /// Creates a copy of <paramref name="other"/>.
        /// </summary>
        public QueryParameters(QueryParameters other)
        {
            _parameters = new Dictionary<string, string>();
            foreach (var queryParameter in other)
                _parameters.Add(queryParameter.Key, queryParameter.Value);
        }

        #region IDictionary<string,string> Members

        public string this[string key]
        {
            get
            {
                if (String.IsNullOrEmpty(key))
                    return null;

                string value;
                return !_parameters.TryGetValue(key, out value) ? null : value;
            }
            set
            {
                if (!_parameters.ContainsKey(key))
                    _parameters.Add(key, value);
                else
                    _parameters[key] = value;
            }
        }
        
        public void Add(string key, string value)
        {
            _parameters.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _parameters.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _parameters.Keys; }
        }

        public bool Remove(string key)
        {
            return _parameters.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _parameters.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return _parameters.Values; }
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> Members

        public void Clear()
        {
            _parameters.Clear();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            ((ICollection<KeyValuePair<string, string>>)_parameters).Add(item);
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string,string>>)_parameters).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, string>>)_parameters).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _parameters.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string, string>>) _parameters).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        #endregion
    }
}
