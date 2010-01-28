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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	/// <summary>
	/// Model used in study list grid control <see cref="Study"/>.
	/// </summary>
	/// <remarks>
	/// </remarks>
	[Serializable]
	public class StudySummary
	{
		#region Private members

		private bool? _requiresWorkQueueAttention;

	    #endregion Private members


		#region Public Properties

		public ServerEntityKey Key { get; set; }

		public string PatientId { get; set; }

		public string PatientsName { get; set; }

		public string StudyDate { get; set; }

		public string AccessionNumber { get; set; }

		public string StudyDescription { get; set; }

		public int NumberOfStudyRelatedSeries { get; set; }

		public int NumberOfStudyRelatedInstances { get; set; }

		public StudyStatusEnum StudyStatusEnum { get; set; }

		public QueueStudyStateEnum QueueStudyStateEnum { get; set; }

		public string StudyStatusEnumString
		{
			get
			{
				if (!QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle))
					return String.Format("{0}, {1}", StudyStatusEnum.Description, QueueStudyStateEnum.Description);

				return StudyStatusEnum.Description;
			}
		}

		public string ModalitiesInStudy { get; set; }

		public Study TheStudy { get; set; }

		public ServerPartition ThePartition { get; set; }

		public ArchiveStudyStorage TheArchiveLocation { get; set; }

		public bool IsArchived
		{
			get
			{
				return TheArchiveLocation != null;
			}
		}

	    public bool IsOnlineLossy
        {
            get { return TheStudyStorage.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy); }
        }

		public bool IsLocked { get; set; }

		public bool IsProcessing { get; set; }

		public bool IsArchiving { get; set; }

		public bool IsReconcileRequired { get; set; }

		public bool IsNearline
		{
			get { return StudyStatusEnum == StudyStatusEnum.Nearline; }
		}

		public string StudyInstanceUid { get; set; }

		public string ReferringPhysiciansName { get; set; }

		public string StudyTime { get; set; }

		public string StudyId { get; set; }

		public StudyStorage TheStudyStorage { get; set; }

		public bool HasPendingWorkQueueItems { get; set; }

		#endregion Public Properties


        public bool RequiresWorkQueueAttention
        {
            get
            {
                if (_requiresWorkQueueAttention==null)
                {
                    _requiresWorkQueueAttention = false;
                    StudyController controller = new StudyController();
                    IList<WorkQueue> workqueueItems = controller.GetWorkQueueItems(TheStudy);
                    foreach (WorkQueue item in workqueueItems)
                    {
                        if (!ServerPlatform.IsActiveWorkQueue(item))
                        {
                            _requiresWorkQueueAttention = true;
                            break;
                        }
                    }
                }

                return _requiresWorkQueueAttention.Value;
            }
        } 

		public bool CanScheduleDelete(out string reason)
		{
			if (IsLocked)
			{
				reason = "Study is being locked";
				return false;
			}
			if (IsReconcileRequired)
			{
				reason = "There are images to be reconciled for this study";
				return false;
			}
			if (IsNearline)
			{
				reason = "Study is nearline. It must be restored before it can be deleted.";
				return false;
			}

			reason = String.Empty;
			return true;
		}

		public bool CanScheduleEdit(out string reason)
		{
			if (IsLocked)
			{
				reason = "Study has been locked by another process.";
				return false;
			}

			if (IsProcessing)
			{
				reason = "Study is being processed.";
				return false;
			}

			if (IsNearline)
			{
				reason = "Study is nearline. It must be restored before it can be edited.";
				return false;
			}

			if (HasPendingWorkQueueItems)
			{
				reason = "There are pending WorkQueue entries for this study.";
				return false;
			}

			if (IsReconcileRequired)
			{
				reason = "There are images to be reconciled for this study.";
				return false;
			}

            if (StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy) 
                && IsArchivedLossless)
            {
                reason = "Study was archived as lossless. It must be restored before it can be edited.";
                return false;
            }

			reason = String.Empty;
			return true;
		}

	    public bool IsArchivedLossless
	    {
            get { return TheArchiveLocation != null && TheArchiveLocation.ServerTransferSyntax.Lossless; }
	    }

	    public bool CanScheduleMove(out string reason)
		{
			if (IsLocked)
			{
				reason = "Study is being locked.";
				return false;
			}
            
			if (IsReconcileRequired)
			{
				reason = "There are images to be reconciled for this study.";
				return false;
			}
            
			if (IsNearline)
			{
				reason = "Study is nearline.";
				return false;
			}

			reason = String.Empty;
			return true;
		}

		public bool CanScheduleRestore(out string reason)
		{
			if (IsArchiving)
			{
				reason = "Study is being archived";
				return false;
			}

			if (!IsArchived)
			{
				reason = "Study has not been archived.";
				return false;
			}
            
			if (IsLocked)
			{
				reason = "Study is being locked";
				return false;
			}
            
			reason = String.Empty;
			return true;
		}

	    public bool CanScheduleReconcile(out string reason)
		{
			if (IsLocked)
			{
				reason = "Study is being locked";
				return false;
			}
            
			if (IsReconcileRequired)
			{
				reason = "There are images to be reconciled for this study";
				return false;
			}

			if (IsNearline)
			{
				reason = "Study is nearline.";
				return false;
			}

			reason = String.Empty;
			return true;
		}

	    public bool CanReprocess(out string reason)
	    {
            if (IsNearline)
            {
                reason = "Study is nearline.";
                return false;
            }
            
            
            if (HasPendingWorkQueueItems)
            {
                reason = "There is some pending work queue item(s) for this study.";
                return false;
            } 
            
            if (IsProcessing)
            {
                reason = "Study is being processed.";
                return false;
            }

            // no pending work queue items
            reason = String.Empty;
            return true;
	    }
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
		private IList<StudySummary> _list = new List<StudySummary>();
		private readonly string STUDYDATE_DATEFORMAT = "yyyyMMdd";

		#endregion

		#region Public Properties

		public string AccessionNumber { get; set; }

		public string PatientId { get; set; }

		public string PatientName { get; set; }

		public string StudyDescription { get; set; }

		public string ToStudyDate { get; set; }

		public string FromStudyDate { get; set; }

		public ServerPartition Partition { get; set; }

		public string DateFormats { get; set; }

		public IList<StudySummary> List
		{
			get { return _list; }
		}

		public int ResultCount { get; set; }

		public string[] Modalities { get; set; }

		public string[] Statuses { get; set; }

		#endregion

		#region Private Methods
		private StudySelectCriteria GetSelectCriteria()
		{
			StudySelectCriteria criteria = new StudySelectCriteria();

			// only query for device in this partition
			criteria.ServerPartitionKey.EqualTo(Partition.Key);

			if (!String.IsNullOrEmpty(PatientId))
			{
				string key = PatientId.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.PatientId.Like(key);
			}

			if (!String.IsNullOrEmpty(PatientName))
			{
				string key = PatientName.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.PatientsName.Like(key);
			}
			criteria.PatientsName.SortAsc(0);

			if (!String.IsNullOrEmpty(AccessionNumber))
			{
				string key = AccessionNumber.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.AccessionNumber.Like(key);
			}

            if (!String.IsNullOrEmpty(ToStudyDate) && !String.IsNullOrEmpty(FromStudyDate))
			{
                string toKey = DateTime.ParseExact(ToStudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT) + " 23:59:59.997";
                string fromKey = DateTime.ParseExact(FromStudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT);
				criteria.StudyDate.Between(fromKey, toKey);
            }
            else if (!String.IsNullOrEmpty(ToStudyDate))
            {
                string toKey = DateTime.ParseExact(ToStudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT);
                criteria.StudyDate.LessThanOrEqualTo(toKey);
            }
            else if (!String.IsNullOrEmpty(FromStudyDate))
            {
                string fromKey = DateTime.ParseExact(FromStudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT);
                criteria.StudyDate.MoreThanOrEqualTo(fromKey);
            }

			if (!String.IsNullOrEmpty(StudyDescription))
			{
				string key = StudyDescription.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.StudyDescription.Like(key);
			}

			if(Modalities != null && Modalities.Length > 0)
			{
				SeriesSelectCriteria seriesCriteria = new SeriesSelectCriteria();
				if (Modalities.Length == 1)
					seriesCriteria.Modality.EqualTo(Modalities[0]);
				else
					seriesCriteria.Modality.In(Modalities);

				criteria.SeriesRelatedEntityCondition.Exists(seriesCriteria);
			}

            if (Statuses != null && Statuses.Length > 0)
            {
                StudyStorageSelectCriteria storageCriteria = new StudyStorageSelectCriteria();
                if (Statuses.Length == 1)
                    storageCriteria.StudyStatusEnum.EqualTo(StudyStatusEnum.GetEnum(Statuses[0]));
                else
                {
                    List<StudyStatusEnum> statusList = new List<StudyStatusEnum>();
                    foreach(string status in Statuses)
                    {
                        statusList.Add(StudyStatusEnum.GetEnum(status));
                    }

                    storageCriteria.StudyStatusEnum.In(statusList);
                }

                criteria.StudyStorageRelatedEntityCondition.Exists(storageCriteria);
            }

			return criteria;
		}

		#endregion

		#region Public Methods
		public IEnumerable<StudySummary> Select(int startRowIndex, int maximumRows)
		{
			if (maximumRows == 0 || Partition == null) return new List<StudySummary>();

			StudySelectCriteria criteria = GetSelectCriteria();

			IList<Study> studyList = _searchController.GetRangeStudies(criteria, startRowIndex, maximumRows);

			_list = new List<StudySummary>();

			foreach (Study study in studyList)
                _list.Add(StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study));

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

	}

	public class StudySummaryAssembler
	{

		/// <summary>
		/// Returns an instance of <see cref="StudySummary"/> based on a <see cref="Study"/> object.
		/// </summary>
		/// <param name="study"></param>
		/// <param name="read"></param>
		/// <returns></returns>
		/// <remark>
		/// 
		/// </remark>
		static public StudySummary CreateStudySummary(IPersistenceContext read, Study study)
		{
            if (study==null)
			{
			    return null;
			}

			StudySummary studySummary = new StudySummary();
			StudyController controller = new StudyController();

			studySummary.Key = study.GetKey();
			studySummary.AccessionNumber = study.AccessionNumber;
			studySummary.NumberOfStudyRelatedInstances = study.NumberOfStudyRelatedInstances;
			studySummary.NumberOfStudyRelatedSeries = study.NumberOfStudyRelatedSeries;
			studySummary.PatientId = study.PatientId;
			studySummary.PatientsName = study.PatientsName;
			studySummary.StudyDate = study.StudyDate;
			studySummary.StudyInstanceUid = study.StudyInstanceUid;
			studySummary.StudyDescription = study.StudyDescription;
			studySummary.ModalitiesInStudy = controller.GetModalitiesInStudy(read, study);
			studySummary.StudyTime = study.StudyTime;
			studySummary.StudyId = study.StudyId;
			studySummary.TheStudy = study;

			studySummary.ThePartition = ServerPartitionMonitor.Instance.FindPartition(study.ServerPartitionKey);
			if (studySummary.ThePartition == null)
				studySummary.ThePartition = ServerPartition.Load(read, study.ServerPartitionKey);

			studySummary.ReferringPhysiciansName = study.ReferringPhysiciansName;
			studySummary.TheStudyStorage = StudyStorage.Load(read, study.ServerPartitionKey, study.StudyInstanceUid);
			studySummary.StudyStatusEnum = studySummary.TheStudyStorage.StudyStatusEnum;
			studySummary.QueueStudyStateEnum = studySummary.TheStudyStorage.QueueStudyStateEnum;

			IList<ArchiveStudyStorage> archiveList = controller.GetArchiveStudyStorage(read, studySummary.TheStudyStorage.Key);
			if (archiveList.Count > 0)
				studySummary.TheArchiveLocation = CollectionUtils.FirstElement(archiveList);

			studySummary.IsArchiving = controller.GetArchiveQueueCount(study) > 0;

			studySummary.IsProcessing = studySummary.TheStudyStorage.WriteLock;

			// the study is considered "locked" if it's being processed or some action which requires the lock has been scheduled
			// No additional action should be allowed on the study until everything is completed.
			studySummary.IsLocked = studySummary.IsProcessing ||
			                        (studySummary.TheStudyStorage.QueueStudyStateEnum != QueueStudyStateEnum.Idle);

           IList<StudyIntegrityQueue> integrityQueueItems =
				controller.GetStudyIntegrityQueueItems(studySummary.TheStudyStorage.Key);

			if (integrityQueueItems != null && integrityQueueItems.Count > 0)
			{
				studySummary.IsReconcileRequired = true;
			}

		    IList<WorkQueue> workQueueItems = controller.GetWorkQueueItems(study);
		    studySummary.HasPendingWorkQueueItems = CollectionUtils.Contains(workQueueItems, 
                delegate(WorkQueue item)
                     {
                         return
                             item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Idle) ||
                             item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.InProgress) ||
                             item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Pending);
                     });

            
			return studySummary;
		}
	}
}