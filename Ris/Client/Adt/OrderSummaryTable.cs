using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class OrderSummaryTable : Table<OrderSummary>
    {
        public OrderSummaryTable()
        {
            this.Columns.Add(new TableColumn<OrderSummary, string>("Scheduled Requested For",
                delegate(OrderSummary order) { return Format.DateTime(order.SchedulingRequestDateTime); }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnAccessionNumber,
                delegate(OrderSummary order) { return order.AccessionNumber; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnDiagnosticService,
                delegate(OrderSummary order) { return order.DiagnosticServiceName; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnPriority,
                delegate(OrderSummary order) { return order.OrderPriority.Value; }));

            //TODO PatientNameDetail formatting
            this.Columns.Add(new TableColumn<OrderSummary, string>("Ordered by",
                delegate(OrderSummary order) { return PersonNameFormat.Format(order.OrderingPractitioner.PersonNameDetail); }));

            this.Columns.Add(new TableColumn<OrderSummary, string>("Ordered From",
                delegate(OrderSummary order) { return order.OrderingFacility; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>("Reason for Study",
                delegate(OrderSummary order) { return order.ReasonForStudy; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnCreatedOn,
                delegate(OrderSummary order) { return Format.Date(order.EnteredDateTime); }));
        }
    }
}
