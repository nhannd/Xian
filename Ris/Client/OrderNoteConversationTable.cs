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

		private const string _itemSeparator = ";  ";

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

			TableColumn<Checkable<OrderNoteDetail>, string> acknowledgeColumn =
				new TableColumn<Checkable<OrderNoteDetail>, string>(
					SR.ColumnNoteAcknowledged,
					delegate(Checkable<OrderNoteDetail> item) { return FormatAcknowledged(item.Item.StaffRecipients, item.Item.GroupRecipients, true); },
					1.5f);
			this.Columns.Add(acknowledgeColumn);

			TableColumn<Checkable<OrderNoteDetail>, string> notAcknowledgeColumn =
				new TableColumn<Checkable<OrderNoteDetail>, string>(
					SR.ColumnNoteNotAcknowledged,
					delegate(Checkable<OrderNoteDetail> item) { return FormatAcknowledged(item.Item.StaffRecipients, item.Item.GroupRecipients, false); },
					1.0f);
			this.Columns.Add(notAcknowledgeColumn);

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

			foreach (OrderNoteDetail.StaffRecipientDetail staffRecipientDetail in staffRecipients)
			{
				if (IsStaffMe(staffRecipientDetail.Staff))
				{
					sb.Insert(0, "me" + _itemSeparator);
				}
				else
				{
					sb.Append(StaffNameAndRoleFormat.Format(staffRecipientDetail.Staff));
					sb.Append(_itemSeparator);
				}
			}

			foreach (OrderNoteDetail.GroupRecipientDetail groupRecipientDetail in groupRecipients)
			{
				sb.Append(groupRecipientDetail.Group.Name);
				sb.Append(_itemSeparator);
			}

			return sb.ToString().TrimEnd(_itemSeparator.ToCharArray());
		}

		private static string FormatAcknowledged(
			IEnumerable<OrderNoteDetail.StaffRecipientDetail> staffRecipients,
			IEnumerable<OrderNoteDetail.GroupRecipientDetail> groupRecipients,
			bool acknowledged)
		{
			StringBuilder sb = new StringBuilder();

			if (acknowledged)
			{
				List<string> acknowledgedStaffId = new List<string>();
				List<OrderNoteDetail.GroupRecipientDetail> acknowledgedGroupRecipients = CollectionUtils.Select(groupRecipients,
					delegate(OrderNoteDetail.GroupRecipientDetail detail) { return detail.IsAcknowledged; });
				List<OrderNoteDetail.StaffRecipientDetail> acknowledgedStaffRecipients = CollectionUtils.Select(staffRecipients,
					delegate(OrderNoteDetail.StaffRecipientDetail detail) { return detail.IsAcknowledged; });

				foreach (OrderNoteDetail.GroupRecipientDetail recipient in acknowledgedGroupRecipients)
				{
					sb.Append(FormatAcknowledgedRecipient(recipient));

					if (!acknowledgedStaffId.Contains(recipient.AcknowledgedByStaff.StaffId))
						acknowledgedStaffId.Add(recipient.AcknowledgedByStaff.StaffId);
				}

				foreach (OrderNoteDetail.StaffRecipientDetail recipient in acknowledgedStaffRecipients)
				{
					// this staff already acknowledged on behalf of a group
					if (acknowledgedStaffId.Contains(recipient.Staff.StaffId))
						continue;

					sb.Append(FormatAcknowledgedRecipient(recipient));

					if (!acknowledgedStaffId.Contains(recipient.Staff.StaffId))
						acknowledgedStaffId.Add(recipient.Staff.StaffId);
				}
			}
			else
			{
				List<OrderNoteDetail.GroupRecipientDetail> notAcknowledgedGroupRecipients = CollectionUtils.Select(groupRecipients,
					delegate(OrderNoteDetail.GroupRecipientDetail detail) { return !detail.IsAcknowledged; });
				List<OrderNoteDetail.StaffRecipientDetail> notAcknowledgedStaffRecipients = CollectionUtils.Select(staffRecipients,
					delegate(OrderNoteDetail.StaffRecipientDetail detail) { return !detail.IsAcknowledged; });

				if (notAcknowledgedGroupRecipients.Count > 0 || notAcknowledgedStaffRecipients.Count > 0)
				{
					foreach (OrderNoteDetail.GroupRecipientDetail recipient in notAcknowledgedGroupRecipients)
					{
						sb.Append(FormatAcknowledgedRecipient(recipient));
					}

					foreach (OrderNoteDetail.StaffRecipientDetail recipient in notAcknowledgedStaffRecipients)
					{
						sb.Append(FormatAcknowledgedRecipient(recipient));
					}
				}
			}

			return sb.ToString().TrimEnd(_itemSeparator.ToCharArray());
		}

		private static string FormatAcknowledgedRecipient(OrderNoteDetail.RecipientDetail recipient)
		{
			StringBuilder sb = new StringBuilder();

			if (recipient is OrderNoteDetail.StaffRecipientDetail)
			{
				OrderNoteDetail.StaffRecipientDetail staffRecipient = (OrderNoteDetail.StaffRecipientDetail) recipient;
				string identity = IsStaffMe(staffRecipient.Staff) ? "me" : StaffNameAndRoleFormat.Format(staffRecipient.Staff);

				if (recipient.IsAcknowledged)
				{
					sb.AppendFormat(identity);
					sb.AppendFormat(" on {0}", Format.DateTime(recipient.AcknowledgedTime));
				}
				else
				{
					sb.Append(identity);
				}
			}
			else // is GroupRecipient
			{
				OrderNoteDetail.GroupRecipientDetail groupRecipient = (OrderNoteDetail.GroupRecipientDetail)recipient;

				if (recipient.IsAcknowledged)
				{
					sb.Append(StaffNameAndRoleFormat.Format(groupRecipient.AcknowledgedByStaff));
					sb.AppendFormat(SR.FormatOnBehalfOf, groupRecipient.Group.Name);
					sb.AppendFormat(" on {0}", recipient.AcknowledgedTime);
				}
				else
				{
					sb.Append(groupRecipient.Group.Name);	
				}
			}

			sb.Append(_itemSeparator);
			return sb.ToString();
		}

		private static bool IsStaffMe(StaffSummary staff)
		{
			return String.Equals(PersonNameFormat.Format(staff.Name), PersonNameFormat.Format(LoginSession.Current.FullName));			
		}
	}
}
