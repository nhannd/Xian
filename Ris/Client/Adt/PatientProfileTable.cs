using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Adt
{
    class PatientProfileTable : Table<PatientProfileSummary>
    {
        public PatientProfileTable()
        {
            this.Columns.Add(
                new TableColumn<PatientProfileSummary, string>(SR.ColumnSite,
                    delegate(PatientProfileSummary profile) { return profile.AssigningAuthority; }, 0.5f));
            this.Columns.Add(
               new TableColumn<PatientProfileSummary, string>(SR.ColumnMRN,
                   delegate(PatientProfileSummary profile) { return profile.Mrn; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnName,
                  delegate(PatientProfileSummary profile) { return profile.Name; }, 2.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnHealthcardNumber,
                  delegate(PatientProfileSummary profile) { return profile.Healthcard; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnDateOfBirth,
                  delegate(PatientProfileSummary profile) { return Format.Custom(profile.DateOfBirth); }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnSex,
                  delegate(PatientProfileSummary profile) { return profile.Sex.Value; }, 0.5f));
        }
    }
}
