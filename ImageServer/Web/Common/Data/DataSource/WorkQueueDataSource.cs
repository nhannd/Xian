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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	/// <summary>
	/// Summary view of a <see cref="WorkQueue"/> item in the WorkQueue configuration UI.
	/// </summary>
	/// <remarks>
	/// A <see cref="WorkQueueSummary"/> contains the summary of a <see cref="WorkQueue"/> and related information and is displayed
	/// in the WorkQueue configuration UI.
	/// </remarks>
	public class WorkQueueSummary
	{
		#region Private members

	    private string _patientID;
		private string _patientName;
		private string _notes;
		private ServerPartition _thePartition;
		private WorkQueue _theWorkQueueItem;
        private bool _requiresAttention;
        private string _fullDescription;
        #endregion Private members

		#region Public Properties

		public DateTime ScheduledDateTime
		{
			get { return _theWorkQueueItem.ScheduledTime; }
		}

		public string TypeString
		{
			get { return _theWorkQueueItem.WorkQueueTypeEnum.Description; }
		}

		public string StatusString
		{
			get { return _theWorkQueueItem.WorkQueueStatusEnum.Description; }
		}

		public string PriorityString
		{
			get { return _theWorkQueueItem.WorkQueuePriorityEnum.Description; }
		}

		public string PatientId
		{
			get { return _patientID; }
			set { _patientID = value; }
		}

		public string PatientsName
		{
			get { return _patientName; }
			set { _patientName = value; }
		}

		public ServerEntityKey Key
		{
			get { return _theWorkQueueItem.Key; }
		}

		public string Notes
		{
			get { return _notes; }
			set { _notes = value; }
		}

		public string ProcessorID
		{
			get { return _theWorkQueueItem.ProcessorID; }
		}
		public WorkQueue TheWorkQueueItem
		{
			get { return _theWorkQueueItem; }
			set { _theWorkQueueItem = value; }
		}
		public ServerPartition ThePartition
		{
			get { return _thePartition; }
			set { _thePartition = value; }
		}

        public bool RequiresAttention
	    {
            get { return _requiresAttention; }
            set { _requiresAttention = value; }
	    }

        public string FullDescription
        {
            get { return _fullDescription; }
            set { _fullDescription = value; }
        }

	    #endregion Public Properties
	}

	/// <summary>
	/// Datasource for use with the ObjectDataSource to select a subset of results
	/// </summary>
	public class WorkQueueDataSource
	{
		#region Public Delegates
		public delegate void WorkQueueFoundSetDelegate(IList<WorkQueueSummary> list);

		public WorkQueueFoundSetDelegate WorkQueueFoundSet;
		#endregion

		#region Private Members
		private readonly WorkQueueController _searchController = new WorkQueueController();
		private string _patientId;
		private string _scheduledDate;
		private string _patientName;
		private int _resultCount;
		private ServerPartition _partition;
		private WorkQueueTypeEnum[] _typeEnums;
		private WorkQueueStatusEnum[] _statusEnums;
		private WorkQueuePriorityEnum _priorityEnum;
		private string _dateFormats;
		private IList<WorkQueueSummary> _list = new List<WorkQueueSummary>();
		private IList<ServerEntityKey> _searchKeys;
	    private string _processingServer;
		#endregion

		#region Public Properties
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}
		public string ScheduledDate
		{
			get { return _scheduledDate; }
			set { _scheduledDate = value; }
		}
		public string PatientsName
		{
			get { return _patientName; }
			set { _patientName = value; }
		}

	    public string ProcessingServer
	    {
            get { return _processingServer; }
            set { _processingServer = value; }
	    }

		public ServerPartition Partition
		{
			get { return _partition; }
			set { _partition = value; }
		}

		public WorkQueueTypeEnum[] TypeEnums
		{
			get { return _typeEnums; }
			set { _typeEnums = value; }
		}

		public WorkQueueStatusEnum[] StatusEnums
		{
			get { return _statusEnums; }
			set { _statusEnums = value; }
		}

		public WorkQueuePriorityEnum PriorityEnum
		{
			get { return _priorityEnum; }
			set { _priorityEnum = value; }
		}

		public string DateFormats
		{
			get { return _dateFormats; }
			set { _dateFormats = value; }
		}

		public IList<WorkQueueSummary> List
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

		#region Private Methods
		private IList<WorkQueue> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
		{
			resultCount = 0;

			if (maximumRows == 0) return new List<WorkQueue>();

			if (SearchKeys != null)
			{
				IList<WorkQueue> workQueueList = new List<WorkQueue>();
				foreach (ServerEntityKey key in SearchKeys)
					workQueueList.Add(WorkQueue.Load(key));

				resultCount = workQueueList.Count;

				return workQueueList;
			}

			WebWorkQueueQueryParameters parameters = new WebWorkQueueQueryParameters();
			parameters.StartIndex = startRowIndex;
			parameters.MaxRowCount = maximumRows;

			if (Partition != null)
				parameters.ServerPartitionKey = Partition.Key;

			if (!string.IsNullOrEmpty(PatientsName))
			{
				string key = PatientsName.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				parameters.PatientsName = key;
			}
			if (!string.IsNullOrEmpty(PatientId))
			{
				string key = PatientId.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				parameters.PatientID = key;
			}
            if (!string.IsNullOrEmpty(ProcessingServer))
            {
                string key = ProcessingServer.Replace("*", "%");
                key = key.Replace("?", "_");
                key = "%" + key + "%";
                parameters.ProcessorID = key;
            }

			if (String.IsNullOrEmpty(ScheduledDate))
				parameters.ScheduledTime = null;
			else
				parameters.ScheduledTime = DateTime.ParseExact(ScheduledDate, DateFormats, null);

			if (TypeEnums != null && TypeEnums.Length > 0)
			{
				string types = "(";
				if (TypeEnums.Length == 1)
					types += TypeEnums[0].Enum;
				else {
					string separator = "";
					foreach (WorkQueueTypeEnum typeEnum in TypeEnums)
					{
						types += separator + typeEnum.Enum;
						separator = ",";
					}
				}
                
				parameters.Type = types + ")";
			}

			if (StatusEnums != null && StatusEnums.Length > 0)
			{
				string statuses = "(";
				if (StatusEnums.Length == 1)
					statuses += StatusEnums[0].Enum;
				else
				{
					string separator = "";
					foreach (WorkQueueStatusEnum statusEnum in StatusEnums)
					{
						statuses += separator + statusEnum.Enum;
						separator = ",";
					}
				}

				parameters.Status = statuses + ")";
			}

			if (PriorityEnum != null)
				parameters.Priority = PriorityEnum;

			IList<WorkQueue> list = _searchController.FindWorkQueue(parameters);

			resultCount = parameters.ResultCount;

            string workQueueItems = "\n";
            foreach (WorkQueue item in list)
            {
                workQueueItems += "[" + item.Key + "]";
            }

			return list;
		}
		#endregion

		#region Public Methods
		public IEnumerable<WorkQueueSummary> Select(int startRowIndex, int maximumRows)
		{
			IList<WorkQueue> list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

			_list = new List<WorkQueueSummary>();
            foreach (WorkQueue item in list)
            {
                if (item == null) break;
                _list.Add(CreateWorkQueueSummary(item));
            }

		    if (WorkQueueFoundSet != null)
				WorkQueueFoundSet(_list);

			return _list;
		}
		
		public int SelectCount()
		{
			if (ResultCount != 0) return ResultCount;

			// Ignore the search results
			InternalSelect(0, 1, out _resultCount);

			return ResultCount;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Constructs an instance of <see cref="WorkQueueSummary"/> based on a <see cref="WorkQueue"/> object.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <remark>
		/// 
		/// </remark>
		private WorkQueueSummary CreateWorkQueueSummary(WorkQueue item)
		{
			WorkQueueSummary summary = new WorkQueueSummary();
			summary.TheWorkQueueItem = item;
			summary.ThePartition = Partition;

			// Fetch the patient info:
			StudyStorageAdaptor ssAdaptor = new StudyStorageAdaptor();
			StudyStorage storages = ssAdaptor.Get(item.StudyStorageKey);
			if (storages == null)
			{
				summary.PatientId = "N/A";
				summary.PatientsName = "N/A";
				return summary;
			}
			StudyAdaptor studyAdaptor = new StudyAdaptor();
			StudySelectCriteria studycriteria = new StudySelectCriteria();
			studycriteria.StudyInstanceUid.EqualTo(storages.StudyInstanceUid);
			studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
			IList<Study> studyList = studyAdaptor.Get(studycriteria);

			if (studyList == null || studyList.Count == 0)
			{
				summary.PatientId = "N/A";
				summary.PatientsName = "N/A";
			}
			else
			{
				summary.PatientId = studyList[0].PatientId;
				summary.PatientsName = studyList[0].PatientsName;
			}

			if (item.WorkQueueTypeEnum == WorkQueueTypeEnum.WebMoveStudy
			    || item.WorkQueueTypeEnum == WorkQueueTypeEnum.AutoRoute)
			{
				DeviceDataAdapter deviceAdaptor = new DeviceDataAdapter();
				Device dest = deviceAdaptor.Get(item.DeviceKey);

				summary.Notes = String.Format("Destination AE : {0}", dest.AeTitle);

				if (item.FailureDescription != null)
				{
					if (item.FailureDescription.Length > 60)
					{
						summary.Notes = String.Format("{0}, Failure: {1} ...", summary.Notes, item.FailureDescription.Substring(0, 60));
                        summary.FullDescription = String.Format("{0}, Failure: {1}", summary.Notes, item.FailureDescription);   //Set the FullDescription for the Tooltip in the GUI
					}
					else
						summary.Notes = String.Format("{0}, Failure: {1}", summary.Notes, item.FailureDescription);
				}
			}
			else if (item.FailureDescription != null)
			{
				// This used to only be shown when the status was "Failed" for a 
				// queue entry.  We now show it any time there's 
				if (item.FailureDescription.Length > 60)
				{
					summary.Notes = item.FailureDescription.Substring(0, 60);
					summary.Notes += " ...";
				    summary.FullDescription = item.FailureDescription;  //Set the FullDescription for the Tooltip in the GUI
				}
				else
					summary.Notes = item.FailureDescription;
			}

            summary.RequiresAttention = item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Failed) || !ServerPlatform.IsActiveWorkQueue(item);
			return summary;
		}
		#endregion
	}
}