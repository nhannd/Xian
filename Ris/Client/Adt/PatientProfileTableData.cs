using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Adt
{
    class PatientProfileTableData : TableData<PatientProfile>
    {
        public PatientProfileTableData(IHealthcareServiceLayer service)
        {
            this.Columns.Add(
                new TableColumn<PatientProfile, string>("Site",
                    delegate(PatientProfile profile) { return profile.MRN.AssigningAuthority; }));
            this.Columns.Add(
               new TableColumn<PatientProfile, string>("MRN",
                   delegate(PatientProfile profile) { return profile.MRN.Id; }));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>("Name",
                  delegate(PatientProfile profile) { return profile.Name.Format(); }));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>("Healthcard",
                  delegate(PatientProfile profile) { return profile.Healthcard.Id; }));
            this.Columns.Add(
              new TableColumn<PatientProfile, DateTime>("Date of Birth",
                  delegate(PatientProfile profile) { return profile.DateOfBirth; }));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>("Sex",
                  delegate(PatientProfile profile) { return service.SexEnumTable[profile.Sex].Value; }));
        }
    }
}
