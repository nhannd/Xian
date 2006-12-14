using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    class PatientProfileTable : Table<PatientProfile>
    {
        public PatientProfileTable()
        {
            IAdtService service = ApplicationContext.GetService<IAdtService>();
            SexEnumTable sexChoices = service.GetSexEnumTable();

            this.Columns.Add(
                new TableColumn<PatientProfile, string>(SR.ColumnSite,
                    delegate(PatientProfile profile) { return profile.Mrn.AssigningAuthority; }, 0.5f));
            this.Columns.Add(
               new TableColumn<PatientProfile, string>(SR.ColumnMRN,
                   delegate(PatientProfile profile) { return profile.Mrn.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>(SR.ColumnName,
                  delegate(PatientProfile profile) { return profile.Name.Format(); }, 2.0f));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>(SR.ColumnHealthcardNumber,
                  delegate(PatientProfile profile) { return profile.Healthcard.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>(SR.ColumnDateOfBirth,
                  delegate(PatientProfile profile) { return Format.Date(profile.DateOfBirth); }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfile, string>(SR.ColumnSex,
                  delegate(PatientProfile profile) { return sexChoices[profile.Sex].Value; }, 0.5f));
        }
    }
}
