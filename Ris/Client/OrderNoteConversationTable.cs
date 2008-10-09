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
			IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

			TableColumn<Checkable<OrderNoteDetail>, IconSet> canAcknowledgeColumn = new TableColumn<Checkable<OrderNoteDetail>, IconSet>(
				SR.ColumnCanAcknowledge,
				delegate(Checkable<OrderNoteDetail> item) { return GetCanAcknowledgeIcon(item.Item.CanAcknowledge); },
				0.2f);
			canAcknowledgeColumn.Comparison = delegate(Checkable<OrderNoteDetail> item1, Checkable<OrderNoteDetail> item2) { return item1.Item.CanAcknowledge.CompareTo(item2.Item.CanAcknowledge); };
			canAcknowledgeColumn.ResourceResolver = resolver;
			this.Columns.Add(canAcknowledgeColumn);

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, bool>(
				SR.ColumnWillAcknowledge,
				delegate(Checkable<OrderNoteDetail> item) { return item.IsChecked; },
				delegate(Checkable<OrderNoteDetail> item, bool value)
				{
					if (item.Item.CanAcknowledge)
					{
						item.IsChecked = value;
						EventsHelper.Fire(_checkedItemsChanged, this, EventArgs.Empty);
					}
					this.Items.NotifyItemUpdated(item);
				},
				0.4f));

			TableColumn<Checkable<OrderNoteDetail>, IconSet> urgentColumn =
				new TableColumn<Checkable<OrderNoteDetail>, IconSet>(SR.ColumnUrgent,
				delegate(Checkable<OrderNoteDetail> item)
				{
					return item.Item.Urgent ? new IconSet("SingleExclamation.png") : null;
				}, 0.5f);
			urgentColumn.Comparison = delegate(Checkable<OrderNoteDetail> item1, Checkable<OrderNoteDetail> item2)
				{
					return item1.Item.Urgent.CompareTo(item2.Item.Urgent);
				};
			urgentColumn.ResourceResolver = resolver;
			this.Columns.Add(urgentColumn);

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>(
				SR.ColumnFrom,
				delegate(Checkable<OrderNoteDetail> item)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append(StaffNameAndRoleFormat.Format(item.Item.Author));
						if (item.Item.OnBehalfOfGroup != null)
							sb.Append(string.Format(SR.FormatOnBehalfOf, item.Item.OnBehalfOfGroup.Name));
						return sb.ToString();
					},
				1.0f));

		    TableColumn<Checkable<OrderNoteDetail>, string> postTimeColumn =
		        new TableColumn<Checkable<OrderNoteDetail>, string>(
		            SR.ColumnNotePostTime,
		            delegate(Checkable<OrderNoteDetail> item) { return Format.DateTime(item.Item.PostTime); },
		            1.2f);
            postTimeColumn.Comparison = delegate(Checkable<OrderNoteDetail> item1, Checkable<OrderNoteDetail> item2)
                { return Nullable.Compare(item1.Item.PostTime, item2.Item.PostTime); };
			this.Columns.Add(postTimeColumn);

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>(
				SR.ColumnTo,
				delegate(Checkable<OrderNoteDetail> item) { return RecipientsList(item.Item.StaffRecipients, item.Item.GroupRecipients); },
				1.0f));

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>(
				SR.ColumnNoteBody,
				delegate(Checkable<OrderNoteDetail> item) { return item.Item.NoteBody; },
				5.0f));

}

		public event EventHandler CheckedItemsChanged
		{
			add { _checkedItemsChanged += value; }
			remove { _checkedItemsChanged -= value; }
		}

		public bool HasUncheckedUnacknowledgedNotes()
		{
			return CollectionUtils.Contains(this.Items,
											delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail)
											{
												return !checkableOrderNoteDetail.IsChecked && checkableOrderNoteDetail.Item.CanAcknowledge;
											});
		}

		public bool HasUnacknowledgedNotes()
		{
			return CollectionUtils.Contains(this.Items,
											delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail)
											{
												return checkableOrderNoteDetail.Item.CanAcknowledge;
											});
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
					sb.Append(StaffNameAndRoleFormat.Format(staffRecipientDetail.Staff));
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