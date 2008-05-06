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

		private ILookupHandler _staffLookupHandler;
		private StaffSummary _selectedStaff = null;
		private readonly List<StaffSummary> _staffRecipients;

		private ILookupHandler _staffGroupLookupHandler;
		private StaffGroupSummary _selectedStaffGroup = null;
		private readonly List<StaffGroupSummary> _groupRecipients;

		private bool _defaultRecpientsAdded = false;

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

			_staffRecipients = new List<StaffSummary>();
			_groupRecipients = new List<StaffGroupSummary>();
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			// TODO prepare the component for its live phase
			base.Start();

			_staffLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow);
			_staffGroupLookupHandler = new StaffGroupLookupHandler(this.Host.DesktopWindow);

			try
			{
				Platform.GetService<IOrderNoteService>(
					delegate(IOrderNoteService service)
					{
						GetConversationRequest request = new GetConversationRequest(_orderRef);
						GetConversationResponse response = service.GetConversation(request);

						IList<Checkable<OrderNoteDetail>> checkableOrderNoteDetails =
							CollectionUtils.Map<OrderNoteDetail, Checkable<OrderNoteDetail>>(
								response.OrderNotes,
								delegate(OrderNoteDetail detail)
								{
									return new Checkable<OrderNoteDetail>(detail);
								});

						_notes.Items.AddRange(checkableOrderNoteDetails);
					});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
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

			bool contains = CollectionUtils.Contains(_staffRecipients,
				delegate(StaffSummary staff) { return staff == _selectedStaff; });

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

			bool contains = CollectionUtils.Contains(_groupRecipients,
				delegate(StaffGroupSummary group) { return group == _selectedStaffGroup; });

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
			if(string.IsNullOrEmpty(_body)) return null;

			OrderNoteDetail reply = new OrderNoteDetail(OrderNoteCategory.PreliminaryDiagnosis.Key, _body, _staffRecipients, _groupRecipients);
			return reply;
		}

		private bool HasUnacknowledgedNotes()
		{
			return CollectionUtils.Contains(_notes.Items,
				delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail)
					{
						return checkableOrderNoteDetail.IsChecked == false && ShouldAcknowledge(checkableOrderNoteDetail.Item);
					});
		}

		// TODO:  this should be moved to server and be a property of the OrderNoteDetail
		private static bool ShouldAcknowledge(OrderNoteDetail detail)
		{
			bool shouldAcknowledge = false;

			shouldAcknowledge |= CollectionUtils.Contains(
				detail.StaffRecipients,
				delegate(OrderNoteDetail.StaffRecipientDetail staffRecipientDetail)
				{
					return staffRecipientDetail.IsAcknowledged == false
							&& string.Equals(PersonNameFormat.Format(staffRecipientDetail.Staff.Name),
					                     PersonNameFormat.Format(LoginSession.Current.FullName));
				});

			//foreach (OrderNoteDetail.GroupRecipientDetail groupRecipient in detail.GroupRecipients)
			//{
			//}

			return shouldAcknowledge;
		}
	}

	public class PreliminaryDiagnosisOrderNoteTable : Table<Checkable<OrderNoteDetail>>
	{
		private event EventHandler _checkedItemsChanged;

		public PreliminaryDiagnosisOrderNoteTable()
			: base(3)
		{
			this.Columns.Add(new TableColumn<Checkable<OrderNoteDetail>, bool>(".",
				delegate(Checkable<OrderNoteDetail> item) { return item.IsChecked; },
				delegate(Checkable<OrderNoteDetail> item, bool value)
				{
					item.IsChecked = value;
					EventsHelper.Fire(_checkedItemsChanged, this, EventArgs.Empty);
				}, 
				0.20f));

			//TableColumn<OrderNoteDetail, IconSet> acknowledgedColumn =
			//    new TableColumn<OrderNoteDetail, IconSet>(SR.ColumnStatus,
			//        delegate(OrderNoteDetail item) { return GetIsAcknowledgedIcon(item.IsAcknowledged); },
			//        0.3f);
			//acknowledgedColumn.Comparison = delegate(OrderNoteDetail item1, OrderNoteDetail item2)
			//    { return item1.IsAcknowledged.CompareTo(item2.IsAcknowledged); };
			//acknowledgedColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
			//this.Columns.Add(acknowledgedColumn);

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

		private static IconSet GetIsAcknowledgedIcon(bool isAcknowledged)
		{
			return isAcknowledged ? new IconSet("NoteRead.png") : new IconSet("NoteUnread.png");
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
