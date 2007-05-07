using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RICTable : Table<RICSummary>
    {
        public RICTable()
        {
            this.Columns.Add(new TableColumn<RICSummary, string>("Requested Procedure",
                delegate(RICSummary item) { return item.RequestedProcedureName; }));

            //TODO: PersonNameDetail formatting
            //this.Columns.Add(new TableColumn<RICSummary, string>("Ordering Physician",
            //    delegate(RICSummary item) { return Format.Custom(item.OrderingPractitioner); }));
            this.Columns.Add(new TableColumn<RICSummary, string>("Ordering Physician",
                delegate(RICSummary item) { return String.Format("{0}, {1}", item.OrderingPractitioner.FamilyName, item.OrderingPractitioner.GivenName); }));

            this.Columns.Add(new TableColumn<RICSummary, string>("Insurance",
                delegate(RICSummary item) { return item.Insurance; }));
            this.Columns.Add(new TableColumn<RICSummary, string>("Scheduled For",
                delegate(RICSummary item) { return FormatRequestedProcedureScheduledTime(item.ModalityProcedureStepScheduledTime); }));
            this.Columns.Add(new TableColumn<RICSummary, string>("Facility",
                delegate(RICSummary item) { return item.PerformingFacility; }));
            this.Columns.Add(new TableColumn<RICSummary, string>("Status",
                delegate(RICSummary item) { return item.Status; }));
        }

        private string FormatRequestedProcedureScheduledTime(DateTime? scheduledTime)
        {
            DateTime today = Platform.Time.Date;
            DateTime datePart = scheduledTime.Value.Date;

            if (datePart == today)
            {
                return String.Format("Today {0}", Format.Time(scheduledTime));
            }
            else if (datePart == today.AddDays(-1))
            {
                return String.Format("Yesterday {0}", Format.Time(scheduledTime));
            }
            else if (datePart == today.AddDays(1))
            {
                return String.Format("Tomorrow {0}", Format.Time(scheduledTime));
            }
            else if (datePart.CompareTo(today) < 0)
            {
                TimeSpan ts = today.Subtract(datePart);
                if (ts.Days <= 1)
                    return String.Format("{0} day ago", ts.Days);
                else
                    return String.Format("{0} days ago", ts.Days);
            }

            return Format.DateTime(scheduledTime);
        }
   }
}
