using ClearCanvas.Desktop;
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
			this.Columns.Add(new DateTimeTableColumn<OrderListItem>(SR.ColumnCreatedOn,
				delegate(OrderListItem order) { return order.EnteredTime; }, 0.5f));
			this.Columns.Add(new DateTimeTableColumn<OrderListItem>("Scheduled For",
				delegate(OrderListItem order) { return order.OrderScheduledStartTime; }, 0.5f));

			this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnImagingService,
				delegate(OrderListItem order) { return order.DiagnosticService.Name; }, 1.5f));
			this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnStatus,
				delegate(OrderListItem order) { return order.OrderStatus.Value; }, 0.5f));

			this.Columns.Add(new TableColumn<OrderListItem, string>("MoreInfo",
				delegate(OrderListItem order)
				{
					return string.Format("{0} Ordered by {1}, Facility: {2}",
										 AccessionFormat.Format(order.AccessionNumber),
										 PersonNameFormat.Format(order.OrderingPractitioner.Name),
										 order.OrderingFacility.Code
										 );
				}, 1));

			this.Columns.Add(new TableColumn<OrderListItem, string>("Indication",
				delegate(OrderListItem order)
				{
					return string.Format("Indication: {0}", order.ReasonForStudy);
				}, 2));

			//this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnAccessionNumber,
			//    delegate(OrderListItem order) { return AccessionFormat.Format(order.AccessionNumber); }));
			//this.Columns.Add(new TableColumn<OrderListItem, string>("Ordering Facility",
			//    delegate(OrderListItem order) { return order.OrderingFacility.Name; }));
			//this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnPriority,
			//    delegate(OrderListItem order) { return order.OrderPriority.Value; }));

			//this.Columns.Add(new DateTimeTableColumn<OrderListItem>(SR.ColumnCreatedOn,
			//    delegate(OrderListItem order) { return order.EnteredTime; }));
		}
	}
}
