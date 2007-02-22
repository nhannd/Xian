using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;


namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorklistTable : Table<WorklistItem>
    {
        public RegistrationWorklistTable()
        {
            this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnSite,
                delegate(WorklistItem item) { return Format.Custom(item.Mrn.AssigningAuthority); }, 0.5f));
            this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnMRN,
                delegate(WorklistItem item) { return Format.Custom(item.Mrn.Id); }, 1.0f));
            this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnName,
                delegate(WorklistItem item) { return Format.Custom(item.PatientName); }, 2.0f));
            this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnHealthcardNumber,
                delegate(WorklistItem item) { return Format.Custom(item.HealthcardNumber.Id); }, 1.0f));
            this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnDateOfBirth,
                delegate(WorklistItem item) { return Format.Date(item.DateOfBirth); }, 1.0f));
            this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnSex,
                delegate(WorklistItem item) { return Format.Custom(item.Sex); }, 0.5f));
        }
   }
}
