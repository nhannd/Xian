#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Reflection;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// External Practitioner table used by the Admin summary page.
	/// </summary>
	public class ExternalPractitionerAdminTable : Table<ExternalPractitionerSummary>
	{
		public ExternalPractitionerAdminTable()
		{
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnFamilyName
				, item => item.Name.FamilyName, 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnGivenName,
				item => item.Name.GivenName, 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnLicenseNumber,
				item => item.LicenseNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnBillingNumber,
				item => item.BillingNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, bool>(SR.ColumnVerified,
				item => item.IsVerified, 0.25f));
		}
	}

	/// <summary>
	/// External Practitioner table used by the folder system.
	/// </summary>
	public class ExternalPractitionerWorkflowTable : Table<ExternalPractitionerSummary>
	{
		private readonly ITableColumn _timeColumn;
		private PropertyInfo _timeInfo;

		public ExternalPractitionerWorkflowTable()
		{

			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnName
				, item => PersonNameFormat.Format(item.Name), 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnLicenseNumber,
				item => item.LicenseNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnBillingNumber,
				item => item.BillingNumber, 0.5f));
			this.Columns.Add(_timeColumn = new DateTimeTableColumn<ExternalPractitionerSummary>(SR.ColumnTime,
				item => _timeInfo == null ? item.LastVerifiedTime : (DateTime?)_timeInfo.GetValue(item, null), 0.75f));

			// Perform initial sort
			this.Sort(new TableSortParams(_timeColumn, true));
		}

		public string PropertyNameForTimeColumn
		{
			get { return _timeInfo == null ? null : _timeInfo.Name; }
			set
			{
				// Get the field info that matches the sortByTimeFieldName.  Make sure the field name is actually a date time type
				var info = string.IsNullOrEmpty(value) ? null : typeof(ExternalPractitionerSummary).GetProperty(value);
				if (info != null && info.PropertyType.Equals(typeof (DateTime?)))
					_timeInfo = info;
			}
		}

		public bool SortAscending
		{
			get { return this.SortParams == null ? true : this.SortParams.Ascending; }
			set
			{
				if (this.SortParams == null)
				{
					this.Sort(new TableSortParams(_timeColumn, value));
				}
				else
				{
					var newSortParams = new TableSortParams(this.SortParams.Column, value);
					this.Sort(newSortParams);
				}
			}
		}

	}
}
