#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class PatientProfileTable : Table<PatientProfileSummary>
    {
        public PatientProfileTable()
        {
            this.Columns.Add(
               new TableColumn<PatientProfileSummary, string>(SR.ColumnMRN,
                   delegate(PatientProfileSummary profile) { return MrnFormat.Format(profile.Mrn); }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnName,
                  delegate(PatientProfileSummary profile) { return PersonNameFormat.Format(profile.Name); }, 2.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnHealthcardNumber,
                  delegate(PatientProfileSummary profile) { return HealthcardFormat.Format(profile.Healthcard); }, 1.0f));
            this.Columns.Add(
              new DateTableColumn<PatientProfileSummary>(SR.ColumnDateOfBirth,
                  delegate(PatientProfileSummary profile) { return profile.DateOfBirth; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnSex,
                  delegate(PatientProfileSummary profile) { return profile.Sex.Value; }, 0.5f));
        }
    }
}
