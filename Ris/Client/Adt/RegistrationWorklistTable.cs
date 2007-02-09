using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;


namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorklistTable : Table<RegistrationWorklistItem>
    {
        public RegistrationWorklistTable()
        {
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSite,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.Mrn.AssigningAuthority); }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnMRN,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.Mrn.Id); }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnName,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.PatientName); }, 2.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnHealthcardNumber,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.HealthcardNumber.Id); }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnDateOfBirth,
                delegate(RegistrationWorklistItem item) { return Format.Date(item.DateOfBirth); }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSex,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.Sex); }, 0.5f));
        }
   }
}
