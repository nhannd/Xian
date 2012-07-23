#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;
using System.Text;

namespace ClearCanvas.Ris.Client
{
	public class VisitListTable : Table<VisitListItem>
	{
		public VisitListTable()
			: base(2)
		{
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitNumber, visitListItem => VisitNumberFormat.Format(visitListItem.VisitNumber), 1.0f));

			//Visit type description
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitType, FormatVisitType, 1));

			//status
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitStatus, visitListItem => visitListItem.VisitStatus.Value, 1.0f));

			//admit date/time
			this.Columns.Add(new DateTableColumn<VisitListItem>(SR.ColumnAdmitDateTime, visitListItem => visitListItem.AdmitTime, 1.0f));

			this.Columns.Add(new DateTableColumn<VisitListItem>(SR.ColumnDischargeDateTime, visitListItem => visitListItem.DischargeTime, 1.0f));
		}

		private static string FormatVisitType(VisitListItem item)
		{
			var sb = new StringBuilder();
			sb.Append(item.PatientClass.Value);
			if(item.PatientType != null)
			{
				sb.Append(" - ");
				sb.Append(item.PatientType.Value);
			}
			if (item.AdmissionType != null)
			{
				sb.Append(" - ");
				sb.Append(item.AdmissionType.Value);
			}

			return sb.ToString();
		}
	}
}
