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
			private DesktopWindow _desktopWindow;

			internal StaffAndGroupLookupHandler(DesktopWindow desktopWindow)
				: base(new ILookupHandler[] { new StaffLookupHandler(desktopWindow), new StaffGroupLookupHandler(desktopWindow, false) })
			{
				_desktopWindow = desktopWindow;
			}

			protected override bool ResolveNameInteractive(string query, out object result)
			{
				result = null;
				var component = new StaffOrStaffGroupSummaryComponent();

				ApplicationComponentExitCode exitCode = LaunchAsDialog(
					_desktopWindow, component, SR.TitleStaffOrStaffGroups);

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					if (component.IsSelectingStaff)
						result = component.SelectedStaff;
					else
						result = component.SelectedStaffGroup;
				}

				return (result != null);
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
		private string _templateId;

		private readonly List<SoftKeyData> _softKeys;
		private readonly List<string> _orderNoteCategories;

		private string _body;

		private bool _urgent;

		private IList<StaffGroupSummary> _onBehalfOfChoices;
		private StaffGroupSummary _onBehalfOf;

		private RecipientTable _recipients;
		private CrudActionModel _recipientsActionModel;
		private Checkable<RecipientTableItem> _selectedRecipient;

		private ICannedTextLookupHandler _cannedTextLookupHandler;

		private OrderNoteViewComponent _orderNoteViewComponent;
		private ChildComponentHost _orderNotesComponentHost;

		private readonly StaffGroupSummary _emptyStaffGroup = new StaffGroupSummary();

		private event EventHandler _newRecipientAdded;

		#endregion

		#region Constructors

		public OrderNoteConversationComponent(EntityRef orderRef, string orderNoteCategory, string templateId)
			: this(orderRef, new[] { orderNoteCategory }, templateId)
		{
		}

		public OrderNoteConversationComponent(EntityRef orderRef, IEnumerable<string> orderNoteCategories, string templateId)
		{
			_orderRef = orderRef;
			_softKeys = new List<SoftKeyData>();
			_orderNoteCategories = orderNoteCategories != null ? new List<string>(orderNoteCategories) : new List<string>();
			_templateId = templateId;

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

			// init recip table here, and not in constructor, because it relies on Host being set
			_recipients = new RecipientTable(this);

			// load template if specified
			var template = _templateId == null ? null : LoadTemplate(_templateId);
			if(template != null) 
				_softKeys.AddRange(template.SoftKeys);

			// load the existing conversation, plus editor form data
			var orderNotes = new List<OrderNoteDetail>();
			GetConversationEditorFormDataResponse formDataResponse = null;
			Platform.GetService<IOrderNoteService>(
				service =>
				{
					var formDataRequest = new GetConversationEditorFormDataRequest(
						template != null ? template.GetStaffRecipients() : new List<string>(),
						template != null ? template.GetGroupRecipients() : new List<string>());
					formDataResponse = service.GetConversationEditorFormData(formDataRequest);

					var request = new GetConversationRequest(_orderRef, _orderNoteCategories, false);
					var response = service.GetConversation(request);

					_orderRef = response.OrderRef;
					orderNotes = response.OrderNotes;
				});


			// init on-behalf of choices
			_onBehalfOfChoices = formDataResponse.OnBehalfOfGroupChoices;
			_onBehalfOfChoices.Insert(0, _emptyStaffGroup);

			// Set default recipients list, either based on the existing conversation, or from template if new conversation
			InitializeRecipients(orderNotes, formDataResponse.RecipientStaffs, formDataResponse.RecipientStaffGroups);

			// Set default on-behalf of
			InitializeOnBehalfOf(template, orderNotes);

			// build the action model
			_recipientsActionModel = new CrudActionModel(true, false, true, new ResourceResolver(this.GetType(), true));
			_recipientsActionModel.Add.SetClickHandler(AddRecipient);
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
				NotifyPropertyChanged("Body");
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

		public event EventHandler NewRecipientAdded
		{
			add { _newRecipientAdded += value; }
			remove { _newRecipientAdded -= value; }
		}

		public void AddRecipient()
		{
			// try to select an existing blank cell
			_selectedRecipient = CollectionUtils.SelectFirst(_recipients.Items, i => i.Item.Recipient == null);

			// if none, then add one
			if(_selectedRecipient == null)
			{
				_selectedRecipient = _recipients.AddNew(true);
			}

			// inform view to select the cell
			OnSelectedRecipientChanged();

			// inform view to begin editing the cell
			EventsHelper.Fire(_newRecipientAdded, this, EventArgs.Empty);
		}

		public bool SoftKeysVisible
		{
			get { return _softKeys.Count > 0; }
		}

		public IList<string> SoftKeyNames
		{
			get { return CollectionUtils.Map<SoftKeyData, string>(_softKeys, key => key.ButtonName); }
		}

		public void ApplySoftKey(string softKeyName)
		{
			var softKey = CollectionUtils.SelectFirst(_softKeys, key => Equals(key.ButtonName, softKeyName));
			this.Body = softKey.InsertText;
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

		private void InitializeOnBehalfOf(TemplateData template, List<OrderNoteDetail> orderNotes)
		{
			// if this is a new note, and we have a template, use the template,
			// otherwise use the saved setting
			string groupName;
			if (orderNotes.Count == 0 && template != null)
			{
				// take from template, and update the user prefs
				groupName = template.OnBehalfOfGroup;
				OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName = groupName;
				OrderNoteConversationComponentSettings.Default.Save();
			}
			else
			{
				groupName = OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName;
			}

			_onBehalfOf = CollectionUtils.SelectFirst(_onBehalfOfChoices, group => group.Name == groupName);
		}

		private void InitializeRecipients(List<OrderNoteDetail> orderNotes, List<StaffSummary> staffs, List<StaffGroupSummary> groups)
		{
			// if there are existing notes, use some rules to initialize recipients
			if(orderNotes.Count > 0)
			{
				// rules:
				// 1. if note was sent on behalf of a group, the note should be posted back to that group by default
				// 2. if note was not send on behalf of a group, it should be posted back to it's author by default
				// 3. the note should be posted back to all group recipients, and all staff recipients excluding the
				// current user (effectively "reply all")

				foreach (var note in orderNotes)
				{
					if (note.OnBehalfOfGroup != null)
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
			else
			{
				// otherwise, this is a new note, so initialize recipients from template data
				if (staffs != null)
				{
					foreach (var item in staffs)
					{
						_recipients.Add(item, true);
					}
				}
				if (groups != null)
				{
					foreach (var item in groups)
					{
						_recipients.Add(item, true);
					}
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
					_recipients.CheckStaff,
					_recipients.CheckedStaffGroups);
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
			NotifyPropertyChanged("SelectedRecipient");
		}

		#endregion
	}
}
