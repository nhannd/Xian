using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorklistTable : Table<RegistrationWorklistItem>
    {
        public RegistrationWorklistTable()
        {
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSite,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.MrnAssigningAuthority); }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnMRN,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.MrnID); }, 1.0f));
            
            //TODO: PersonNameDetail formatting
            //this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnName,
            //    delegate(RegistrationWorklistItem item) { return Format.Custom(item.Name); }, 2.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnName,
                delegate(RegistrationWorklistItem item) { return String.Format("{0}, {1}", item.Name.FamilyName, item.Name.GivenName); }, 2.0f));

            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnHealthcardNumber,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.Healthcard.Id); }, 1.0f));

            //TODO: Date formatting
            //this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnDateOfBirth,
            //    delegate(RegistrationWorklistItem item) { return Format.Date(item.DateOfBirth); }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnDateOfBirth,
                delegate(RegistrationWorklistItem item) { return item.DateOfBirth.ToString(); }, 1.0f));

            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSex,
                delegate(RegistrationWorklistItem item) { return Format.Custom(item.Sex); }, 0.5f));
        }
   }
}
