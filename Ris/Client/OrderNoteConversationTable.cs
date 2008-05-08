using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteConversationTable : Table<Checkable<OrderNoteDetail>>
	{
		private event EventHandler _checkedItemsChanged;

		public OrderNoteConversationTable()
		{
			TableColumn<Checkable<OrderNoteDetail>, IconSet> canAcknowledgeColumn = new TableColumn<Checkable<OrderNoteDetail>, IconSet>(
				"!",
				delegate(Checkable<OrderNoteDetail> item) { return GetCanAcknowledgeIcon(item.Item.CanAcknowledge); },
				0.2f);
			canAcknowledgeColumn.Comparison = delegate(Checkable<OrderNoteDetail> item1, Checkable<OrderNoteDetail> item2) { return item1.Item.CanAcknowledge.CompareTo(item2.Item.CanAcknowledge); };
			canAcknowledgeColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
			this.Columns.Add(canAcknowledgeColumn);

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>(
				SR.ColumnFrom,
				delegate(Checkable<OrderNoteDetail> item)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append(PersonNameFormat.Format(item.Item.Author.Name));
						if (item.Item.OnBehalfOfGroup != null)
							sb.Append(string.Format(" on behalf of {0}", item.Item.OnBehalfOfGroup.Name));
						return sb.ToString();
					},
				1.0f));

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, DateTime?>(
				"Posted",
				delegate(Checkable<OrderNoteDetail> item) { return item.Item.PostTime; },
				1.0f));

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>(
				SR.ColumnTo,
				delegate(Checkable<OrderNoteDetail> item) { return RecipientsList(item.Item.StaffRecipients, item.Item.GroupRecipients); },
				1.0f));

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>
				("Message",
				delegate(Checkable<OrderNoteDetail> item) { return item.Item.NoteBody; },
				5.0f));

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, bool>(
				".",
				delegate(Checkable<OrderNoteDetail> item) { return item.IsChecked; },
				delegate(Checkable<OrderNoteDetail> item, bool value)
				{
					if (item.Item.CanAcknowledge)
					{
						item.IsChecked = value;
						EventsHelper.Fire(_checkedItemsChanged, this, EventArgs.Empty);
					}
				},
				0.20f));

}

		public event EventHandler CheckedItemsChanged
		{
			add { _checkedItemsChanged += value; }
			remove { _checkedItemsChanged -= value; }
		}

		private static IconSet GetCanAcknowledgeIcon(bool canAcknowledge)
		{
			return canAcknowledge ? new IconSet("WarningHS.png") : null;
		}

		// Creates a semi-colon delimited list of the recipients
		private static string RecipientsList(
			IEnumerable<OrderNoteDetail.StaffRecipientDetail> staffRecipients,
			IEnumerable<OrderNoteDetail.GroupRecipientDetail> groupRecipients)
		{
			StringBuilder sb = new StringBuilder();
			const string itemSeparator = ";  ";

			foreach (OrderNoteDetail.StaffRecipientDetail staffRecipientDetail in staffRecipients)
			{
				if (String.Equals(PersonNameFormat.Format(staffRecipientDetail.Staff.Name), PersonNameFormat.Format(LoginSession.Current.FullName)))
				{
					sb.Insert(0, "me; ");
				}
				else
				{
					sb.Append(PersonNameFormat.Format(staffRecipientDetail.Staff.Name));
					sb.Append(itemSeparator);
				}
			}

			foreach (OrderNoteDetail.GroupRecipientDetail groupRecipientDetail in groupRecipients)
			{
				sb.Append(groupRecipientDetail.Group.Name);
				sb.Append(itemSeparator);
			}

			return sb.ToString().TrimEnd(itemSeparator.ToCharArray());
		}
	}
}