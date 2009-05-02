#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Exceptions;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
    public class ReconcileDetails
    {
        public class SeriesDetails
        {
            private string _description;
            private int _numberOfInstances;
            private string _modality;

            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }

            public string Modalitiy
            {
                get { return _modality; }
                set { _modality = value; }
            }

            public int NumberOfInstances
            {
                get { return _numberOfInstances; }
                set { _numberOfInstances = value; }
            }
        }

        public class PatientInfo
        {
            private string _name;
            private string _sex;
            private string _birthDate;
            private string _patientID;
            private string _issuerOfPatientID;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Sex
            {
                get { return _sex; }
                set { _sex = value; }
            }

            public string BirthDate
            {
                get { return _birthDate; }
                set { _birthDate = value; }
            }

            public string PatientID
            {
                get { return _patientID; }
                set { _patientID = value; }
            }

            public string IssuerOfPatientID
            {
                get { return _issuerOfPatientID; }
                set { _issuerOfPatientID = value; }
            }


        }

        public class StudyInfo
        {
            private PatientInfo _patient = new PatientInfo();
            private IEnumerable<SeriesDetails> _series = new List<SeriesDetails>();
            private string _accessionNumber;
            private string _studyInstanceUID;
            private string _studyDate;
            public IEnumerable<SeriesDetails> Series
            {
                get { return _series; }
                set { _series = value; }
            }


            public string StudyDate
            {
                get { return _studyDate; }
                set { _studyDate = value; }
            }

            public PatientInfo Patient
            {
                get { return _patient; }
                set { _patient = value; }
            }

            public string AccessionNumber
            {
                get { return _accessionNumber; }
                set { _accessionNumber = value; }
            }

            public string StudyInstanceUID
            {
                get { return _studyInstanceUID; }
                set { _studyInstanceUID = value; }
            }
        }

        private string _studyInstanceUID;
        private readonly StudyIntegrityQueue _item;
        private StudyInfo _existingStudy;
        private ImageSetDetails _conflictingImages;


        public ReconcileDetails(StudyIntegrityQueue queueItem)
        {
            this._item = queueItem;
        }


        public string StudyInstanceUID
        {
            get { return _studyInstanceUID; }
            set { _studyInstanceUID = value; }
        }


        public StudyInfo ExistingStudy
        {
            get
            {
                if (_existingStudy == null)
                    _existingStudy = new StudyInfo();
                return _existingStudy;
            }
            set { _existingStudy = value; }
        }

        public ImageSetDetails ConflictingImageSet
        {
            get
            {
                if (_conflictingImages == null)
                    _conflictingImages = new ImageSetDetails();
                return _conflictingImages;
            }
            set { _conflictingImages = value; }
        }

        public StudyIntegrityQueue StudyIntegrityQueueItem
        {
            get { return _item; }
        }
    }

	public class StudyIntegrityQueueSummary
	{
		#region Private members

	    private StudyIntegrityReasonEnum _reason;
		private string _existingPatientId;
		private string _existingPatientName;
		private string _existingAccessionNumber;
		private string _conflictingAccessionNumber;
		private string _conflictingPatientName;
		private string _conflictingPatientId;
		private string _studyInstanceUID;
		private DateTime _receivedTime;
		private StudyIntegrityQueue _studyIntegrityQueueItem;
		private ServerPartition _partition;
		private StudySummary _study;
	    private string[] _conflictingModalities;
	    private ReconcileStudyWorkQueueData _queueData;

	    #endregion Private members

		#region Public Properties

	    public bool StudyExists
	    {
            get { return StudySummary != null; }
	    }

		public StudySummary StudySummary
		{
			get { return _study; }
			set { _study = value; }
		}
        
		public string ExistingAccessionNumber
		{
			get { return _existingAccessionNumber; }
			set { _existingAccessionNumber = value; }
		}
		public string ConflictingAccessionNumber
		{
			get { return _conflictingAccessionNumber; }
			set { _conflictingAccessionNumber = value; }
		}
		public StudyIntegrityQueue TheStudyIntegrityQueueItem
		{
			get { return _studyIntegrityQueueItem; }
			set { _studyIntegrityQueueItem = value; }
		}

		public ServerPartition ThePartition
		{
			get { return _partition; }
			set { _partition = value;}
		}

		public string ExistingPatientName
		{
			get { return _existingPatientName; }
			set { _existingPatientName = value; }
		}

		public string ExistingPatientId
		{
			get { return _existingPatientId; }
			set { _existingPatientId = value; }
		}

		public string ConflictingPatientName
		{
			get { return _conflictingPatientName; }
			set { _conflictingPatientName = value; }
		}

		public string ConflictingPatientId
		{
			get { return _conflictingPatientId; }
			set { _conflictingPatientId = value; }
		}

	    public string[] ConflictingModalities
	    {
            get { return _conflictingModalities; }
            set { _conflictingModalities = value; }
	    }

		public DateTime ReceivedTime
		{
			get { return _receivedTime; }
			set { _receivedTime = value; }
		}
		public string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
			set { _studyInstanceUID = value; }
		}


	    public ReconcileStudyWorkQueueData QueueData
	    {
            get
            {
                if (_queueData==null)
                {
                    _queueData = XmlUtils.Deserialize < ReconcileStudyWorkQueueData>(_studyIntegrityQueueItem.QueueData);
                }
                return _queueData;
            }
	    }

		public bool CanReconcile
		{
			get
			{
                if (!StudyExists)
                    return false;

				if (_study.IsLocked || _study.IsNearline || _study.IsLocked || _study.IsProcessing)
					return false;
				else
					return true;
			}
		}

	    public StudyIntegrityReasonEnum Reason
	    {
	        get { return _reason; }
	        set { _reason = value; }
	    }

	    #endregion Public Properties
	}

	public class StudyIntegrityQueueDataSource
	{
		#region Public Delegates
		public delegate void StudyIntegrityQueueFoundSetDelegate(IList<StudyIntegrityQueueSummary> list);
		public StudyIntegrityQueueFoundSetDelegate StudyIntegrityQueueFoundSet;
		#endregion

		#region Private Members
		private StudyIntegrityQueueController _searchController = new StudyIntegrityQueueController();
		private string _description;
		private string _patientName;
		private string _patientId;
		private string _accessionNumber;
		private string _insertTime;
		private int _resultCount;
		private ServerPartition _partition;
		private StudyIntegrityReasonEnum _reasonEnum;

		private string _dateFormats;
		private IList<StudyIntegrityQueueSummary> _list = new List<StudyIntegrityQueueSummary>();
		private IList<ServerEntityKey> _searchKeys;
		#endregion

		#region Public Properties
        
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
		public string PatientName
		{
			get { return _patientName; }
			set { _patientName = value; }
		}
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value; }
		}
		public string InsertTime
		{
			get { return _insertTime; }
			set { _insertTime = value; }
		}
		public ServerPartition Partition
		{
			get { return _partition; }
			set { _partition = value; }
		}
		public StudyIntegrityReasonEnum ReasonEnum
		{
			get { return _reasonEnum; }
			set { _reasonEnum = value; }
		}
		public string DateFormats
		{
			get { return _dateFormats; }
			set { _dateFormats = value; }
		}
		public IList<StudyIntegrityQueueSummary> List
		{
			get { return _list; }
		}
		public int ResultCount
		{
			get { return _resultCount; }
			set { _resultCount = value; }
		}
		public IList<ServerEntityKey> SearchKeys
		{
			get { return _searchKeys; }
			set { _searchKeys = value; }
		}

		#endregion

		#region Public Methods
		public IEnumerable<StudyIntegrityQueueSummary> Select(int startRowIndex, int maximumRows)
		{
			if (maximumRows == 0 || Partition == null) return new List<StudyIntegrityQueueSummary>();

			StudyIntegrityQueueSelectCriteria criteria = GetStudyIntegrityQueueCriteria();

			IList<StudyIntegrityQueue> queueList = _searchController.GetRangeStudyIntegrityQueueItems(criteria, startRowIndex, maximumRows);

			_list = new List<StudyIntegrityQueueSummary>();

			foreach (StudyIntegrityQueue item in queueList)
				_list.Add(CreateStudyIntegrityQueueSummary(item));

			if (StudyIntegrityQueueFoundSet != null)
				StudyIntegrityQueueFoundSet(_list);

			return _list;
		}

		public int SelectCount()
		{
			if (Partition == null) return 0;

			StudyIntegrityQueueSelectCriteria criteria = GetStudyIntegrityQueueCriteria();

			ResultCount = _searchController.GetReconicleQueueItemsCount(criteria);

			return ResultCount;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Constructs an instance of <see cref="WorkQueue"/> based on a <see cref="WorkQueueSummary"/> object.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <remark>
		/// 
		/// </remark>
		private StudyIntegrityQueueSummary CreateStudyIntegrityQueueSummary(StudyIntegrityQueue item)
		{
            StudyIntegrityQueueSummary summary = new StudyIntegrityQueueSummary();
                
			StudyStorageAdaptor ssAdaptor = new StudyStorageAdaptor();
            StudyStorage storages = ssAdaptor.Get(HttpContextData.Current.ReadContext, item.StudyStorageKey);
            try
            {
                summary.Reason = item.StudyIntegrityReasonEnum;
                summary.TheStudyIntegrityQueueItem = item;
                summary.ThePartition = Partition;

                ReconcileStudyQueueDescription queueDescription = new ReconcileStudyQueueDescription();
                queueDescription.Parse(item.Description);

                summary.ConflictingPatientId = queueDescription.ConflictingPatientId;
                summary.ConflictingPatientName = queueDescription.ConflictingPatientName;
                summary.ConflictingAccessionNumber = queueDescription.ConflictingAccessionNumber;
                summary.ReceivedTime = item.InsertTime;

                ReconcileStudyWorkQueueData queueData =
                    XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(item.QueueData);

                List<string> modalities = new List<string>();
                List<SeriesInformation> seriesList = queueData.Details.StudyInfo.Series;
                foreach (SeriesInformation series in seriesList)
                {
                    if (!modalities.Contains(series.Modality))
                        modalities.Add(series.Modality);
                }
                summary.ConflictingModalities = modalities.ToArray();

                // Fetch existing study info. Note: this is done last because the study may not exist.
                Study study = storages.LoadStudy(HttpContextData.Current.ReadContext);
                summary.StudySummary = StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study);
                if (summary.StudySummary!=null)
                {
                    summary.StudyInstanceUID = summary.StudySummary.StudyInstanceUid;
                    summary.ExistingPatientName = summary.StudySummary.PatientsName;
                    summary.ExistingPatientId = summary.StudySummary.PatientId;
                    summary.ExistingAccessionNumber = summary.StudySummary.AccessionNumber;
                }
                
            }
            catch(StudyNotFoundException studyNotFoundException)
            {
                // Study record may not exist. For eg, duplicate arrives but the existing study hasn't been processed.
            }


            return summary;
		}

		private StudyIntegrityQueueSelectCriteria GetStudyIntegrityQueueCriteria()
		{
			StudyIntegrityQueueSelectCriteria criteria = new StudyIntegrityQueueSelectCriteria();

			// only query for device in this partition
			criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

			string description = string.Empty;

			if (!String.IsNullOrEmpty(PatientName))
			{
				description += PatientName + "%";
			}
			if (!String.IsNullOrEmpty(PatientId))
			{
				description += PatientId;
			}
			if(!String.IsNullOrEmpty(AccessionNumber))
			{
				description += AccessionNumber + "%";                
			}

			if(!String.IsNullOrEmpty(description))
			{
				description = description.Replace("*", "%");
				description = description.Replace("?", "_");
				criteria.Description.Like(description);    
			}
                       
			if (!String.IsNullOrEmpty(InsertTime))
			{
				DateTime insertTime = DateTime.Parse(InsertTime);
				criteria.InsertTime.Between(insertTime, insertTime.AddHours(24));
			}

			// Must do a sort order for range search to work right in this release.
			criteria.InsertTime.SortAsc(0);

			return criteria;
		}

		#endregion
	}
}