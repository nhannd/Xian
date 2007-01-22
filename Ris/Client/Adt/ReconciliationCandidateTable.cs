using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    class ReconciliationCandidateTable : Table<ReconciliationCandidateTableEntry>
    {
        public ReconciliationCandidateTable()
        {
            IAdtService service = ApplicationContext.GetService<IAdtService>();
            SexEnumTable sexChoices = service.GetSexEnumTable();

            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, bool>(SR.ColumnAbbreviationReconciliation,
                   delegate(ReconciliationCandidateTableEntry item) { return item.Checked; },
                   delegate(ReconciliationCandidateTableEntry item, bool value) { item.Checked = value; }, 0.5f));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnScore,
                    delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.Score.ToString(); }, 1.0f));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnSite,
                    delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.Mrn.AssigningAuthority; }, 0.5f));
            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnMRN,
                   delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.Mrn.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnName,
                  delegate(ReconciliationCandidateTableEntry item) { return Format.Custom(item.ProfileMatch.PatientProfile.Name); }, 2.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnHealthcardNumber,
                  delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.Healthcard.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, DateTime>(SR.ColumnDateOfBirth,
                  delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.DateOfBirth; }, 1.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnSex,
                  delegate(ReconciliationCandidateTableEntry item) { return sexChoices[item.ProfileMatch.PatientProfile.Sex].Value; }, 0.5f));
        }
    }
}
