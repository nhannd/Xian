using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RICTable : Table<RICSummary>
    {
        public RICTable()
        {
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnRequestedProcedure,
                delegate(RICSummary item) { return item.RequestedProcedureName; }));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnOrderingPhysician,
                delegate(RICSummary item) { return PersonNameFormat.Format(item.OrderingPractitioner, "%F, %G"); }));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnInsurance,
                delegate(RICSummary item) { return item.Insurance; }));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnScheduledFor,
                delegate(RICSummary item) { return FormatRequestedProcedureScheduledTime(item.ModalityProcedureStepScheduledTime); }));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnFacility,
                delegate(RICSummary item) { return item.PerformingFacility; }));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnStatus,
                delegate(RICSummary item) { return item.Status; }));
        }

        protected string FormatRequestedProcedureScheduledTime(DateTime? scheduledTime)
        {
            if (scheduledTime == null)
                return SR.TextNotScheduled;

            DateTime today = Platform.Time.Date;
            DateTime datePart = scheduledTime.Value.Date;

            if (datePart == today)
            {
                return String.Format(SR.TextTodayTime, Format.Time(scheduledTime));
            }
            else if (datePart == today.AddDays(-1))
            {
                return String.Format(SR.TextYesterdayTime, Format.Time(scheduledTime));
            }
            else if (datePart == today.AddDays(1))
            {
                return String.Format(SR.TextTomorrowTime, Format.Time(scheduledTime));
            }
            else if (datePart.CompareTo(today) < 0)
            {
                TimeSpan ts = today.Subtract(datePart);
                return String.Format(SR.TextXDaysAgo, ts.Days);
            }

            return Format.DateTime(scheduledTime);
        }
    }
}
