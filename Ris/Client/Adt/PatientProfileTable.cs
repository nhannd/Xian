using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    class PatientProfileTable : Table<PatientProfile>
    {
        public PatientProfileTable()
        {
            IAdtService service = ApplicationContext.GetService<IAdtService>();
            SexEnumTable sexChoices = service.GetSexEnumTable();

            this.Columns.Add(
                new TableColumn<PatientProfile, string>("Site",
                    delegate(PatientProfile profile) { return profile.MRN.AssigningAuthority; }, 0.5f));
            this.Columns.Add(
               new TableColumn<PatientProfile, string>("MRN",
                   delegate(PatientProfile profile) { return profile.MRN.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>("Name",
                  delegate(PatientProfile profile) { return profile.Name.Format(); }, 2.0f));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>("Healthcard",
                  delegate(PatientProfile profile) { return profile.Healthcard.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfile, DateTime>("DOB",
                  delegate(PatientProfile profile) { return profile.DateOfBirth; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>("Sex",
                  delegate(PatientProfile profile) { return sexChoices[profile.Sex].Value; }, 0.5f));
        }
    }
}
