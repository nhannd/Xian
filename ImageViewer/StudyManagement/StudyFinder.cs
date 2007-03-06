using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A list of <see cref="StudyItem"/> objects.
	/// </summary>
	public class StudyItemList : List<StudyItem>
	{
		public StudyItemList()
		{
		}
	}

	/// <summary>
	/// A study item.
	/// </summary>
	public class StudyItem
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
		public StudyItem()
		{
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
		/// Gets or sets the study date.
		/// </summary>
        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

		/// <summary>
		/// Gets or sets the patient ID.
		/// </summary>
		public string PatientId
		{
			get { return _patientID; }
			set { _patientID = value; }
		}

		/// <summary>
		/// Gets or sets the patient's name.
		/// </summary>
        public PersonName PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

		/// <summary>
		/// Gets or sets the modalities in the study.
		/// </summary>
        public string ModalitiesInStudy
        {
            get { return _modalitiesInStudy; }
            set { _modalitiesInStudy = value; }
        }

		/// <summary>
		/// Gets or sets the Study Instance UID.
		/// </summary>
		public string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
			set { _studyInstanceUID = value; }
		}

		/// <summary>
		/// Gets or sets the study loader name.
		/// </summary>
		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
			set { _studyLoaderName = value; }
		}

		/// <summary>
		/// Gets or sets the number of study related instances.
		/// </summary>
        public uint NumberOfStudyRelatedInstances
        {
            get { return _numberOfStudyRelatedInstances; }
            set { _numberOfStudyRelatedInstances = value; }
        }

		/// <summary>
		/// Gets or sets the specific character set.
		/// </summary>
        public string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set { _specificCharacterSet = value; }
        }

		/// <summary>
		/// Gets or sets the server.
		/// </summary>
        public ApplicationEntity Server
        {
            get { return _server; }
            set { _server = value; }
        }

        #region Private Members
        private string _patientID;
        private string _patientsBirthDate;
        private string _accessionNumber;
        private string _studyDescription;
        private string _studyDate;
        private string _studyInstanceUID;
        private string _studyLoaderName;
        private string _modalitiesInStudy;
        private uint _numberOfStudyRelatedInstances;
        private string _specificCharacterSet;
        private ApplicationEntity _server;
        private PersonName _patientsName;
        #endregion
	}

	/// <summary>
	/// A map of query parameters.
	/// </summary>
	public class QueryParameters : Dictionary<string,string>
	{
		public QueryParameters()
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class StudyFinder : IStudyFinder
	{
		public StudyFinder()
		{

		}

        public abstract string Name
		{
			get;
		}

		public abstract StudyItemList Query(QueryParameters queryParams);
        public abstract StudyItemList Query<T>(T targetServerObject, QueryParameters queryParams);

    }
}
