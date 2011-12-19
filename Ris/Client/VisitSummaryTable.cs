#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Text;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class VisitSummaryTable : Table<VisitSummary>
	{
		public VisitSummaryTable()
		{
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnVisitNumber, v => VisitNumberFormat.Format(v.VisitNumber), 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnVisitType, FormatVisitType, 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnCurrentLocation, v => v.CurrentLocation != null ? v.CurrentLocation.Name : null, 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnRoom, v => v.CurrentRoom, 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnBed, v => v.CurrentBed, 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnStatus, v => v.Status.Value, 1.0f));
			this.Columns.Add(new DateTableColumn<VisitSummary>(
				SR.ColumnAdmitDateTime, v => v.AdmitTime, 1.0f));
			this.Columns.Add(new DateTableColumn<VisitSummary>(
				SR.ColumnDischargeDateTime, v => v.DischargeTime, 1.0f));
		}

		private static string FormatVisitType(VisitSummary v)
		{
			var sb = new StringBuilder();
			sb.Append(v.PatientClass);
			if (v.PatientType != null)
			{
				sb.Append(" - ");
				sb.Append(v.PatientType.Value);
			}
			if (v.AdmissionType != null)
			{
				sb.Append(" - ");
				sb.Append(v.AdmissionType.Value);
			}
			return sb.ToString();
		}
	}
}
