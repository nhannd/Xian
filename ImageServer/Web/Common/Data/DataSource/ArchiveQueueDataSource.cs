#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class ArchiveQueueSummary
	{
		#region Private members

		private string _patientID;
		private string _patientName;
		private ServerPartition _thePartition;
		private ArchiveQueue _theArchiveQueueItem;
		#endregion Private members

		#region Public Properties

		public DateTime ScheduledDateTime
		{
			get { return _theArchiveQueueItem.ScheduledTime; }
		}

		public string StatusString
		{
			get { return _theArchiveQueueItem.ArchiveQueueStatusEnum.Description; }
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
			get { return _theArchiveQueueItem.Key; }
		}

		public string ProcessorId
		{
			get { return _theArchiveQueueItem.ProcessorId; }
		}
		public ArchiveQueue TheArchiveQueueItem
		{
			get { return _theArchiveQueueItem; }
			set { _theArchiveQueueItem = value; }
		}
		public ServerPartition ThePartition
		{
			get { return _thePartition; }
			set { _thePartition = value; }
		}
		#endregion Public Properties
	}

	public class ArchiveQueueDataSource
	{
		#region Public Delegates
		public delegate void ArchiveQueueFoundSetDelegate(IList<ArchiveQueueSummary> list);
		public ArchiveQueueFoundSetDelegate ArchiveQueueFoundSet;
		#endregion

		#region Private Members
		private readonly ArchiveQueueController _searchController = new ArchiveQueueController();
		private string _accessionNumber;
		private string _patientId;
		private string _patientName;
		private string _scheduledDate;
		private int _resultCount;
		private ServerPartition _partition;
		private ArchiveQueueStatusEnum _statusEnum;
		private string _dateFormats;
		private IList<ArchiveQueueSummary> _list = new List<ArchiveQueueSummary>();
		private IList<ServerEntityKey> _searchKeys;
		#endregion

		#region Public Properties
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value; }
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
		public string ScheduledDate
		{
			get { return _scheduledDate; }
			set { _scheduledDate = value; }
		}
		public ServerPartition Partition
		{
			get { return _partition; }
			set { _partition = value; }
		}
		public ArchiveQueueStatusEnum StatusEnum
		{
			get { return _statusEnum; }
			set { _statusEnum = value; }
		}
		public string DateFormats
		{
			get { return _dateFormats; }
			set { _dateFormats = value; }
		}
		public IList<ArchiveQueueSummary> List
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
		private IList<ArchiveQueue> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
		{
			resultCount = 0;

			if (maximumRows == 0) return new List<ArchiveQueue>();

			if (SearchKeys != null)
			{
				IList<ArchiveQueue> archiveQueueList = new List<ArchiveQueue>();
				foreach (ServerEntityKey key in SearchKeys)
					archiveQueueList.Add(ArchiveQueue.Load(key));

				resultCount = archiveQueueList.Count;

				return archiveQueueList;
			}

			WebQueryArchiveQueueParameters parameters = new WebQueryArchiveQueueParameters();
			parameters.StartIndex = startRowIndex;
			parameters.MaxRowCount = 25;

			if (Partition != null)
				parameters.ServerPartitionKey = Partition.Key;
			if (AccessionNumber != null)
				parameters.AccessionNumber = AccessionNumber.Replace("*", "%");
			if (PatientId != null)
				parameters.PatientId = PatientId.Replace("*", "%");
			if (PatientName != null)
				parameters.PatientsName = PatientName.Replace("*", "%");

			if (String.IsNullOrEmpty(ScheduledDate))
				parameters.ScheduledTime = null;
			else
				parameters.ScheduledTime = DateTime.ParseExact(ScheduledDate, DateFormats, null);

			if (StatusEnum != null)
				parameters.ArchiveQueueStatusEnum = StatusEnum;

			IList<ArchiveQueue> list = _searchController.FindArchiveQueue(parameters);

			resultCount = parameters.ResultCount;

			return list;
		}
		#endregion

		#region Public Methods
		public IEnumerable<ArchiveQueueSummary> Select(int startRowIndex, int maximumRows)
		{
			IList<ArchiveQueue> list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

			_list = new List<ArchiveQueueSummary>();
			foreach (ArchiveQueue item in list)
				_list.Add(CreateWorkQueueSummary(item));

			if (ArchiveQueueFoundSet != null)
				ArchiveQueueFoundSet(_list);

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
		private ArchiveQueueSummary CreateWorkQueueSummary(ArchiveQueue item)
		{
			ArchiveQueueSummary summary = new ArchiveQueueSummary();
			summary.TheArchiveQueueItem = item;
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
			studycriteria.ServerPartitionKey.EqualTo(storages.ServerPartitionKey);
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

			return summary;
		}
		#endregion
	}
}