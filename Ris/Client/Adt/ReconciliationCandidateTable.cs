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
               new TableColumn<ReconciliationCandidateTableEntry, bool>("Rec.",
                   delegate(ReconciliationCandidateTableEntry item) { return item.Checked; },
                   delegate(ReconciliationCandidateTableEntry item, bool value) { item.Checked = value; }, 0.5f));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>("Score",
                    delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.Score.ToString(); }, 1.0f));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>("Site",
                    delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.MRN.AssigningAuthority; }, 0.5f));
            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, string>("MRN",
                   delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.MRN.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>("Name",
                  delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.Name.Format(); }, 2.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>("Healthcard",
                  delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.Healthcard.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, DateTime>("DOB",
                  delegate(ReconciliationCandidateTableEntry item) { return item.ProfileMatch.PatientProfile.DateOfBirth; }, 1.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>("Sex",
                  delegate(ReconciliationCandidateTableEntry item) { return sexChoices[item.ProfileMatch.PatientProfile.Sex].Value; }, 0.5f));
        }
    }
}
