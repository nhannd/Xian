using System;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	/// <summary>
	/// Model used in study list grid control <see cref="Study"/>.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class StudySummary
	{
		#region Private members
		private ServerEntityKey _ref;
		private string _patientId;
		private string _patientName;
		private string _studyDate;
		private string _accessionNumber;
		private string _studyDescription;
		private int _numberOfRelatedSeries;
		private int _numberOfRelatedInstances;
		private StudyStatusEnum _studyStatusEnum;
		private string _modalitiesInStudy;
		private Model.Study _theStudy;
		private ServerPartition _thePartition;
		#endregion Private members


		#region Public Properties

		public ServerEntityKey Key
		{
			get { return _ref; }
			set { _ref = value; }
		}

		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		public string PatientsName
		{
			get { return _patientName; }
			set { _patientName = value; }
		}

		public string StudyDate
		{
			get { return _studyDate; }
			set { _studyDate = value; }
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

		public int NumberOfRelatedSeries
		{
			get { return _numberOfRelatedSeries; }
			set { _numberOfRelatedSeries = value; }
		}

		public int NumberOfRelatedInstances
		{
			get { return _numberOfRelatedInstances; }
			set { _numberOfRelatedInstances = value; }
		}

		public StudyStatusEnum StudyStatusEnum
		{
			get { return _studyStatusEnum; }
			set { _studyStatusEnum = value; }
		}

		public string StudyStatusEnumString
		{
			get { return _studyStatusEnum.Description; }
		}

		public string ModalitiesInStudy
		{
			get { return _modalitiesInStudy; }
			set { _modalitiesInStudy = value; }
		}

		public Study TheStudy
		{
			get { return _theStudy; }
			set { _theStudy = value; }
		}
		public ServerPartition ThePartition
		{
			get { return _thePartition; }
			set { _thePartition = value; }
		}
		#endregion Public Properties
	}

	/// <summary>
	/// Datasource for use with the ObjectDataSource to select a subset of results
	/// </summary>
	public class StudyDataSource
	{
		#region Public Delegates
		public delegate void StudyFoundSetDelegate(IList<StudySummary> list);

		public StudyFoundSetDelegate StudyFoundSet;
		#endregion

		#region Private Members
		private readonly StudyController _searchController = new StudyController();
		private string _accessionNumber;
		private string _patientId;
		private string _patientName;
		private string _studyDescription;
		private string _studyDate;
		private int _resultCount;
		private ServerPartition _partition;
		private string _dateFormats;
		private IList<StudySummary> _list = new List<StudySummary>();
		private readonly string STUDYDATE_DATEFORMAT = "yyyyMMdd";
		#endregion

		#region Public Properties
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value; }
		}

		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}
		public string PatientName
		{
			get { return _patientName; }
			set { _patientName = value; }
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

		public ServerPartition Partition
		{
			get { return _partition; }
			set { _partition = value; }
		}
		public string DateFormats
		{
			get { return _dateFormats; }
			set { _dateFormats = value; }
		}

		public IList<StudySummary> List
		{
			get { return _list; }
		}

		public int ResultCount
		{
			get { return _resultCount; }
			set { _resultCount = value; }
		}
		#endregion

		#region Private Methods
		private StudySelectCriteria GetSelectCriteria()
		{
			StudySelectCriteria criteria = new StudySelectCriteria();

			// only query for device in this partition
			criteria.ServerPartitionKey.EqualTo(Partition.Key);

			if (!String.IsNullOrEmpty(PatientId))
			{
				string key = PatientId + "%";
				key = key.Replace("*", "%");
				criteria.PatientId.Like(key);
			}
			if (!String.IsNullOrEmpty(PatientName))
			{
				string key = PatientName + "%";
				key = key.Replace("*", "%");
				key = key.Replace("?", "_");
				criteria.PatientsName.Like(key);
			}
			criteria.PatientsName.SortAsc(0);

			if (!String.IsNullOrEmpty(AccessionNumber))
			{
				string key = AccessionNumber + "%";
				key = key.Replace("*", "%");
				key = key.Replace("?", "_");
				criteria.AccessionNumber.Like(key);
			}

			if (!String.IsNullOrEmpty(StudyDate))
			{
				string key = DateTime.ParseExact(StudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT);
				criteria.StudyDate.Like(key);
			}

			if (!String.IsNullOrEmpty(StudyDescription))
			{
				string key = StudyDescription + "%";
				key = key.Replace("*", "%");
				key = key.Replace("?", "_");
				criteria.StudyDescription.Like(key);
			}
			return criteria;
		}

		#endregion

		#region Public Methods
		public IEnumerable<StudySummary> Select(int startRowIndex, int maximumRows)
		{
			if (maximumRows == 0 || Partition == null) return new List<StudySummary>();
			
			StudySelectCriteria criteria = GetSelectCriteria();

			IList<Study> studyList = _searchController.GetRangeStudies(criteria,startRowIndex,maximumRows);

			_list = new List<StudySummary>();

			foreach (Study study in studyList)
				_list.Add(CreateStudySummary(study));

			if (StudyFoundSet != null)
				StudyFoundSet(_list);

			return _list;
		}

		public int SelectCount()
		{
			if (Partition == null) return 0;

			StudySelectCriteria criteria = GetSelectCriteria();

			ResultCount = _searchController.GetStudyCount(criteria);

			return ResultCount;
		}
		#endregion

		/// <summary>
        /// Returns an instance of <see cref="StudySummary"/> based on a <see cref="Study"/> object.
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        private StudySummary CreateStudySummary(Study study)
        {
            StudySummary summary = new StudySummary();
			StudyController controller = new StudyController();

            summary.Key = study.GetKey();
            summary.AccessionNumber = study.AccessionNumber;
            summary.NumberOfRelatedInstances = study.NumberOfStudyRelatedInstances;
            summary.NumberOfRelatedSeries = study.NumberOfStudyRelatedSeries;
            summary.PatientId = study.PatientId;
            summary.PatientsName = study.PatientsName;
            summary.StudyDate = study.StudyDate;
            summary.StudyDescription = study.StudyDescription;
        	summary.StudyStatusEnum = study.StudyStatusEnum;
        	summary.ModalitiesInStudy = controller.GetModalitiesInStudy(study);
			summary.TheStudy = study;
			summary.ThePartition = Partition;
            return summary;
        }}
}
