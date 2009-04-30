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
