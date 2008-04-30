using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteboxTable : Table<OrderNoteboxItemSummary>
	{
		private static readonly int NumRows = 2;
		private static readonly int DescriptionRow = 1;

		public OrderNoteboxTable()
			: this(NumRows)
		{
		}

		private OrderNoteboxTable(int cellRowCount)
			: base(cellRowCount)
		{

			TableColumn<OrderNoteboxItemSummary, IconSet> acknowledgedColumn =
				new TableColumn<OrderNoteboxItemSummary, IconSet>("Acknowledged?",
					delegate(OrderNoteboxItemSummary item) { return GetIsAcknowledgedIcon(item.IsAcknowledged); },
					0.3f);
			acknowledgedColumn.Comparison = delegate(OrderNoteboxItemSummary item1, OrderNoteboxItemSummary item2)
				{ return item1.IsAcknowledged.CompareTo(item2.IsAcknowledged); };
			acknowledgedColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
			this.Columns.Add(acknowledgedColumn);

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>("MRN",
				delegate(OrderNoteboxItemSummary item) { return MrnFormat.Format(item.Mrn); },
				1.0f));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>("Patient Name",
				delegate(OrderNoteboxItemSummary item) { return PersonNameFormat.Format(item.PatientName); },
				1.0f));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>("Author",
				delegate(OrderNoteboxItemSummary item) { return PersonNameFormat.Format(item.Author.Name); },
				1.0f));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>("Description",
				delegate(OrderNoteboxItemSummary item)
				{
					return string.Format("{0} {1}", item.AccessionNumber, item.DiagnosticServiceName);
				},
				1.0f, DescriptionRow))
			;
		}

		private static IconSet GetIsAcknowledgedIcon(bool isAcknowledged)
		{
			return isAcknowledged ? null : new IconSet("AlertMessenger.png");
		}
	}
}