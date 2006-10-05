using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    class ReconciliationCandidateTableData : Table<ReconciliationCandidateTableEntry>
    {
        public ReconciliationCandidateTableData(IHealthcareServiceLayer service)
        {
            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, bool>("R",
                   delegate(ReconciliationCandidateTableEntry item) { return item.Checked; },
                   delegate(ReconciliationCandidateTableEntry item, bool value) { item.Checked = value; }));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>("Score",
                    delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.Score.ToString(); }));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>("Site",
                    delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.MRN.AssigningAuthority; }));
            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, string>("MRN",
                   delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.MRN.Id; }));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>("Name",
                  delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.Name.Format(); }));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>("Healthcard",
                  delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.Healthcard.Id; }));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, DateTime>("Date of Birth",
                  delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.DateOfBirth; }));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>("Sex",
                  delegate(ReconciliationCandidateTableEntry item) { return service.SexEnumTable[item.ProfileMatch.PatientProfile.Sex].Value; }));
        }
    }
}
