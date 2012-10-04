#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Reflection;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;
using System;

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
