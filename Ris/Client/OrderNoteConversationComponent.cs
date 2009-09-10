#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Validation;

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
	public partial class OrderNoteConversationComponent : ApplicationComponent
	{
		#region StaffAndGroupLookupHandler class

		class StaffAndGroupLookupHandler : LookupHandlerAggregator
		{
			internal StaffAndGroupLookupHandler(DesktopWindow desktopWindow)
				: base(new ILookupHandler[] { new StaffLookupHandler(desktopWindow), new StaffGroupLookupHandler(desktopWindow, false) })
			{

			}

			protected override bool ResolveNameInteractive(string query, out object result)
			{
				throw new NotImplementedException();
			}

			public override string FormatItem(object item)
			{
				var s = base.FormatItem(item);
				if (item is StaffGroupSummary)
					s += " (Staff Group)";
				return s;
			}
		}

		#endregion

		#region Private Fields

		private EntityRef _orderRef;

		private readonly List<string> _orderNoteCategories;

		private string _body;

		private bool _urgent;

		private IList<StaffGroupSummary> _onBehalfOfChoices;
		private StaffGroupSummary _onBehalfOf;

		private RecipientTable _recipients;
		private CrudActionModel _recipientsActionModel;
		private Checkable<RecipientTableItem> _selectedRecipient;

		private ILookupHandler _recipientLookupHandler;
		private object _recipientLookupSelection;

		private ICannedTextLookupHandler _cannedTextLookupHandler;

		private OrderNoteViewComponent _orderNoteViewComponent;
		private ChildComponentHost _orderNotesComponentHost;

		private readonly StaffGroupSummary _emptyStaffGroup = new StaffGroupSummary();

		#endregion

		#region Constructors

		public OrderNoteConversationComponent(EntityRef orderRef, string orderNoteCategory)
			: this(orderRef, new [] { orderNoteCategory })
		{
		}

		public OrderNoteConversationComponent(EntityRef orderRef, IEnumerable<string> orderNoteCategories)
		{
			_orderRef = orderRef;
			_orderNoteCategories = orderNoteCategories != null ? new List<string>(orderNoteCategories) : new List<string>();

			this.Validation.Add(new ValidationRule("SelectedRecipient",
				delegate
				{
					// if body is non-empty (a new note is being posted), must have at least 1 recip
					var atLeastOneRecipient = CollectionUtils.Contains(_recipients.Items, r => r.IsChecked);
					return new ValidationResult(atLeastOneRecipient || IsBodyEmpty, SR.MessageNoRecipientsSelected);
				}));

		}

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			// init lookup handlers
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);
			_recipientLookupHandler = new StaffAndGroupLookupHandler(this.Host.DesktopWindow);

			// init recip table here, and not in constructor, because it relies on Host being set
			_recipients = new RecipientTable(this);

			// load the existing conversation, plus editor form data
			var templateRecipients = new List<Checkable<RecipientTableItem>>();
			var orderNotes = new List<OrderNoteDetail>();
			GetConversationEditorFormDataResponse formDataResponse = null;
			Platform.GetService<IOrderNoteService>(
				service =>
				{
					var formDataRequest =
						new GetConversationEditorFormDataRequest(new List<string>(), new List<string>());
					formDataResponse = service.GetConversationEditorFormData(formDataRequest);

					var request = new GetConversationRequest(_orderRef, _orderNoteCategories, false);
					var response = service.GetConversation(request);

					_orderRef = response.OrderRef;
					orderNotes = response.OrderNotes;
				});


			// init on-behalf of choices
			_onBehalfOfChoices = formDataResponse.OnBehalfOfGroupChoices;
			_onBehalfOfChoices.Insert(0, _emptyStaffGroup);
			_onBehalfOf = CollectionUtils.SelectFirst(_onBehalfOfChoices,
				group => group.Name == OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName);

			// Set default recipients list, either based on the existing conversation, or from template if new conversation
			if(orderNotes.Count > 0)
			{
				InitializeRecipients(orderNotes);
			}
			else
			{
				if (formDataResponse.RecipientStaffs != null && formDataResponse.RecipientStaffs.Count > 0)
				{
					templateRecipients.AddRange(
						CollectionUtils.Map(formDataResponse.RecipientStaffs,
											(StaffSummary s) => new Checkable<RecipientTableItem>(
																	new RecipientTableItem(s),
																	true)));
				}

				if (formDataResponse.RecipientStaffGroups != null && formDataResponse.RecipientStaffGroups.Count > 0)
				{
					templateRecipients.AddRange(
						CollectionUtils.Map(formDataResponse.RecipientStaffGroups,
											(StaffGroupSummary sg) => new Checkable<RecipientTableItem>(
																		new RecipientTableItem(sg),
																		true)));
				}
			}

			// build the action model
			_recipientsActionModel = new CrudActionModel(false, false, true, new ResourceResolver(this.GetType(), true));
			_recipientsActionModel.Delete.SetClickHandler(DeleteRecipient);

			// init conversation view component
			_orderNoteViewComponent = new OrderNoteViewComponent(orderNotes);
			_orderNoteViewComponent.CheckedItemsChanged += delegate { NotifyPropertyChanged("CompleteButtonLabel"); };
			_orderNotesComponentHost = new ChildComponentHost(this.Host, _orderNoteViewComponent);
			_orderNotesComponentHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_orderNotesComponentHost != null)
			{
				_orderNotesComponentHost.StopComponent();
				_orderNotesComponentHost = null;
			}

			base.Stop();
		}

		#endregion

		#region Presentation Model

		public ApplicationComponentHost OrderNotesHost
		{
			get { return _orderNotesComponentHost; }
		}

		public string Body
		{
			get { return _body; }
			set
			{
				_body = value;
			}
		}

		public bool Urgent
		{
			get { return _urgent; }
			set { _urgent = value; }
		}

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		public IList<StaffGroupSummary> OnBehalfOfGroupChoices
		{
			get
			{
				return _onBehalfOfChoices;
            }
		}

		public string FormatOnBehalfOf(object item)
		{
			var s = item == null ? null : ((StaffGroupSummary) item).Name;
			return s ?? "";
		}

		public StaffGroupSummary OnBehalfOf
		{
			get
			{
				return _onBehalfOf;
			}
			set
			{
				if(!Equals(value, _onBehalfOf))
				{
					_onBehalfOf = value;
					NotifyPropertyChanged("OnBehalfOf");
					OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName = _onBehalfOf != null ? _onBehalfOf.Name : string.Empty;
				}
			}
		}

		public ITable Recipients
		{
			get { return _recipients; }
		}

		public ActionModelNode RecipientsActionModel
		{
			get { return _recipientsActionModel; }
		}

		public ISelection SelectedRecipient
		{
			get { return new Selection(_selectedRecipient); }
			set
			{
				if (value.Item != _selectedRecipient)
				{
					_selectedRecipient = (Checkable<RecipientTableItem>)value.Item;
				}

				OnSelectedRecipientChanged();
			}
		}

		public ILookupHandler RecipientLookupHandler
		{
			get { return _recipientLookupHandler; }
		}

		public object RecipientLookupSelection
		{
			get { return _recipientLookupSelection; }
			set { _recipientLookupSelection = value; }
		}

		public bool AddRecipientEnabled
		{
			get { return _recipientLookupSelection != null; }
		}

		public void AddRecipient()
		{
			if (_recipientLookupSelection == null) return;

			_recipients.Add(_recipientLookupSelection, true);
		}

		public string OrderNotesLabel
		{
			get
			{
				return _orderNoteViewComponent.HasAcknowledgeableNotes
					? SR.TitleConversationHistoryWithCheckBoxes
					: SR.TitleConversationHistory;
			}
		}

		public string CompleteButtonLabel
		{
			get
			{
				return _orderNoteViewComponent.CheckedNotes.Count > 0
						? (IsBodyEmpty ? SR.TitleAcknowledge : SR.TitleAcknowledgeAndPost)
						: SR.TitlePost;
			}
		}

		public bool CompleteButtonEnabled
		{
			get
			{
				return (_orderNoteViewComponent.CheckedNotes.Count != 0 || !IsBodyEmpty)
					&& _orderNoteViewComponent.AllAcknowledgeableNotesAreChecked;
			}
		}

		public void AcknowledgeAndPost()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				SaveChanges();

				Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow,
				                        () => Exit(ApplicationComponentExitCode.Error));
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		#region Private Methods

		private bool IsBodyEmpty
		{
			get { return string.IsNullOrEmpty(_body); }
		}

		private void InitializeRecipients(IEnumerable<OrderNoteDetail> notes)
		{
			// rules:
			// 1. if note was sent on behalf of a group, the note should be posted back to that group by default
			// 2. if note was not send on behalf of a group, it should be posted back to it's author by default
			// 3. the note should be posted back to all group recipients, and all staff recipients excluding the
			// current user (effectively "reply all")

			foreach (var note in notes)
			{
				if(note.OnBehalfOfGroup != null)
				{
					_recipients.Add(note.OnBehalfOfGroup, note.CanAcknowledge);
					_recipients.Add(note.Author, false);
				}
				else
				{
					_recipients.Add(note.Author, note.CanAcknowledge);
				}

				foreach (var staffRecipient in note.StaffRecipients)
				{
					if (!IsStaffCurrentUser(staffRecipient.Staff))
					{
						_recipients.Add(staffRecipient.Staff, false);
					}
				}
				foreach (var groupRecipient in note.GroupRecipients)
				{
					_recipients.Add(groupRecipient.Group, false);
				}
			}
		}

		private static bool IsStaffCurrentUser(StaffSummary staff)
		{
			return string.Equals(staff.StaffId, LoginSession.Current.Staff.StaffId);
		}

		private void SaveChanges()
		{
			var orderNoteRefsToBeAcknowledged = CollectionUtils.Map(
				_orderNoteViewComponent.CheckedNotes, (OrderNoteDetail note) => note.OrderNoteRef);

			Platform.GetService<IOrderNoteService>(
				service => service.AcknowledgeAndPost(new AcknowledgeAndPostRequest(_orderRef, orderNoteRefsToBeAcknowledged, GetReply())));
		}

		private OrderNoteDetail GetReply()
		{
			// if Reply is unchecked or the body is empty, there is no reply to send.
			if (!IsBodyEmpty)
			{
				return new OrderNoteDetail(
					OrderNoteCategory.PreliminaryDiagnosis.Key,
					_body,
					_onBehalfOf == _emptyStaffGroup ? null : _onBehalfOf,
					_urgent,
					_recipients.SelectedStaff,
					_recipients.SelectedStaffGroups);
			}
			return null;
		}

		private void DeleteRecipient()
		{
			if (_selectedRecipient == null) 
				return;

			_recipients.Items.Remove(_selectedRecipient);
			this.SelectedRecipient = Selection.Empty;
		}

		private void OnSelectedRecipientChanged()
		{
			_recipientsActionModel.Delete.Enabled = _selectedRecipient != null;
		}

		#endregion
	}
}
