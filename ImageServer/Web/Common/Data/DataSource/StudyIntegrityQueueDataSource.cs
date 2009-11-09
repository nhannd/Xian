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
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Exceptions;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
    public class ReconcileDetails
    {
        #region Private Memebers

        private readonly StudyIntegrityQueue _item;
        private StudyInfo _existingStudy;
        private StudyStorageLocation _location;
        private StudyStorage _studyStorage;

        #endregion


        public class SeriesDetails
        {
            public string Description { get; set; }

            public string Modality { get; set; }

            public int NumberOfInstances { get; set; }

            public string SeriesInstanceUid { get; set; }

            public string SeriesNumber { get; set; }
        }

        public class PatientInfo
        {
            public string Name { get; set; }

            public string Sex { get; set; }

            public string BirthDate { get; set; }

            public string PatientID { get; set; }

            public string IssuerOfPatientID { get; set; }
        }

        public class StudyInfo
        {
            public StudyInfo()
            {
                Series = new List<SeriesDetails>();
                Patient = new PatientInfo();
            }

            public IEnumerable<SeriesDetails> Series { get; set; }

            public string StudyDate { get; set; }

            public PatientInfo Patient { get; set; }

            public string AccessionNumber { get; set; }

            public string StudyInstanceUid { get; set; }
        }

        public ReconcileDetails(StudyIntegrityQueue queueItem)
        {
            _item = queueItem;
        }


        public StudyIntegrityQueue StudyIntegrityQueueItem
        {
            get { return _item; }
        }


        public string StudyInstanceUid { get; set; }


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

        public ImageSetDetails ConflictingImageSet { get; set; }

        public StudyInfo ConflictingStudyInfo { get; set; }

        public string GetFolderPath()
        {
            if (_location == null)
            {
                if (_studyStorage == null)
                {
                    using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
                        _studyStorage = StudyStorage.Load(context, _item.StudyStorageKey);
                    }
                }

                _location = StudyStorageLocation.FindStorageLocations(_studyStorage)[0];

            }

            String path = Path.Combine(_location.FilesystemPath, _location.PartitionFolder);
			path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
            if(!string.IsNullOrEmpty(_item.GroupID)) path = Path.Combine(path, _item.GroupID);
            path = Path.Combine(path, _location.StudyInstanceUid);

            return path;
        }
    }

	public class StudyIntegrityQueueSummary
	{
		#region Private members

	    private StudySummary _study;
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

	    public string ExistingAccessionNumber { get; set; }

	    public string ConflictingAccessionNumber { get; set; }

	    public StudyIntegrityQueue TheStudyIntegrityQueueItem { get; set; }

	    public ServerPartition ThePartition { get; set; }

	    public string ExistingPatientName { get; set; }

	    public string ExistingPatientId { get; set; }

	    public string ConflictingPatientName { get; set; }

	    public string ConflictingPatientId { get; set; }

	    public string[] ConflictingModalities { get; set; }

	    public DateTime ReceivedTime { get; set; }

	    public string StudyInstanceUid { get; set; }


	    public ReconcileStudyWorkQueueData QueueData
	    {
            get
            {
                if (_queueData==null)
                {
                    _queueData = XmlUtils.Deserialize < ReconcileStudyWorkQueueData>(TheStudyIntegrityQueueItem.Details);
                }
                return _queueData;
            }
            set
            {
                _queueData = value;
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
				return true;
			}
		}

	    public StudyIntegrityReasonEnum Reason { get; set; }

	    public string ConflictingStudyDate { get; set; }

	    public string ConflictingStudyDescription { get; set; }

	    #endregion Public Properties
	}

	public class StudyIntegrityQueueDataSource
	{
		#region Public Delegates
		public delegate void StudyIntegrityQueueFoundSetDelegate(IList<StudyIntegrityQueueSummary> list);
		public StudyIntegrityQueueFoundSetDelegate StudyIntegrityQueueFoundSet;
		#endregion

		#region Private Members
		private readonly StudyIntegrityQueueController _searchController = new StudyIntegrityQueueController();

	    public StudyIntegrityQueueDataSource()
	    {
	        List = new List<StudyIntegrityQueueSummary>();
	        ReasonEnum = new List<StudyIntegrityReasonEnum>();
	    }

	    #endregion

		#region Public Properties

	    public string Description { get; set; }

	    public string PatientName { get; set; }

	    public string PatientId { get; set; }

	    public string AccessionNumber { get; set; }

	    public string InsertTime { get; set; }

	    public ServerPartition Partition { get; set; }

	    public IList<StudyIntegrityReasonEnum> ReasonEnum { get; set; }

	    public string DateFormats { get; set; }

	    public IList<StudyIntegrityQueueSummary> List { get; private set; }

	    public int ResultCount { get; set; }

	    public IList<ServerEntityKey> SearchKeys { get; set; }

	    #endregion

		#region Public Methods
		public IEnumerable<StudyIntegrityQueueSummary> Select(int startRowIndex, int maximumRows)
		{
			if (maximumRows == 0 || Partition == null) return new List<StudyIntegrityQueueSummary>();

			StudyIntegrityQueueSelectCriteria criteria = GetStudyIntegrityQueueCriteria();

			IList<StudyIntegrityQueue> queueList = _searchController.GetRangeStudyIntegrityQueueItems(criteria, startRowIndex, maximumRows);

			List = new List<StudyIntegrityQueueSummary>();

			foreach (StudyIntegrityQueue item in queueList)
				List.Add(CreateStudyIntegrityQueueSummary(item));

			if (StudyIntegrityQueueFoundSet != null)
				StudyIntegrityQueueFoundSet(List);

			return List;
		}

		public int SelectCount()
		{
			if (Partition == null) return 0;

			StudyIntegrityQueueSelectCriteria criteria = GetStudyIntegrityQueueCriteria();

			ResultCount = _searchController.GetReconcileQueueItemsCount(criteria);

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
            var summary = new StudyIntegrityQueueSummary();
                
			var ssAdaptor = new StudyStorageAdaptor();
            var storages = ssAdaptor.Get(HttpContextData.Current.ReadContext, item.StudyStorageKey);
            try
            {
                summary.Reason = item.StudyIntegrityReasonEnum;
                summary.TheStudyIntegrityQueueItem = item;
                summary.ThePartition = Partition;

                var queueDescription = new ReconcileStudyQueueDescription();
                queueDescription.Parse(item.Description);
                
                summary.QueueData = item.StudyIntegrityReasonEnum.Equals(StudyIntegrityReasonEnum.InconsistentData) ? XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(item.Details) : XmlUtils.Deserialize<DuplicateSIQQueueData>(item.Details);

                ImageSetDescriptor studyData = ImageSetDescriptor.Parse(item.StudyData.DocumentElement);
                    

                // These fields only exists in Enterprise version
                if (summary.QueueData.Details != null && summary.QueueData.Details.StudyInfo!=null)
                {
                    summary.ReceivedTime = item.InsertTime;
                    summary.ConflictingPatientId = summary.QueueData.Details.StudyInfo.PatientInfo.PatientId;
                    summary.ConflictingPatientName = summary.QueueData.Details.StudyInfo.PatientInfo.Name;
                    summary.ConflictingAccessionNumber = summary.QueueData.Details.StudyInfo.AccessionNumber;
                    summary.ConflictingStudyDate = summary.QueueData.Details.StudyInfo.StudyDate;
                    summary.ConflictingStudyDescription = summary.QueueData.Details.StudyInfo.StudyDescription;
                    
                    var modalities = new List<string>();
                    List<SeriesInformation> seriesList = summary.QueueData.Details.StudyInfo.Series;
                    foreach (SeriesInformation series in seriesList)
                    {
                        if (!modalities.Contains(series.Modality))
                            modalities.Add(series.Modality);
                    }
                    summary.ConflictingModalities = modalities.ToArray();
                }
                else
                {
                    string value;
                    if (studyData.TryGetValue(DicomTags.PatientId, out value))
                        summary.ConflictingPatientId = value;

                    if (studyData.TryGetValue(DicomTags.PatientsName, out value))
                        summary.ConflictingPatientName = value;

                    if (studyData.TryGetValue(DicomTags.AccessionNumber, out value))
                        summary.ConflictingAccessionNumber = value;

                    if (studyData.TryGetValue(DicomTags.StudyDate, out value))
                        summary.ConflictingStudyDate = value;

                    if (studyData.TryGetValue(DicomTags.StudyDescription, out value))
                        summary.ConflictingStudyDescription = value;

                    // no modality info
                    
                }


                // Fetch existing study info. Note: this is done last because the study may not exist.
                Study study = storages.LoadStudy(HttpContextData.Current.ReadContext);
                summary.StudySummary = StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study);
                if (summary.StudySummary!=null)
                {
                    summary.StudyInstanceUid = summary.StudySummary.StudyInstanceUid;
                    summary.ExistingPatientName = summary.StudySummary.PatientsName;
                    summary.ExistingPatientId = summary.StudySummary.PatientId;
                    summary.ExistingAccessionNumber = summary.StudySummary.AccessionNumber;
                }
                
            }
            catch(StudyNotFoundException)
            {
                // Study record may not exist. For eg, duplicate arrives but the existing study hasn't been processed.
            }


            return summary;
		}

		private StudyIntegrityQueueSelectCriteria GetStudyIntegrityQueueCriteria()
		{
			var criteria = new StudyIntegrityQueueSelectCriteria();

			// only query for device in this partition
			criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

			string description = string.Empty;
            if (!String.IsNullOrEmpty(PatientName))
            {
                description += "%ExistingPatientName=%" + PatientName + "%";
            }
            if (!String.IsNullOrEmpty(PatientId))
            {
                description += "%ExistingPatientId=%" + PatientId + "%";
            }
            else if (!String.IsNullOrEmpty(description))
                description += "%ExistingPatientId=%";

            if (!String.IsNullOrEmpty(AccessionNumber))
            {
                description += "%ExistingAccessionNumber=%" + AccessionNumber + "%";
            }
            else if (!String.IsNullOrEmpty(description))
                description += "%ExistingAccessionNumber=%";

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

            if(ReasonEnum.Count > 0)
            {
                criteria.StudyIntegrityReasonEnum.In(ReasonEnum);
            }

			// Must do a sort order for range search to work right in this release.
			criteria.InsertTime.SortAsc(0);

			return criteria;
		}

		#endregion
	}

    public class DuplicateEntryDetails : ReconcileDetails
    {
        private readonly DuplicateSopReceivedQueue _duplicateSIQEntry;

        public DuplicateEntryDetails(StudyIntegrityQueue siqEntry):base(siqEntry)
        {
            _duplicateSIQEntry = new DuplicateSopReceivedQueue(siqEntry);
        }

        public DuplicateSIQQueueData QueueData
        {
            get
            {
            	if (_duplicateSIQEntry != null && _duplicateSIQEntry.Details != null)
                    return XmlUtils.Deserialize<DuplicateSIQQueueData>(_duplicateSIQEntry.Details);
            	return new DuplicateSIQQueueData();
            }
        }
    }
}