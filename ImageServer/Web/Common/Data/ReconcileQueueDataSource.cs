#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class ReconcileQueueSummary
	{
		#region Private members

		private string _existingPatientName;
        private string _conflictingPatientName;
	    private string _studyInstanceUID;
	    private DateTime _receivedTime;
	    private ReconcileQueue _reconcileQueueItem;
	    private ServerPartition _partition;
		#endregion Private members

		#region Public Properties

        public ReconcileQueue TheReconcileQueueItem
        {
            get { return _reconcileQueueItem; }
            set { _reconcileQueueItem = value; }
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

        public string ConflictingPatientName
        {
            get { return _conflictingPatientName; }
            set { _conflictingPatientName = value; }
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

		#endregion Public Properties
	}

	public class ReconcileQueueDataSource
	{
		#region Public Delegates
		public delegate void ReconcileQueueFoundSetDelegate(IList<ReconcileQueueSummary> list);
		public ReconcileQueueFoundSetDelegate ReconcileQueueFoundSet;
		#endregion

		#region Private Members
		private ReconcileQueueController _searchController = new ReconcileQueueController();
		private string _description;
		private string _insertTime;
		private int _resultCount;
		private ServerPartition _partition;
		private ReconcileReasonEnum _reasonEnum;

		private string _dateFormats;
		private IList<ReconcileQueueSummary> _list = new List<ReconcileQueueSummary>();
		private IList<ServerEntityKey> _searchKeys;
		#endregion

		#region Public Properties
        
        public string Description
		{
			get { return _description; }
			set { _description = value; }
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
		public ReconcileReasonEnum ReasonEnum
		{
			get { return _reasonEnum; }
			set { _reasonEnum = value; }
		}
		public string DateFormats
		{
			get { return _dateFormats; }
			set { _dateFormats = value; }
		}
		public IList<ReconcileQueueSummary> List
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
		public IEnumerable<ReconcileQueueSummary> Select(int startRowIndex, int maximumRows)
		{
            if (maximumRows == 0 || Partition == null) return new List<ReconcileQueueSummary>();

            ReconcileQueueSelectCriteria criteria = GetReconcileQueueCriteria();

            IList<ReconcileQueue> queueList = _searchController.GetRangeReconcileQueueItems(criteria, startRowIndex, maximumRows);

            _list = new List<ReconcileQueueSummary>();

            foreach (ReconcileQueue item in queueList)
                _list.Add(CreateReconcileQueueSummary(item));

            if (ReconcileQueueFoundSet != null)
                ReconcileQueueFoundSet(_list);

            return _list;
		}

		public int SelectCount()
		{
            if (Partition == null) return 0;

            ReconcileQueueSelectCriteria criteria = GetReconcileQueueCriteria();

            ResultCount = _searchController.GetReconicleQueueItemsCount(criteria);

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
		private ReconcileQueueSummary CreateReconcileQueueSummary(ReconcileQueue item)
		{
			ReconcileQueueSummary summary = new ReconcileQueueSummary();
			summary.TheReconcileQueueItem = item;
			summary.ThePartition = Partition;

            String patientNames = item.Description.Substring(item.Description.IndexOf("=") + 1);
		    summary.ExistingPatientName = patientNames.Substring(0, patientNames.IndexOf("\r\n"));
	    	summary.ConflictingPatientName = patientNames.Substring(patientNames.IndexOf("=") + 1);

            StudyStorageAdaptor ssAdaptor = new StudyStorageAdaptor();
            StudyStorage storages = ssAdaptor.Get(item.StudyStorageKey);           
            StudyAdaptor studyAdaptor = new StudyAdaptor();
			StudySelectCriteria studycriteria = new StudySelectCriteria();
			studycriteria.StudyInstanceUid.EqualTo(storages.StudyInstanceUid);
			IList<Study> studyList = studyAdaptor.Get(studycriteria);

			if (studyList != null && studyList.Count > 0)
			{
			    Study study = studyList[0];
			    summary.StudyInstanceUID = study.StudyInstanceUid;
			}

			return summary;
		}

        private ReconcileQueueSelectCriteria GetReconcileQueueCriteria()
        {
            ReconcileQueueSelectCriteria criteria = new ReconcileQueueSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

            if (!String.IsNullOrEmpty(Description))
            {
                string key = Description + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.Description.Like(key);
            }

            if (!String.IsNullOrEmpty(InsertTime))
            {
                DateTime insertTime = DateTime.Parse(InsertTime);
                criteria.InsertTime.Like(insertTime);
            }

            return criteria;
        }

	    #endregion
	}
}
