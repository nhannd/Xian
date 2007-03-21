using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    public class OrderSummaryTable : Table<OrderSummary>
    {
        public OrderSummaryTable()
        {
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnMRN,
                delegate(OrderSummary summary) { return String.Format("{0} {1}", summary.MrnId, summary.MrnAssigningAuthority); }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnName,
                delegate(OrderSummary summary) { return Format.Custom(summary.PatientName); }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnVisitNumber,
                delegate(OrderSummary summary) { return String.Format("{0} {1}", summary.VisitId, summary.VisitAssigningAuthority); }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnAccessionNumber,
                delegate(OrderSummary summary) { return summary.AccessionNumber; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnDiagnosticService,
                delegate(OrderSummary summary) { return summary.DiagnosticServiceName; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnProcedure,
                delegate(OrderSummary summary) { return summary.RequestedProcedureName; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnScheduledStep,
                delegate(OrderSummary summary) { return summary.ModalityProcedureStepName; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnModality,
                delegate(OrderSummary summary) { return summary.ModalityName; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnPriority,
                delegate(OrderSummary summary) { return summary.OrderPriority.Value; }));
        }
    }
}
