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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	/// <summary>
	/// Datasource for use with the ObjectDataSource to select a subset of results
	/// </summary>
	public class WorkQueueDataSource
	{
		#region Public Delegates
		public delegate void WorkQueueFoundSetDelegate(IList<WorkQueue> list);

		public WorkQueueFoundSetDelegate WorkQueueFoundSet;
		#endregion

		#region Private Members
		private WorkQueueController _searchController = new WorkQueueController();
		private string _accessionNumber;
		private string _patientId;
		private string _scheduledDate;
		private string _studyDescription;
		private int _resultCount;
		private ServerPartition _partition;
		private WorkQueueTypeEnum _typeEnum;
		private WorkQueueStatusEnum _statusEnum;
		private WorkQueuePriorityEnum _priorityEnum;
		private string _dateFormats;
		private IList<WorkQueue> _list = new List<WorkQueue>();
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
		public string ScheduledDate
		{
			get { return _scheduledDate; }
			set { _scheduledDate = value; }
		}
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		public ServerPartition Partition
		{
			get { return _partition; }
			set { _partition = value; }
		}

		public WorkQueueTypeEnum TypeEnum
		{
			get { return _typeEnum; }
			set { _typeEnum = value; }
		}

		public WorkQueueStatusEnum StatusEnum
		{
			get { return _statusEnum; }
			set { _statusEnum = value; }
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

		public IList<WorkQueue> List
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
		private IList<WorkQueue> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
		{
			resultCount = 0;

			if (maximumRows == 0) return new List<WorkQueue>();

			WebWorkQueueQueryParameters parameters = new WebWorkQueueQueryParameters();
			parameters.StartIndex = startRowIndex;
			parameters.MaxRowCount = 25;
			if (Partition != null)
				parameters.ServerPartitionKey = Partition.Key;
			parameters.Accession = AccessionNumber;
			parameters.PatientID = PatientId;
			if (String.IsNullOrEmpty(ScheduledDate))
				parameters.ScheduledTime = null;
			else
				parameters.ScheduledTime = DateTime.ParseExact(ScheduledDate, DateFormats, null);

			parameters.Accession = AccessionNumber;
			parameters.StudyDescription = StudyDescription;

			if (TypeEnum != null)
				parameters.Type = TypeEnum;

			if (StatusEnum != null)
				parameters.Status = StatusEnum;

			if (PriorityEnum != null)
				parameters.Priority = PriorityEnum;

			IList<WorkQueue> list = _searchController.FindWorkQueue(parameters);

			resultCount = parameters.ResultCount;

			return list;
		}
		#endregion

		#region Public Methods
		public IEnumerable<WorkQueue> Select(int startRowIndex, int maximumRows)
		{
			_list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

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
	}
}
