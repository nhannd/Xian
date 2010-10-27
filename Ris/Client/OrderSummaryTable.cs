#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class OrderSummaryTable : Table<OrderSummary>
    {
        public OrderSummaryTable()
        {
            this.Columns.Add(new DateTimeTableColumn<OrderSummary>("Scheduled Requested For",
                delegate(OrderSummary order) { return order.SchedulingRequestTime; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnAccessionNumber,
                delegate(OrderSummary order) { return AccessionFormat.Format(order.AccessionNumber); }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnImagingService,
                delegate(OrderSummary order) { return order.DiagnosticServiceName; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnPriority,
                delegate(OrderSummary order) { return order.OrderPriority.Value; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnStatus,
                delegate(OrderSummary order) { return order.OrderStatus.Value; }));

            this.Columns.Add(new TableColumn<OrderSummary, string>("Ordered by",
                delegate(OrderSummary order) { return PersonNameFormat.Format(order.OrderingPractitioner.Name); }));

            this.Columns.Add(new TableColumn<OrderSummary, string>("Ordered From",
                delegate(OrderSummary order) { return order.OrderingFacility; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>("Reason for Study",
                delegate(OrderSummary order) { return order.ReasonForStudy; }));
			this.Columns.Add(new DateTableColumn<OrderSummary>(SR.ColumnCreatedOn,
                delegate(OrderSummary order) { return order.EnteredTime; }));

        }
    }
}
