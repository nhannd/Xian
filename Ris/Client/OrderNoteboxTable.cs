using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteboxTable : Table<OrderNoteboxItemSummary>
	{
		private static readonly int NumRows = 4;

		public OrderNoteboxTable()
			: this(NumRows)
		{
		}

		private OrderNoteboxTable(int cellRowCount)
			: base(cellRowCount)
		{

			TableColumn<OrderNoteboxItemSummary, IconSet> acknowledgedColumn =
				new TableColumn<OrderNoteboxItemSummary, IconSet>(SR.ColumnStatus,
					delegate(OrderNoteboxItemSummary item) { return GetIsAcknowledgedIcon(item.IsAcknowledged); },
					0.3f);
			acknowledgedColumn.Comparison = delegate(OrderNoteboxItemSummary item1, OrderNoteboxItemSummary item2)
				{ return item1.IsAcknowledged.CompareTo(item2.IsAcknowledged); };
			acknowledgedColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
			this.Columns.Add(acknowledgedColumn);

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnMRN,
				delegate(OrderNoteboxItemSummary item) { return MrnFormat.Format(item.Mrn); },
				1.0f));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnPatientName,
				delegate(OrderNoteboxItemSummary item) { return PersonNameFormat.Format(item.PatientName); },
				1.0f));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnDescription,
				delegate(OrderNoteboxItemSummary item)
				{
					return string.Format("{0} {1}", item.AccessionNumber, item.DiagnosticServiceName);
				},
				1.0f, 1));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnFrom,
				delegate(OrderNoteboxItemSummary item)
				{
					if (item.OnBehalfOfGroup != null)
						return String.Format(SR.FormatFromOnBehalf, PersonNameFormat.Format(item.Author.Name), item.OnBehalfOfGroup.Name, item.PostTime);
					else
						return String.Format(SR.FormatFrom, PersonNameFormat.Format(item.Author.Name), item.PostTime);
				},
				1.0f, 2));

			this.Columns.Add(new TableColumn<OrderNoteboxItemSummary, string>(SR.ColumnTo,
				delegate(OrderNoteboxItemSummary item) { return String.Format(SR.FormatTo, RecipientsList(item.StaffRecipients, item.GroupRecipients)); },
				1.0f, 3));
		}

		private static IconSet GetIsAcknowledgedIcon(bool isAcknowledged)
		{
			return isAcknowledged ? new IconSet("NoteRead.png") : new IconSet("NoteUnread.png");
		}

		// Creates a semi-colon delimited list of the recipients
		private static string RecipientsList(IEnumerable<StaffSummary> staffRecipients, IEnumerable<StaffGroupSummary> groupRecipients)
		{
			StringBuilder sb = new StringBuilder();
			const string itemSeparator = ";  ";

			foreach (StaffSummary staffSummary in staffRecipients)
			{
				if (String.Equals(PersonNameFormat.Format(staffSummary.Name), PersonNameFormat.Format(LoginSession.Current.FullName)))
				{
					sb.Insert(0, "me; ");
				}
				else
				{
					sb.Append(PersonNameFormat.Format(staffSummary.Name));
					sb.Append(itemSeparator);
				}
			}

			foreach (StaffGroupSummary groupSummary in groupRecipients)
			{
				sb.Append(groupSummary.Name);
				sb.Append(itemSeparator);
			}

			return sb.ToString().TrimEnd(itemSeparator.ToCharArray());
		}
	}
}