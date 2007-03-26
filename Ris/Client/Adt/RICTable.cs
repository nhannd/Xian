using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

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
                delegate(RICSummary item) { return Format.Custom(item.ModalityProcedureStepScheduledTime); }));
            this.Columns.Add(new TableColumn<RICSummary, string>("Facility",
                delegate(RICSummary item) { return item.PerformingFacility; }));
        }
   }
}
