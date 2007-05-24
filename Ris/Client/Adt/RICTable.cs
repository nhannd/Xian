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
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnRequestedProcedures,
                delegate(RICSummary item) { return item.RequestedProcedureName; }, 0.4f));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnScheduledFor,
                delegate(RICSummary item) { return (item.ScheduledTime == null ? SR.TextNotScheduled : FormatTime(item.ScheduledTime.Value)); }, 0.2f));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnStatus,
                delegate(RICSummary item) { return item.Status; }, 0.2f));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnInsurance,
                delegate(RICSummary item) { return item.Insurance; }, 0.1f));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnOrderingFacility,
                delegate(RICSummary item) { return item.OrderingFacility; }, 0.1f));
            this.Columns.Add(new TableColumn<RICSummary, string>(SR.ColumnOrderingPhysician,
                delegate(RICSummary item) { return PersonNameFormat.Format(item.OrderingPractitioner, "%F, %G"); }, 0.2f));
        }

        protected string FormatTime(DateTime time)
        {
            DateTime today = Platform.Time.Date;
            DateTime datePart = time.Date;

            if (datePart == today)
            {
                return String.Format(SR.TextTodayTime, Format.Time(time));
            }
            else if (datePart == today.AddDays(-1))
            {
                return SR.TextYesterday;
            }
            else if (datePart == today.AddDays(1))
            {
                return String.Format(SR.TextTomorrowTime, Format.Time(time));
            }
            else if (datePart.CompareTo(today) < 0)
            {
                TimeSpan ts = today.Subtract(datePart);
                return String.Format(SR.TextXDaysAgo, ts.Days);
            }

            return Format.DateTime(time);
        }
    }
}
