#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class OrderListTable : Table<OrderListItem>
	{
		public OrderListTable()
			: base(3)
		{
			this.Columns.Add(new DateTableColumn<OrderListItem>(SR.ColumnCreatedOn, order => order.EnteredTime, 0.5f));
			this.Columns.Add(new DateTimeTableColumn<OrderListItem>(SR.ColumnScheduledFor, order => order.OrderScheduledStartTime, 0.75f));
			this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnImagingService, order => order.DiagnosticService.Name, 1.5f));

			this.Columns.Add(new TableColumn<OrderListItem, string>(
				SR.ColumnStatus,
				order => order.OrderStatus.Code == "SC" && order.OrderScheduledStartTime == null
					? SR.MessageUnscheduled
					: order.OrderStatus.Value,
				0.5f));

			this.Columns.Add(new TableColumn<OrderListItem, string>(
				SR.ColumnMoreInfo,
				order => string.Format(SR.FormatMoreInfo,
					AccessionFormat.Format(order.AccessionNumber),
					PersonNameFormat.Format(order.OrderingPractitioner.Name),
					order.OrderingFacility.Name),
				1));

			this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnIndication, order => string.Format(SR.FormatIndication, order.ReasonForStudy), 2));
		}
	}
}
