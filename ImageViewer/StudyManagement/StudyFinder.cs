using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class StudyItemList : List<StudyItem>
	{
		public StudyItemList()
		{
		}
	}

	public class StudyItem
	{
		public StudyItem()
		{
		}

        public string PatientsBirthDate
        {
            get { return _patientsBirthDate; }
            set { _patientsBirthDate = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

		public string PatientId
		{
			get { return _patientID; }
			set { _patientID = value; }
		}

        public PersonName PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public string ModalitiesInStudy
        {
            get { return _modalitiesInStudy; }
            set { _modalitiesInStudy = value; }
        }

		public string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
			set { _studyInstanceUID = value; }
		}

		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
			set { _studyLoaderName = value; }
		}

        public uint NumberOfStudyRelatedInstances
        {
            get { return _numberOfStudyRelatedInstances; }
            set { _numberOfStudyRelatedInstances = value; }
        }

        public string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set { _specificCharacterSet = value; }
        }

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

	public class QueryParameters : Dictionary<string,string>
	{
		public QueryParameters()
		{
		}
	}

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
