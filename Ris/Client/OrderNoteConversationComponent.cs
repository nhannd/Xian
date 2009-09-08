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
using ClearCanvas.Ris.Client.Formatting;
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
		#region Private Fields

		private EntityRef _orderRef;

		private readonly List<string> _orderNoteCategories;

		private string _body;

		private bool _urgent;
		private bool _reply;

		private IList<StaffGroupSummary> _onBehalfOfChoices;
		private StaffGroupSummary _onBehalfOf;

		private readonly RecipientTable _recipients;
		private CrudActionModel _recipientsActionModel;
		private Checkable<RecipientTableItem> _selectedRecipient;

		private ILookupHandler _staffLookupHandler;
		private StaffSummary _selectedStaff;

		private ILookupHandler _staffGroupLookupHandler;
		private StaffGroupSummary _selectedStaffGroup;

		private ICannedTextLookupHandler _cannedTextLookupHandler;

		private bool _usingDefaultRecipients;

		private OrderNoteViewComponent _orderNoteViewComponent;
		private ChildComponentHost _orderNotesComponentHost;

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
			_recipients = new RecipientTable(this);
		}

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_staffLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow);
			_staffGroupLookupHandler = new StaffGroupLookupHandler(this.Host.DesktopWindow, false);
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			var defaultRecipients = new List<Checkable<RecipientTableItem>>();
			var orderNotes = new List<OrderNoteDetail>();

			Platform.GetService<IOrderNoteService>(
				service =>
				{
					var formDataRequest =
						new GetConversationEditorFormDataRequest(OrderNoteConversationRecipientsSettingsHelper.Instance.StaffIDs,
						                                         OrderNoteConversationRecipientsSettingsHelper.Instance.StaffGroupNames);
					var formDataResponse = service.GetConversationEditorFormData(formDataRequest);
					_onBehalfOfChoices = formDataResponse.OnBehalfOfGroupChoices;

					if (formDataResponse.RecipientStaffs != null && formDataResponse.RecipientStaffs.Count > 0)
					{
						defaultRecipients.AddRange(
							CollectionUtils.Map(formDataResponse.RecipientStaffs,
							                    (StaffSummary s) => new Checkable<RecipientTableItem>(
							                                        	new RecipientTableItem(s),
							                                        	OrderNoteConversationRecipientsSettingsHelper
							                                        		.Instance.GetCheckState(s))));
					}

					if (formDataResponse.RecipientStaffGroups != null && formDataResponse.RecipientStaffGroups.Count > 0)
					{
						defaultRecipients.AddRange(
							CollectionUtils.Map(formDataResponse.RecipientStaffGroups,
							                    (StaffGroupSummary sg) => new Checkable<RecipientTableItem>(
							                                              	new RecipientTableItem(sg),
							                                              	OrderNoteConversationRecipientsSettingsHelper
							                                              		.Instance.GetCheckState(sg))));
					}

					this.OnBehalfOf = OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName;

					var request = new GetConversationRequest(_orderRef, _orderNoteCategories, false);
					var response = service.GetConversation(request);

					_orderRef = response.OrderRef;

					orderNotes = response.OrderNotes;
				});

			this.Validation.Add(new ValidationRule("SelectedRecipient",
				delegate
				{
					var atLeastOneRecipient = CollectionUtils.Contains(_recipients.Items, checkable => checkable.IsChecked);

					var notPosting = string.IsNullOrEmpty(_body);

					return new ValidationResult(atLeastOneRecipient || notPosting, SR.MessageNoRecipientsSelected);
				}));

			// Set default recipients list
			InitializeRecipients(orderNotes, defaultRecipients);

			// build the action model
			_recipientsActionModel = new CrudActionModel(false, false, true, new ResourceResolver(this.GetType(), true));
			_recipientsActionModel.Delete.SetClickHandler(DeleteRecipient);

			_orderNoteViewComponent = new OrderNoteViewComponent(orderNotes);
			_orderNoteViewComponent.CheckedItemsChanged += delegate { NotifyPropertyChanged("CompleteLabel"); };
			_orderNotesComponentHost = new ChildComponentHost(this.Host, _orderNoteViewComponent);
			_orderNotesComponentHost.StartComponent();

			_reply = this.IsCreatingNewNote || !string.IsNullOrEmpty(_body);

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

		public bool Reply
		{
			get { return _reply; }
			set { _reply = value; }
		}

		public bool HideReply
		{
			get { return !_reply; }
		}

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		public IList<string> OnBehalfOfGroupChoices
		{
			get
			{
				var choices = CollectionUtils.Map(_onBehalfOfChoices, (StaffGroupSummary summary) => summary.Name);
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
					summary => string.Equals(summary.Name, value));

				OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName = _onBehalfOf != null ? _onBehalfOf.Name : string.Empty;
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

			_recipients.Add(_selectedStaff, true);
			NotifyPropertyChanged("Recipients");
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

			_recipients.Add(_selectedStaffGroup, true);
			NotifyPropertyChanged("Recipients");
		}

		#endregion

		public bool HasExistingNotes
		{
			get { return _orderNoteViewComponent.HasExistingNotes; }
		}

		public bool IsCreatingNewNote
		{
			get { return !this.HasExistingNotes || !_orderNoteViewComponent.HasNotesToBeAcknowledged; }
		}

		public string OrderNotesLabel
		{
			get
			{
				return _orderNoteViewComponent.HasNotesToBeAcknowledged
					? SR.TitleConversationHistoryWithCheckBoxes
					: SR.TitleConversationHistory;
			}
		}

		public string CompleteLabel
		{
			get {
				return _orderNoteViewComponent.NotesJustAcknowledged.Count > 0
				       	? (string.IsNullOrEmpty(_body) ? SR.TitleAcknowledge : SR.TitleAcknowledgeAndPost)
				       	: (string.IsNullOrEmpty(_body) ? SR.TitleDone : SR.TitlePost);
			}
		}

		public bool CompleteEnabled
		{
			get { return !_orderNoteViewComponent.HasUnacknowledgedNotes; }
		}

		public void OnComplete()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				SaveChanges();

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow,
					delegate
					{
						this.ExitCode = ApplicationComponentExitCode.Error;
						this.Host.Exit();
					});
			}
		}

		public void OnCancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

		#region Private Methods

		private void InitializeRecipients(IEnumerable<OrderNoteDetail> notes, IEnumerable<Checkable<RecipientTableItem>> defaultRecipients)
		{
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

			// Load Default recipients
			if (_recipients.Items.Count == 0)
			{
				_recipients.Items.AddRange(defaultRecipients);
				_usingDefaultRecipients = true;
			}
		}

		private static bool IsStaffCurrentUser(StaffSummary staff)
		{
			return string.Equals(PersonNameFormat.Format(staff.Name), PersonNameFormat.Format(LoginSession.Current.FullName));
		}

		private void SaveChanges()
		{
			var orderNoteRefsToBeAcknowledged = CollectionUtils.Map(
				_orderNoteViewComponent.NotesJustAcknowledged, (OrderNoteDetail note) => note.OrderNoteRef);

			Platform.GetService<IOrderNoteService>(
				service => service.AcknowledgeAndPost(new AcknowledgeAndPostRequest(_orderRef, orderNoteRefsToBeAcknowledged, GetReply())));

			if (_usingDefaultRecipients)
			{
				OrderNoteConversationRecipientsSettingsHelper.Instance.DefaultRecipients = 
					CollectionUtils.Map(_recipients.Items,
					                    (Checkable<RecipientTableItem> item) =>
					                    {
					                    	var setting = new RecipientSettings {Checked = item.IsChecked};
					                    	if (item.Item.IsStaffRecipient)
					                    		setting.StaffId = item.Item.StaffSummary.StaffId;
					                    	else
					                    		setting.StaffGroupName = item.Item.StaffGroupSummary.Name;

					                    	return setting;
					                    });

				OrderNoteConversationRecipientsSettingsHelper.Instance.Save();
			}
		}

		private OrderNoteDetail GetReply()
		{
			// if Reply is unchecked or the body is empty, there is no reply to send.
			if (!_reply || string.IsNullOrEmpty(_body)) return null;

			var reply = new OrderNoteDetail(
				OrderNoteCategory.PreliminaryDiagnosis.Key, 
				_body, 
				_onBehalfOf,
				_urgent,
				_recipients.SelectedStaff, 
				_recipients.SelectedStaffGroups);

			return reply;
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
