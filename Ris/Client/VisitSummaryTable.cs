#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using System.Text;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class VisitSummaryTable : Table<VisitSummary>
    {
        public VisitSummaryTable()
        {
            this.Columns.Add(new TableColumn<VisitSummary, string>(SR.ColumnVisitNumber,
                delegate(VisitSummary v) { return VisitNumberFormat.Format(v.VisitNumber); },
                1.0f));

            //Visit type description
            this.Columns.Add(new TableColumn<VisitSummary, string>(SR.ColumnVisitType,
                delegate(VisitSummary v)
                {
                    StringBuilder sb = new StringBuilder();
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
                },
                1.0f));

			//current location
			this.Columns.Add(new TableColumn<VisitSummary, string>(SR.ColumnCurrentLocation,
				delegate(VisitSummary v) { return v.CurrentLocation != null ? v.CurrentLocation.Name : null; },
				1.0f));

			//status
            this.Columns.Add(new TableColumn<VisitSummary, string>(SR.ColumnStatus,
                delegate(VisitSummary v) { return v.Status.Value; },
                1.0f));

            //admit date/time
            this.Columns.Add(new DateTableColumn<VisitSummary>(SR.ColumnAdmitDateTime,
                delegate(VisitSummary v) { return v.AdmitTime; },
                1.0f));

            //discharge datetime
            this.Columns.Add(new DateTableColumn<VisitSummary>(SR.ColumnDischargeDateTime,
                delegate(VisitSummary v) { return v.DischargeTime; },
                1.0f));
        }
    }
}
