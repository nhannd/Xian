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
	/// Extension point for views onto <see cref="OrderNoteConversationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class OrderNoteConversationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PreliminaryDiagnosisConversationComponent class.
	/// </summary>
	[AssociateView(typeof(OrderNoteConversationComponentViewExtensionPoint))]
	public class OrderNoteConversationComponent : ApplicationComponent
	{
		#region Private Fields

		private readonly EntityRef _orderRef;

		private readonly List<string> _orderNoteCategories;
		private readonly OrderNoteConversationTable _notes;
		private Checkable<OrderNoteDetail> _selectedNote;

		private string _body;

		private IList<StaffGroupSummary> _onBehalfOfChoices;
		private StaffGroupSummary _onBehalfOf;

		private readonly Table<string> _recipients;
		private string _selectedRecipient;
		private readonly CrudActionModel _recipientsActionModel;

		private ILookupHandler _staffLookupHandler;
		private StaffSummary _selectedStaff = null;
		private readonly List<StaffSummary> _staffRecipients;

		private ILookupHandler _staffGroupLookupHandler;
		private StaffGroupSummary _selectedStaffGroup = null;
		private readonly List<StaffGroupSummary> _groupRecipients;

		#endregion

		#region Constructors

		public OrderNoteConversationComponent(EntityRef orderRef, string orderNoteCategory)
			: this(orderRef, new string[] { orderNoteCategory })
		{
		}

		public OrderNoteConversationComponent(EntityRef orderRef, IEnumerable<string> orderNoteCategories)
		{
			_orderRef = orderRef;

			_orderNoteCategories = orderNoteCategories != null ? new List<string>(orderNoteCategories) : new List<string>();

			_notes = new OrderNoteConversationTable();
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

		#endregion

		#region ApplicationComponent overrides

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
					GetConversationEditorFormDataResponse formDataResponse = service.GetConversationEditorFormData(new GetConversationEditorFormDataRequest());
					_onBehalfOfChoices = formDataResponse.OnBehalfOfGroupChoices;

					this.OnBehalfOf = OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName;

					GetConversationRequest request = new GetConversationRequest(_orderRef, _orderNoteCategories, false);
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

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		#endregion

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

		public IList<string> OnBehalfOfGroupChoices
		{
			get
			{
				List<string> choices = CollectionUtils.Map<StaffGroupSummary, string>(
					_onBehalfOfChoices, 
					delegate(StaffGroupSummary summary) { return summary.Name; });
				choices.Insert(0, string.Empty);
				return choices;
			}
		}

		public string OnBehalfOf
		{
			get
			{
				return _onBehalfOf != null ? _onBehalfOf.Name : string.Empty;
			}
			set
			{
				_onBehalfOf = CollectionUtils.SelectFirst(
					_onBehalfOfChoices,
					delegate(StaffGroupSummary summary) { return string.Equals(summary.Name, value); });

				OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName = _onBehalfOf != null ? _onBehalfOf.Name : string.Empty;
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

		public string CompleteLabel
		{
			get
			{
				string label;
				if (HasUnacknowledgedNotes())
				{
					label = string.IsNullOrEmpty(_body) ? "Acknowledge" : "Acknowledge and Post";
				}
				else
				{
					label = string.IsNullOrEmpty(_body) && _notes.Items.Count != 0 ? "OK" : "Post";
				}
				return label;
			}
		}

		public bool CompleteEnabled
		{
			get
			{
				if (_notes.Items.Count == 0)
				{
					return !string.IsNullOrEmpty(_body);
				}
				else
				{
					return !HasUncheckedUnacknowledgedNotes();
				}
			}
		}

		public void OnComplete()
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

		public void OnCancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

		#region Private Methods

		private void AddDefaultRecipients(IEnumerable<OrderNoteDetail> notes)
		{
			foreach (OrderNoteDetail note in notes)
			{
				if(note.OnBehalfOfGroup != null)
				{
					AddUnique(note.OnBehalfOfGroup);
				}
				else
				{
					AddUnique(note.Author);
				}

				foreach (OrderNoteDetail.StaffRecipientDetail staffRecipient in note.StaffRecipients)
				{
					if (!IsStaffCurrentUser(staffRecipient.Staff))
					{
						AddUnique(staffRecipient.Staff);
					}
				}
				foreach (OrderNoteDetail.GroupRecipientDetail groupRecipient in note.GroupRecipients)
				{
					AddUnique(groupRecipient.Group);
				}
			}
		}

		private static bool IsStaffCurrentUser(StaffSummary staff)
		{
			return string.Equals(PersonNameFormat.Format(staff.Name), PersonNameFormat.Format(LoginSession.Current.FullName));
		}

		private void AddUnique(StaffSummary proposedStaffRecipient)
		{
			if (!CollectionUtils.Contains(
					_staffRecipients,
					delegate(StaffSummary existingRecipient)
					{
						return string.Equals(PersonNameFormat.Format(existingRecipient.Name),
											 PersonNameFormat.Format(proposedStaffRecipient.Name));
					}))
			{
				_staffRecipients.Add(proposedStaffRecipient);
			}
		}

		private void AddUnique(StaffGroupSummary proposedGroupRecipient)
		{
			if (!CollectionUtils.Contains(
					_groupRecipients,
					delegate(StaffGroupSummary groupSummary)
					{
						return string.Equals(proposedGroupRecipient.Name, groupSummary.Name);
					}))
			{
				_groupRecipients.Add(proposedGroupRecipient);
			}
		}

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
					AcknowledgeAndPostRequest request = new AcknowledgeAndPostRequest(_orderRef, GetOrderNotesToAcknowledge(), GetReply());
					service.AcknowledgeAndPost(request);
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

			OrderNoteDetail reply = new OrderNoteDetail(OrderNoteCategory.PreliminaryDiagnosis.Key, _body, _onBehalfOf, _staffRecipients, _groupRecipients);
			return reply;
		}

		private bool HasUncheckedUnacknowledgedNotes()
		{
			return CollectionUtils.Contains(_notes.Items,
											delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail)
											{
												return !checkableOrderNoteDetail.IsChecked && checkableOrderNoteDetail.Item.CanAcknowledge;
											});
		}

		private bool HasUnacknowledgedNotes()
		{
			return CollectionUtils.Contains(_notes.Items,
											delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail)
											{
												return checkableOrderNoteDetail.Item.CanAcknowledge;
											});
		}

		#endregion
	}
}
