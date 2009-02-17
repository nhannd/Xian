#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
