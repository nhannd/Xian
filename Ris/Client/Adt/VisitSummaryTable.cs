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

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using System.Text;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
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
                    if (!string.IsNullOrEmpty(v.PatientType))
                    {
                        sb.Append(" - ");
                        sb.Append(v.PatientType);
                    }
                    if (!string.IsNullOrEmpty(v.AdmissionType))
                    {
                        sb.Append(" - ");
                        sb.Append(v.AdmissionType);
                    }
                    return sb.ToString();
                },
                1.0f));
            
            ////number
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnNumber,
            //    delegate(Visit v) { return v.VisitNumber.Id; },
            //    1.0f));
            ////site
            //this.Columns.Add(new TableColumn<Visit, string>("Assigning Authority",
            //    delegate(Visit v) { return v.VisitNumber.AssigningAuthority; },
            //    1.0f));

            //status
            this.Columns.Add(new TableColumn<VisitSummary, string>(SR.ColumnStatus,
                delegate(VisitSummary v) { return v.Status; },
                1.0f));

            //admit date/time
            this.Columns.Add(new TableColumn<VisitSummary, string>(SR.ColumnAdmitDateTime,
                delegate(VisitSummary v) { return Format.Date(v.AdmitDateTime); },
                1.0f));

            ////Patient class
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnPatientClass,
            //    delegate(Visit v) { return patientClasses[v.PatientClass].Value; },
            //    1.0f));

            ////patient type
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnPatientType,
            //    delegate(Visit v) { return patientTypes[v.PatientType].Value; },
            //    1.0f));

            ////admission type
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnAdmissionType,
            //    delegate(Visit v) { return admissionTypes[v.AdmissionType].Value; },
            //    1.0f));

            //facility
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnFacility,
            //    delegate(Visit v) { return v.Facility != null ? v.Facility.Name : ""; },
            //    1.0f));ime

            //discharge datetime
            this.Columns.Add(new TableColumn<VisitSummary, string>(SR.ColumnDischargeDateTime,
                delegate(VisitSummary v) { return Format.Date(v.DischargeDateTime); },
                1.0f));

            ////discharge disposition
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnDischargeDisposition,
            //    delegate(Visit v) { return v.DischargeDisposition; },
            //    1.0f));

            //current location
            //_visits.Columns.Add(new TableColumn<Visit, string>(SR.ColumnCurrentLocation,
            //    delegate(Visit v) { return v.CurrentLocation.Format(); },
            //    1.0f));

            //practitioners
            //_visits.Columns.Add(new TableColumn<Visit, string>("Some Practitioner",
            //    delegate(Visit v) { return; },
            //    1.0f));
            
            ////VIP
            //this.Columns.Add(new TableColumn<Visit, bool>("VIP?",
            //    delegate(Visit v) { return v.VipIndicator; },
            //    1.0f));

            ////Ambulatory status
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnAmbulatoryStatus,
            //    delegate(Visit v) { return ambulatoryStatuses[v.AmbulatoryStatus].Value; },
            //    1.0f));
            
            ////preadmit number
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnPreAdmitNumber,
            //    delegate(Visit v) { return v.PreadmitNumber; },
            //    1.0f));

        }
    }
}
