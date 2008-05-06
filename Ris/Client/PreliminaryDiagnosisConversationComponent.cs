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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="PreliminaryDiagnosisConversationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class PreliminaryDiagnosisConversationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PreliminaryDiagnosisConversationComponent class.
	/// </summary>
	[AssociateView(typeof(PreliminaryDiagnosisConversationComponentViewExtensionPoint))]
	public class PreliminaryDiagnosisConversationComponent : ApplicationComponent
	{
		private readonly EntityRef _orderRef;

		private readonly List<string> _orderNoteCategories = new List<string>();
		private readonly PreliminaryDiagnosisOrderNoteTable _notes;
		private Checkable<OrderNoteDetail> _selectedNote;

		private string _body;

		private readonly Table<string> _recipients;
		private string _selectedRecipient;
		private readonly CrudActionModel _recipientsActionModel;

		private ILookupHandler _staffLookupHandler;
		private StaffSummary _selectedStaff = null;
		private List<StaffSummary> _staffRecipients;

		private ILookupHandler _staffGroupLookupHandler;
		private StaffGroupSummary _selectedStaffGroup = null;
		private List<StaffGroupSummary> _groupRecipients;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PreliminaryDiagnosisConversationComponent(EntityRef orderRef)
		{
			_orderRef = orderRef;
			_orderNoteCategories.Add(OrderNoteCategory.PreliminaryDiagnosis.Key);
			_notes = new PreliminaryDiagnosisOrderNoteTable();
			_notes.CheckedItemsChanged += delegate { NotifyPropertyChanged("AcknowledgeEnabled"); };

			_recipients = new Table<string>();
			_recipients.Columns.Add(new TableColumn<string, string>("Name",
				delegate(string item) { return item; },
				1.0f));
			_recipientsActionModel = new CrudActionModel(true, false, true);
			_recipientsActionModel.Add.SetClickHandler(delegate() { });
			_recipientsActionModel.Add.Visible = false;    // hide this action on the menu/toolbar - we'll use a special button instead
			_recipientsActionModel.Delete.SetClickHandler(RemoveSelectedRecipient);
			UpdateRecipientsActionModel();

			_selectedRecipient = null;

			_staffRecipients = new List<StaffSummary>();
			_groupRecipients = new List<StaffGroupSummary>();
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{

			_staffLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow);
			_staffGroupLookupHandler = new StaffGroupLookupHandler(this.Host.DesktopWindow);

			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
                    List<string> filters = new List<string>(new string[] {OrderNoteCategory.PreliminaryDiagnosis.Key});
					GetConversationRequest request = new GetConversationRequest(_orderRef, filters, false);
					GetConversationResponse response = service.GetConversation(request);

					List<Checkable<OrderNoteDetail>> checkableOrderNoteDetails =
						CollectionUtils.Map<OrderNoteDetail, Checkable<OrderNoteDetail>>(
							response.OrderNotes,
							delegate(OrderNoteDetail detail)
							{
								return new Checkable<OrderNoteDetail>(detail);
							});
					checkableOrderNoteDetails.Reverse();
					_notes.Items.AddRange(checkableOrderNoteDetails);

					// Set default recipients list
					AddDefaultRecipients(response.OrderNotes);
				});
            base.Start();
        }

		private void AddDefaultRecipients(IEnumerable<OrderNoteDetail> notes)
		{
			foreach (OrderNoteDetail note in notes)
			{
				foreach (OrderNoteDetail.StaffRecipientDetail staffRecipient in note.StaffRecipients)
				{
					if (!string.Equals(
							PersonNameFormat.Format(staffRecipient.Staff.Name),
							PersonNameFormat.Format(LoginSession.Current.FullName)))
					{
						if (!CollectionUtils.Contains(
								_staffRecipients,
								delegate(StaffSummary staffSummary)
								{
									return string.Equals(PersonNameFormat.Format(staffRecipient.Staff.Name),
														 PersonNameFormat.Format(staffSummary.Name));
								}))
						{
							_staffRecipients.Add(staffRecipient.Staff);
						}
					}
				}
				foreach (OrderNoteDetail.GroupRecipientDetail groupRecipient in note.GroupRecipients)
				{
					if (!CollectionUtils.Contains(
							_groupRecipients,
							delegate(StaffGroupSummary groupSummary)
							{
								return string.Equals(groupRecipient.Group.Name, groupSummary.Name);
							}))
					{
						_groupRecipients.Add(groupRecipient.Group);
					}
				}
			}
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		#region Presentation Model

		#region Conversation Preview

		public ITable Notes
		{
			get { return _notes; }
		}

		public ISelection SelectedNote
		{
			get { return new Selection(_selectedNote); }
			set
			{
				Checkable<OrderNoteDetail> detail = (Checkable<OrderNoteDetail>)value.Item;
				if (_selectedNote != detail)
				{
					_selectedNote = detail;
				}
			}
		}

		public string SelectedNoteBody
		{
			get { return _selectedNote != null ? _selectedNote.Item.NoteBody : ""; }
		}

		#endregion

		public string Body
		{
			get { return _body; }
			set
			{
				_body = value;
			}
		}

		public ITable Recipients
		{
			get
			{
				_recipients.Items.Clear();
				_recipients.Items.AddRange(ConsolidateRecipientsForDisplay());

				return _recipients;
			}
		}

		public void UpdateRecipientsActionModel()
		{
			_recipientsActionModel.Delete.Enabled = (_selectedRecipient != null);
		}

		public ActionModelNode RecipientActionModel
		{
			get { return _recipientsActionModel; }
		}

		public ISelection SelectedRecipient
		{
			get { return new Selection((object)_selectedRecipient); }
			set
			{
				string recipient = (string)value.Item;
				if (recipient != _selectedRecipient)
				{
					_selectedRecipient = recipient;
					UpdateRecipientsActionModel();
				}
			}
		}

		#region Staff Recipients

		public ILookupHandler StaffRecipientLookupHandler
		{
			get { return _staffLookupHandler; }
		}

		public object SelectedStaffRecipient
		{
			get { return _selectedStaff; }
			set { _selectedStaff = (StaffSummary)value; }
		}

		public bool AddStaffRecipientEnabled
		{
			get { return _selectedStaff != null; }
		}

		public void AddStaffRecipient()
		{
			if (_selectedStaff == null) return;

			bool contains = CollectionUtils.Contains(
				_staffRecipients,
				delegate(StaffSummary staff)
					{
						return string.Equals(PersonNameFormat.Format(staff.Name), PersonNameFormat.Format(_selectedStaff.Name));
					});

			if (!contains)
			{
				_staffRecipients.Add(_selectedStaff);
				NotifyPropertyChanged("Recipients");
			}
		}

		#endregion

		#region Group Recipients

		public ILookupHandler GroupRecipientLookupHandler
		{
			get { return _staffGroupLookupHandler; }
		}

		public object SelectedGroupRecipient
		{
			get { return _selectedStaffGroup; }
			set { _selectedStaffGroup = (StaffGroupSummary)value; }
		}

		public bool AddGroupRecipientEnabled
		{
			get { return _selectedStaffGroup != null; }
		}

		public void AddGroupRecipient()
		{
			if (_selectedStaffGroup == null) return;

			bool contains = CollectionUtils.Contains(
				_groupRecipients,
				delegate(StaffGroupSummary group)
					{
						return string.Equals(group.Name, _selectedStaffGroup.Name);
					});

			if (!contains)
			{
				_groupRecipients.Add(_selectedStaffGroup);
				NotifyPropertyChanged("Recipients");
			}
		}

		#endregion

		public string ReplyOrAcknowledge
		{
			get { return string.IsNullOrEmpty(_body) ? "Acknowledge" : "Post and Acknowledge"; }
		}

		public bool AcknowledgeEnabled
		{
			get { return !HasUnacknowledgedNotes(); }
		}

		public void AcknowledgeAndOrPost()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			//else if (ConversationIsStale())
			//{
			//    // Message box
			//    // "The conversation has been updated.  Please review the additional notes before proceeding."
			//}
			else
			{
				try
				{
					SaveChanges();

					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, "Foo", this.Host.DesktopWindow,
											delegate()
											{
												this.ExitCode = ApplicationComponentExitCode.Error;
												this.Host.Exit();
											});
				}
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

		private void RemoveSelectedRecipient()
		{
			if (string.IsNullOrEmpty(_selectedRecipient)) return;

			CollectionUtils.Remove(
				_staffRecipients,
				delegate(StaffSummary staffSummary)
				{
					return string.Equals(PersonNameFormat.Format(staffSummary.Name), _selectedRecipient);
				});

			CollectionUtils.Remove(
				_groupRecipients,
				delegate(StaffGroupSummary groupSummary)
				{
					return string.Equals(groupSummary.Name, _selectedRecipient);
				});

			NotifyPropertyChanged("Recipients");
		}

		private List<string> ConsolidateRecipientsForDisplay()
		{
			List<string> recipients = new List<string>();

			foreach (StaffSummary staffSummary in _staffRecipients)
			{
				recipients.Add(PersonNameFormat.Format(staffSummary.Name));
			}
			foreach (StaffGroupSummary groupSummary in _groupRecipients)
			{
				recipients.Add(groupSummary.Name);
			}

			return recipients;
		}

		private void SaveChanges()
		{
			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					AcknowledgeAndReplyRequest request = new AcknowledgeAndReplyRequest(_orderRef, GetOrderNotesToAcknowledge(), GetReply());
					service.AcknowledgeAndReply(request);
				});
		}

		private List<EntityRef> GetOrderNotesToAcknowledge()
		{
			List<Checkable<OrderNoteDetail>> selectedOrderNotes = CollectionUtils.Select<Checkable<OrderNoteDetail>>(
				_notes.Items,
				delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail) { return checkableOrderNoteDetail.IsChecked; });

			return CollectionUtils.Map<Checkable<OrderNoteDetail>, EntityRef>(
				selectedOrderNotes,
				delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail) { return checkableOrderNoteDetail.Item.OrderNoteRef; });
		}

		private OrderNoteDetail GetReply()
		{
			if (string.IsNullOrEmpty(_body)) return null;

			OrderNoteDetail reply = new OrderNoteDetail(OrderNoteCategory.PreliminaryDiagnosis.Key, _body, _staffRecipients, _groupRecipients);
			return reply;
		}

		private bool HasUnacknowledgedNotes()
		{
			return CollectionUtils.Contains(_notes.Items,
				delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail)
				{
					return !checkableOrderNoteDetail.IsChecked && checkableOrderNoteDetail.Item.CanAcknowledge;
				});
		}
	}

	public class PreliminaryDiagnosisOrderNoteTable : Table<Checkable<OrderNoteDetail>>
	{
		private event EventHandler _checkedItemsChanged;

		public PreliminaryDiagnosisOrderNoteTable()
			: base(3)
		{
			TableColumn<Checkable<OrderNoteDetail>, IconSet> canAcknowledgeColumn =
				new TableColumn<Checkable<OrderNoteDetail>, IconSet>("!",
					delegate(Checkable<OrderNoteDetail> item) { return GetCanAcknowledgeIcon(item.Item.CanAcknowledge); },
					0.2f);
			canAcknowledgeColumn.Comparison = delegate(Checkable<OrderNoteDetail> item1, Checkable<OrderNoteDetail> item2)
				{ return item1.Item.CanAcknowledge.CompareTo(item2.Item.CanAcknowledge); };
			canAcknowledgeColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
			this.Columns.Add(canAcknowledgeColumn);

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, bool>(".",
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

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>(SR.ColumnFrom,
				delegate(Checkable<OrderNoteDetail> item) { return PersonNameFormat.Format(item.Item.Author.Name); },
				1.0f));

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, DateTime?>("Posted",
				delegate(Checkable<OrderNoteDetail> item) { return item.Item.PostTime; },
				1.0f));

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>(SR.ColumnTo,
				delegate(Checkable<OrderNoteDetail> item) { return String.Format(SR.FormatTo, RecipientsList(item.Item.StaffRecipients, item.Item.GroupRecipients)); },
				1.0f, 1));

			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, string>("Foo",
				delegate(Checkable<OrderNoteDetail> item) { return item.Item.NoteBody; },
				1.0f, 2));
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
