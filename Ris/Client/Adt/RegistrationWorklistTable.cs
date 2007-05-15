using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorklistTable : Table<RegistrationWorklistItem>
    {
        public RegistrationWorklistTable()
        {
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSite,
                delegate(RegistrationWorklistItem item) { return item.Mrn.AssigningAuthority; }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnMRN,
                delegate(RegistrationWorklistItem item) { return item.Mrn.Id; }, 1.0f));
            
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnName,
                delegate(RegistrationWorklistItem item) { return PersonNameFormat.Format(item.Name); }, 2.0f));

            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnHealthcardNumber,
                delegate(RegistrationWorklistItem item) { return item.Healthcard.Id; }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnDateOfBirth,
                delegate(RegistrationWorklistItem item) { return Format.Date(item.DateOfBirth); }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSex,
                delegate(RegistrationWorklistItem item) { return item.Sex.Value; }, 0.5f));

            // Sort the table by Name initially
            int sortColumnIndex = this.Columns.FindIndex(delegate(TableColumnBase<RegistrationWorklistItem> column) 
                { return column.Name.Equals(SR.ColumnName); });

            this.Sort(new TableSortParams(this.Columns[sortColumnIndex], true));
        }
   }
}
