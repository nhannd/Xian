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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class ApplicationLogDataSource
	{
		#region Public Delegates
		public delegate void ApplicationLogFoundSetDelegate(IList<ApplicationLog> list);

		public ApplicationLogFoundSetDelegate ApplicationLogFoundSet;
		#endregion

		#region Private Members
		private readonly ApplicationLogController _searchController = new ApplicationLogController();
		private string _host;
		private string _logLevel;
		private string _thread;
		private string _message;
		private string _exception;
		private int _resultCount;
		private IList<ApplicationLog> _list = new List<ApplicationLog>();
		private DateTime? _endDate;
		private DateTime? _startDate;

		#endregion

		#region Public Properties
		public string Host
		{
			get { return _host; }
			set { _host = value; }
		}

		public string LogLevel
		{
			get { return _logLevel; }
			set { _logLevel = value; }
		}
		public string Thread
		{
			get { return _thread; }
			set { _thread = value; }
		}
		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}
		public string Exception
		{
			get { return _exception; }
			set { _exception = value; }
		}
		public DateTime? StartDate
		{
			get { return _startDate; }
			set { _startDate = value; }
		}
		public DateTime? EndDate
		{
			get { return _endDate; }
			set { _endDate = value; }
		}

		public IList<ApplicationLog> List
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
		private ApplicationLogSelectCriteria GetSelectCriteria()
		{
			ApplicationLogSelectCriteria criteria = new ApplicationLogSelectCriteria();


			if (!String.IsNullOrEmpty(LogLevel))
			{
				string key = "%" + LogLevel + "%";
				key = key.Replace("*", "%");
				criteria.LogLevel.Like(key);
			}

			if (!String.IsNullOrEmpty(Thread))
			{
				string key = "%" + Thread + "%";
				key = key.Replace("*", "%");
				key = key.Replace("?", "_");
				criteria.Thread.Like(key);
			}

			if (!String.IsNullOrEmpty(Host))
			{
				string key = "%" + Host + "%";
				key = key.Replace("*", "%");
				key = key.Replace("?", "_");
				criteria.Host.Like(key);
			}

			if (!String.IsNullOrEmpty(Exception))
			{
				string key = "%" + Exception + "%";
				key = key.Replace("*", "%");
				key = key.Replace("?", "_");
				criteria.Exception.Like(key);
			}

			if (!String.IsNullOrEmpty(Message))
			{
				string key = "%" + Message + "%";
				key = key.Replace("*", "%");
				key = key.Replace("?", "_");
				criteria.Message.Like(key);
			}

			// Sort with the latest timestamp first
			criteria.Timestamp.SortDesc(0);


			if (StartDate.HasValue && EndDate.HasValue)
			{
				criteria.Timestamp.Between(StartDate.Value, EndDate.Value);
			}
			else if (StartDate.HasValue)
			{
				criteria.Timestamp.MoreThanOrEqualTo(StartDate.Value);
			}
			else if (EndDate.HasValue)
			{
				criteria.Timestamp.LessThanOrEqualTo(EndDate.Value);
			}

			return criteria;
		}

		#endregion

		#region Public Methods
		public IEnumerable<ApplicationLog> Select(int startRowIndex, int maximumRows)
		{
			if (maximumRows == 0) return new List<ApplicationLog>();

			ApplicationLogSelectCriteria criteria = GetSelectCriteria();
			 _list = _searchController.GetRangeLogs(criteria, startRowIndex, maximumRows);

			return _list;
		}

		public int SelectCount()
		{

			ApplicationLogSelectCriteria criteria = GetSelectCriteria();

			ResultCount = _searchController.GetApplicationLogCount(criteria);

			return ResultCount;
		}


		#endregion
	}
}
