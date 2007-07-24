using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorklistTable : Table<RegistrationWorklistItem>
    {
        public RegistrationWorklistTable()
        {
            TableColumn<RegistrationWorklistItem, IconSet> priorityColumn = new TableColumn<RegistrationWorklistItem, IconSet>(
                SR.ColumnPriority, delegate(RegistrationWorklistItem item) { return GetOrderPriorityIcon(item.OrderPriority); }, 0.5f);

            priorityColumn.Comparison = new Comparison<RegistrationWorklistItem>(
                delegate(RegistrationWorklistItem item1, RegistrationWorklistItem item2)
                {
                    return GetOrderPriorityIndex(item1.OrderPriority) - GetOrderPriorityIndex(item2.OrderPriority);
                });

            priorityColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);

            this.Columns.Add(priorityColumn);
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnPatientClass,
                delegate(RegistrationWorklistItem item) { return GetPatientClassAbbreviation(item.PatientClass); }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSite,
                delegate(RegistrationWorklistItem item) { return item.Mrn.AssigningAuthority; }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnMRN,
                delegate(RegistrationWorklistItem item) { return item.Mrn.Id; }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnName,
                delegate(RegistrationWorklistItem item) { return PersonNameFormat.Format(item.Name); }, 1.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnHealthcardNumber,
                delegate(RegistrationWorklistItem item) { return item.Healthcard.Id; }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnDateOfBirth,
                delegate(RegistrationWorklistItem item) { return Format.Date(item.DateOfBirth); }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSex,
                delegate(RegistrationWorklistItem item) { return item.Sex.Value; }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnScheduledFor,
                delegate(RegistrationWorklistItem item) { return Format.Time(item.EarliestScheduledTime); }, 1.0f));

            // Sort the table by Scheduled Time initially
            int sortColumnIndex = this.Columns.FindIndex(delegate(TableColumnBase<RegistrationWorklistItem> column)
                { return column.Name.Equals(SR.ColumnScheduledFor); });

            this.Sort(new TableSortParams(this.Columns[sortColumnIndex], true));
        }

        private string GetPatientClassAbbreviation(string patientClass)
        {
            switch (patientClass)
            {
                case "Emergency": return "EP";
                case "Inpatient": return "IP";
                case "Outpatient": return "OP";
                case "Preadmit":
                case "Recurring":
                case "Obstetrics":
                case "Not applicable":
                case "Unknown":
                default:
                    return "SP";
            }
        }

        private int GetOrderPriorityIndex(string orderPriority)
        {
            if (String.IsNullOrEmpty(orderPriority))
                return 0;

            switch (orderPriority)
            {
                case "Stat": return 2;
                case "Urgent": return 1;
                default: return 0;
            }
        }

        private IconSet GetOrderPriorityIcon(string orderPriority)
        {
            switch (orderPriority)
            {
                case "Stat": return new IconSet("DoubleExclamation.png");
                case "Urgent": return new IconSet("SingleExclamation.png");
                default: return null;
            }
        }
   }
}
