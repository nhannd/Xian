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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the order editor.
	/// </summary>
	public interface IOrderEditorPageProvider : IExtensionPageProvider<IOrderEditorPage, IOrderEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor
	/// context.
	/// </summary>
	public interface IOrderEditorContext
	{
		/// <summary>
		/// Patient ref.
		/// </summary>
		EntityRef PatientRef { get; }

		/// <summary>
		/// Patient Profile ref.
		/// </summary>
		EntityRef PatientProfileRef { get; }

		/// <summary>
		/// Order ref.
		/// </summary>
		EntityRef OrderRef { get; }

		/// <summary>
		/// Exposes the extended properties associated with the Order.  Modifications made to these
		/// properties by the editor page will be persisted whenever the order editor is saved.
		/// </summary>
		IDictionary<string, string> OrderExtendedProperties { get; }
	}

	/// <summary>
	/// Defines an interface to a custom order editor page.
	/// </summary>
	public interface IOrderEditorPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the order editor.
	/// </summary>
	[ExtensionPoint]
	public class OrderEditorPageProviderExtensionPoint : ExtensionPoint<IOrderEditorPageProvider>
	{
	}

	/// <summary>
	/// Extension point for views onto <see cref="OrderEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class OrderEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// OrderEditorComponent class
	/// </summary>
	[AssociateView(typeof(OrderEditorComponentViewExtensionPoint))]
	public class OrderEditorComponent : ApplicationComponent
	{
		public enum Mode
		{
			NewOrder,
			ModifyOrder,
			ReplaceOrder
		}

		#region HealthcareContext

		/// <summary>
		/// Define a helper class to for DHTML components.
		/// </summary>
		[DataContract]
		class HealthcareContext : DataContractBase
		{
			public HealthcareContext(EntityRef patientRef, EntityRef profileRef, EntityRef orderRef)
			{
				this.PatientRef = patientRef;
				this.PatientProfileRef = profileRef;
				this.OrderRef = orderRef;
			}

			[DataMember]
			public EntityRef PatientRef;

			[DataMember]
			public EntityRef PatientProfileRef;

			[DataMember]
			public EntityRef OrderRef;
		}

		#endregion

		#region OrderEditorContext

		class OrderEditorContext : IOrderEditorContext
		{
			private readonly OrderEditorComponent _owner;

			public OrderEditorContext(OrderEditorComponent owner)
			{
				_owner = owner;
			}

			public EntityRef PatientRef
			{
				get { return _owner._patientRef; }
			}

			public EntityRef PatientProfileRef
			{
				get { return _owner._profileRef; }
			}

			public EntityRef OrderRef
			{
				get { return _owner._orderRef; }
			}

			public IDictionary<string, string> OrderExtendedProperties
			{
				get { return _owner._extendedProperties; }
			}
		}

		#endregion

		#region Private fields

		private readonly Mode _mode;
		private EntityRef _patientRef;
		private readonly EntityRef _profileRef;
		private EntityRef _orderRef;

		private List<VisitSummary> _activeVisits;
		private event EventHandler _activeVisitsChanged;
		private VisitSummary _selectedVisit;

		private DiagnosticServiceLookupHandler _diagnosticServiceLookupHandler;

		private List<FacilitySummary> _facilityChoices;
		private List<EnumValueInfo> _priorityChoices;
		private List<EnumValueInfo> _cancelReasonChoices;

		private FacilitySummary _orderingFacility;

		private ExternalPractitionerLookupHandler _orderingPractitionerLookupHandler;
		private ExternalPractitionerSummary _selectedOrderingPractitioner;
		private ExternalPractitionerContactPointDetail _selectedOrderingPractitionerContactPoint;
		private List<ExternalPractitionerContactPointDetail> _orderingPractitionerContactPointChoices;

		private EnumValueInfo _selectedPriority;
		private EnumValueInfo _selectedCancelReason;

		private DiagnosticServiceSummary _selectedDiagnosticService;

		private DateTime? _schedulingRequestTime;

		private readonly Table<ProcedureRequisition> _proceduresTable;
		private readonly CrudActionModel _proceduresActionModel;
		private ProcedureRequisition _selectedProcedure;

		private readonly Table<ResultRecipientDetail> _recipientsTable;
		private readonly CrudActionModel _recipientsActionModel;
		private ResultRecipientDetail _selectedRecipient;
		private ExternalPractitionerLookupHandler _recipientLookupHandler;
		private ExternalPractitionerSummary _recipientToAdd;
		private ExternalPractitionerContactPointDetail _recipientContactPointToAdd;
		private List<ExternalPractitionerContactPointDetail> _recipientContactPointChoices;

		private string _indication;
		private List<EnumValueInfo> _lateralityChoices;

		private event EventHandler _changeCommitted;

		private readonly AttachedDocumentPreviewComponent _attachmentSummaryComponent;
		private readonly List<OrderAttachmentSummary> _newAttachments = new List<OrderAttachmentSummary>();
		private readonly OrderAdditionalInfoComponent _orderAdditionalInfoComponent;

		private TabComponentContainer _rightHandComponentContainer;
		private ChildComponentHost _rightHandComponentContainerHost;

		private readonly OrderNoteSummaryComponent _noteSummaryComponent;
		private ChildComponentHost _orderNoteSummaryComponentHost;

		private ChildComponentHost _bannerComponentHost;

		private List<IOrderEditorPage> _extensionPages;
		private Dictionary<string, string> _extendedProperties = new Dictionary<string, string>();

		private string _downtimeAccessionNumber;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor for creating a new order.
		/// </summary>
		public OrderEditorComponent(EntityRef patientRef, EntityRef profileRef)
			: this(patientRef, profileRef, null, Mode.NewOrder)
		{
		}

		/// <summary>
		/// Constructor for creating a new order with attachments.
		/// </summary>
		public OrderEditorComponent(EntityRef patientRef, EntityRef profileRef, List<OrderAttachmentSummary> attachments)
			: this(patientRef, profileRef, null, Mode.NewOrder)
		{
			_newAttachments = attachments;
		}

		/// <summary>
		/// Constructor for adding attachments to an existing order.
		/// </summary>
		public OrderEditorComponent(EntityRef patientRef, EntityRef profileRef, EntityRef orderRef, List<OrderAttachmentSummary> attachments)
			: this(patientRef, profileRef, orderRef, Mode.ModifyOrder)
		{
			_newAttachments = attachments;
		}

		/// <summary>
		/// Constructor for modifying or replacing an order.
		/// </summary>
		/// <param name="patientRef"></param>
		/// <param name="profileRef"></param>
		/// <param name="orderRef"></param>
		/// <param name="mode"></param>
		public OrderEditorComponent(EntityRef patientRef, EntityRef profileRef, EntityRef orderRef, Mode mode)
		{
			Platform.CheckForNullReference(patientRef, "patientRef");

			_mode = mode;
			if (mode == Mode.ModifyOrder || mode == Mode.ReplaceOrder)
				Platform.CheckForNullReference(orderRef, "orderRef");

			_patientRef = patientRef;
			_profileRef = profileRef;
			_orderRef = orderRef;

			_proceduresTable = new Table<ProcedureRequisition>();
			_proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, string>("Name", item => item.ProcedureType.Name));
			_proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, string>("Facility", item =>
				item.PerformingFacility == null
				? ""
				: item.PerformingFacility.Code));
			_proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, string>("Lat.", item =>
				(item.Laterality == null || item.Laterality.Code == "N")
				? ""
				: item.Laterality.Value));
			_proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, bool>("Port.", item => item.PortableModality));
			_proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, string>("Scheduled Time", item =>
				{
					// if new or scheduled
					if (item.Status != null && item.Status.Code != "SC")
						return item.Status.Value;

					if (item.Cancelled)
						return "Cancel Pending";

					return Format.DateTime(item.ScheduledTime);
				}));

			_proceduresActionModel = new CrudActionModel();
			_proceduresActionModel.Add.SetClickHandler(AddProcedure);
			_proceduresActionModel.Edit.SetClickHandler(EditSelectedProcedure);
			_proceduresActionModel.Delete.SetClickHandler(CancelSelectedProcedure);

			// in "modify" mode, the Delete action is actually a Cancel action
			if (_mode == Mode.ModifyOrder)
				_proceduresActionModel.Delete.Label = _proceduresActionModel.Delete.Tooltip = "Cancel";


			UpdateProcedureActionModel();

			_recipientsTable = new Table<ResultRecipientDetail>();
			_recipientsTable.Columns.Add(new TableColumn<ResultRecipientDetail, string>("Practitioner",
				item => PersonNameFormat.Format(item.Practitioner.Name)));
			_recipientsTable.Columns.Add(new TableColumn<ResultRecipientDetail, string>("Contact Point",
				item => item.ContactPoint.Name));

			_recipientsActionModel = new CrudActionModel(true, false, true);
			_recipientsActionModel.Add.SetClickHandler(AddRecipient);
			_recipientsActionModel.Add.Visible = false;    // hide this action on the menu/toolbar - we'll use a special button instead
			_recipientsActionModel.Delete.SetClickHandler(RemoveSelectedRecipient);
			UpdateRecipientsActionModel();

			this.Validation.Add(new ValidationRule("SelectedCancelReason",
				component => new ValidationResult(!(_mode == Mode.ReplaceOrder && _selectedCancelReason == null), SR.MessageCancellationReasonRequired)));
			this.Validation.Add(new ValidationRule("DowntimeAccessionNumber",
				component => new ValidationResult(
					!(this.IsDowntimeAccessionNumberVisible && string.IsNullOrEmpty(_downtimeAccessionNumber)),
					SR.MessageDowntimeAccessionNumberRequired)));

			// add validation rule to ensure the table has at least non-cancelled procedure
			this.Validation.Add(new ValidationRule("SelectedProcedure",
				component => new ValidationResult(CollectionUtils.Contains(_proceduresTable.Items, p => !p.Cancelled), SR.MessageNoActiveProcedures)));

			_noteSummaryComponent = new OrderNoteSummaryComponent(OrderNoteCategory.General);
			_noteSummaryComponent.ModifiedChanged += ((sender, args) => this.Modified = true);

			_attachmentSummaryComponent = new AttachedDocumentPreviewComponent(false, AttachedDocumentPreviewComponent.AttachmentMode.Order);
			this.ChangeCommitted += ((sender, args) => _attachmentSummaryComponent.SaveChanges());
			_orderAdditionalInfoComponent = new OrderAdditionalInfoComponent();
		}

		#endregion

		#region ApplicationComponent overrides

		public override void Start()
		{
			_bannerComponentHost = new ChildComponentHost(this.Host, new BannerComponent(new HealthcareContext(_patientRef, _profileRef, _orderRef)));
			_bannerComponentHost.StartComponent();

			_orderAdditionalInfoComponent.HealthcareContext = new HealthcareContext(_patientRef, _profileRef, _orderRef);

			_recipientLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);
			_diagnosticServiceLookupHandler = new DiagnosticServiceLookupHandler(this.Host.DesktopWindow);
			_orderingPractitionerLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);

			Platform.GetService<IOrderEntryService>(service =>
			{
				_activeVisits = service.ListVisitsForPatient(new ListVisitsForPatientRequest(_patientRef)).Visits;

				var formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());
				_facilityChoices = formChoicesResponse.FacilityChoices;
				_priorityChoices = formChoicesResponse.OrderPriorityChoices;
				_cancelReasonChoices = formChoicesResponse.CancelReasonChoices;
				_selectedCancelReason = _cancelReasonChoices.Count > 0 ? _cancelReasonChoices[0] : null;
				_lateralityChoices = formChoicesResponse.LateralityChoices;
			});

			if (_mode == Mode.NewOrder)
			{
				_selectedVisit = _activeVisits.Count > 0 ? _activeVisits[0] : null;
				_selectedPriority = _priorityChoices.Count > 0 ? _priorityChoices[0] : null;
				_orderingFacility = LoginSession.Current.WorkingFacility;
				_schedulingRequestTime = Platform.Time;
				_orderAdditionalInfoComponent.OrderExtendedProperties = _extendedProperties;
				_attachmentSummaryComponent.OrderAttachments = _newAttachments;
			}
			else
			{
				// Pre-populate the order entry page with details
				Platform.GetService<IOrderEntryService>(service =>
				{
					var response = service.GetOrderRequisitionForEdit(new GetOrderRequisitionForEditRequest(_orderRef));

					// update order ref so we have the latest version
					_orderRef = response.OrderRef;

					// update form
					UpdateFromRequisition(response.Requisition);
				});

				// bug #3506: in replace mode, overwrite the procedures with clean one(s) based on diagnostic service
				if (_mode == Mode.ReplaceOrder)
				{
					UpdateDiagnosticService(_selectedDiagnosticService);
				}
			}

			InitializeTabPages();

			base.Start();
		}

		public override void Stop()
		{
			if (_bannerComponentHost != null)
			{
				_bannerComponentHost.StopComponent();
				_bannerComponentHost = null;
			}

			if (_orderNoteSummaryComponentHost != null)
			{
				_orderNoteSummaryComponentHost.StopComponent();
				_orderNoteSummaryComponentHost = null;
			}

			if (_rightHandComponentContainerHost != null)
			{
				_rightHandComponentContainerHost.StopComponent();
				_rightHandComponentContainerHost = null;
			}

			base.Stop();
		}

		#endregion

		#region Presentation Model

		public int BannerHeight
		{
			get { return BannerSettings.Default.BannerHeight; }
		}

		public ApplicationComponentHost RightHandComponentContainerHost
		{
			get { return _rightHandComponentContainerHost; }
		}

		public ApplicationComponentHost OrderNoteSummaryHost
		{
			get { return _orderNoteSummaryComponentHost; }
		}

		public ApplicationComponentHost BannerComponentHost
		{
			get { return _bannerComponentHost; }
		}

		public EntityRef OrderRef
		{
			get { return _orderRef; }
		}

		public bool IsDiagnosticServiceEditable
		{
			get { return _mode != Mode.ModifyOrder; }
		}

		public bool IsCancelReasonVisible
		{
			get { return _mode == Mode.ReplaceOrder; }
		}

		public bool IsDowntimeAccessionNumberVisible
		{
			get { return DowntimeRecovery.InDowntimeRecoveryMode && _mode == Mode.NewOrder; }
		}

		public string AccessionNumberMask
		{
			get { return TextFieldMasks.AccessionNumberMask; }
		}

		public string DowntimeAccessionNumber
		{
			get { return _downtimeAccessionNumber; }
			set
			{
				_downtimeAccessionNumber = value;
				this.Modified = true;
			}
		}

		public IList ActiveVisits
		{
			get { return _activeVisits; }
		}

		public event EventHandler ActiveVisitsChanged
		{
			add { _activeVisitsChanged += value; }
			remove { _activeVisitsChanged -= value; }
		}


		[ValidateNotNull]
		public VisitSummary SelectedVisit
		{
			get { return _selectedVisit; }
			set
			{
				if (!Equals(value, _selectedVisit))
				{
					_selectedVisit = value;
					NotifyPropertyChanged("SelectedVisit");
					this.Modified = true;
				}
			}
		}

		public string FormatVisit(object visit)
		{
			var visitSummary = (VisitSummary)visit;
			var visitIdentity = new StringBuilder();
			if (visitSummary.Facility != null)
			{
				visitIdentity.Append(visitSummary.Facility.Code);
				visitIdentity.Append(" ");
			}
			visitIdentity.Append(visitSummary.VisitNumber.Id);

			if (visitSummary.CurrentLocation != null)
			{
				visitIdentity.Append(", ");
				visitIdentity.Append(visitSummary.CurrentLocation.Name);
				visitIdentity.Append(",");
			}

			var visitType = new StringBuilder();
			visitType.Append(visitSummary.PatientClass.Value);
			if (visitSummary.Status != null)
			{
				visitType.Append(" - ");
				visitType.Append(visitSummary.Status.Value);
			}

			return string.Format("{0} {1} {2}",
				visitIdentity,
				visitType,
				Format.DateTime(visitSummary.AdmitTime));
		}

		public void ShowVisitSummary()
		{
			try
			{
				var visitSummaryComponent = new VisitSummaryComponent(_patientRef, true);
				var exitCode = LaunchAsDialog(this.Host.DesktopWindow, visitSummaryComponent, SR.TitlePatientVisits);

				// remember the previous selection before updating the list
				var selectedVisitRef = _selectedVisit == null ? null : _selectedVisit.VisitRef;

				// if the user made a selection and accepted, then override the previous selection
				if (ApplicationComponentExitCode.Accepted == exitCode)
				{
					var selectedVisit = (VisitSummary)visitSummaryComponent.SummarySelection.Item;
					selectedVisitRef = selectedVisit == null ? null : selectedVisit.VisitRef;
				}

				// regardless of whether the user pressed OK or cancel, we should still update the list of active visits
				// because they could have added a new visit prior to cancelling out of the dialog
				Platform.GetService<IOrderEntryService>(service =>
				{
					var response = service.ListVisitsForPatient(new ListVisitsForPatientRequest(_patientRef));
					_activeVisits = response.Visits;
				});

				NotifyPropertyChanged("ActiveVisits");
				EventsHelper.Fire(_activeVisitsChanged, this, EventArgs.Empty);

				if (selectedVisitRef != null)
				{
					this.SelectedVisit = CollectionUtils.SelectFirst(_activeVisits, visit => visit.VisitRef.Equals(selectedVisitRef, true));
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public ILookupHandler DiagnosticServiceLookupHandler
		{
			get { return _diagnosticServiceLookupHandler; }
		}

		[ValidateNotNull]
		public DiagnosticServiceSummary SelectedDiagnosticService
		{
			get { return _selectedDiagnosticService; }
			set
			{
				if (value != this.SelectedDiagnosticService)
				{
					UpdateDiagnosticService(value);
					this.Modified = true;
				}
			}
		}

		public string FormatDiagnosticService(object item)
		{
			return ((DiagnosticServiceSummary)item).Name;
		}

		public ITable Procedures
		{
			get { return _proceduresTable; }
		}

		public ActionModelNode ProceduresActionModel
		{
			get { return _proceduresActionModel; }
		}

		public ISelection SelectedProcedure
		{
			get { return new Selection(_selectedProcedure); }
			set
			{
				if (value.Item != _selectedProcedure)
				{
					_selectedProcedure = (ProcedureRequisition)value.Item;
					UpdateProcedureActionModel();
				}
			}
		}

		public IList PriorityChoices
		{
			get { return _priorityChoices; }
		}

		[ValidateNotNull]
		public EnumValueInfo SelectedPriority
		{
			get { return _selectedPriority; }
			set
			{
				_selectedPriority = value;
				this.Modified = true;
			}
		}

		public IList CancelReasonChoices
		{
			get { return _cancelReasonChoices; }
		}

		public EnumValueInfo SelectedCancelReason
		{
			get { return _selectedCancelReason; }
			set
			{
				_selectedCancelReason = value;
				this.Modified = true;
			}
		}

		public IList FacilityChoices
		{
			get { return _facilityChoices; }
		}

		public string OrderingFacility
		{
			get { return _orderingFacility.Name; }
		}

		public string FormatFacility(object facility)
		{
			return ((FacilitySummary)facility).Name;
		}

		public ILookupHandler OrderingPractitionerLookupHandler
		{
			get { return _orderingPractitionerLookupHandler; }
		}

		[ValidateNotNull]
		public ExternalPractitionerSummary SelectedOrderingPractitioner
		{
			get { return _selectedOrderingPractitioner; }
			set
			{
				if (_selectedOrderingPractitioner != value)
				{
					_selectedOrderingPractitioner = value;
					NotifyPropertyChanged("SelectedOrderingPractitioner");

					_selectedOrderingPractitionerContactPoint = null;
					UpdateOrderingPractitionerContactPointChoices();
					NotifyPropertyChanged("OrderingPractitionerContactPointChoices");

					this.Modified = true;
				}
			}
		}

		public IList OrderingPractitionerContactPointChoices
		{
			get { return _orderingPractitionerContactPointChoices; }
		}

		[ValidateNotNull]
		public ExternalPractitionerContactPointDetail SelectedOrderingPractitionerContactPoint
		{
			get { return _selectedOrderingPractitionerContactPoint; }
			set
			{
				if (_selectedOrderingPractitionerContactPoint != value)
				{
					_selectedOrderingPractitionerContactPoint = value;
					NotifyPropertyChanged("SelectedOrderingPractitionerContactPoint");

					this.Modified = true;
				}
			}
		}

		public string FormatContactPoint(object cp)
		{
			var detail = (ExternalPractitionerContactPointDetail)cp;
			return ExternalPractitionerContactPointFormat.Format(detail);
		}

		public ITable Recipients
		{
			get { return _recipientsTable; }
		}

		public CrudActionModel RecipientsActionModel
		{
			get { return _recipientsActionModel; }
		}

		public ISelection SelectedRecipient
		{
			get { return new Selection(_selectedRecipient); }
			set
			{
				if (!Equals(value, _selectedRecipient))
				{
					_selectedRecipient = (ResultRecipientDetail)value.Item;
					UpdateRecipientsActionModel();
					NotifyPropertyChanged("SelectedRecipient");
				}
			}
		}

		public ILookupHandler RecipientsLookupHandler
		{
			get { return _recipientLookupHandler; }
		}

		public ExternalPractitionerSummary RecipientToAdd
		{
			get { return _recipientToAdd; }
			set
			{
				if (!Equals(value, _recipientToAdd))
				{
					_recipientToAdd = value;
					NotifyPropertyChanged("RecipientToAdd");

					_recipientContactPointToAdd = null;
					UpdateConsultantContactPointChoices();
					NotifyPropertyChanged("RecipientContactPointChoices");

					// must do this after contact point choices have been updated
					UpdateRecipientsActionModel();
				}
			}
		}

		public IList RecipientContactPointChoices
		{
			get { return _recipientContactPointChoices; }
		}

		public ExternalPractitionerContactPointDetail RecipientContactPointToAdd
		{
			get { return _recipientContactPointToAdd; }
			set
			{
				if (_recipientContactPointToAdd != value)
				{
					_recipientContactPointToAdd = value;
					NotifyPropertyChanged("RecipientContactPointToAdd");
				}
			}
		}

		[ValidateNotNull]
		public string Indication
		{
			get { return _indication; }
			set
			{
				_indication = value;
				this.Modified = true;
			}
		}

		public DateTime? SchedulingRequestTime
		{
			get { return _schedulingRequestTime; }
			set
			{
				_schedulingRequestTime = value;
				this.Modified = true;
			}
		}

		public void AddProcedure()
		{
			try
			{
				List<ProcedureTypeSummary> orderableProcedureTypes = null;
				Platform.GetService<IOrderEntryService>(service =>
					{
						var request = new ListOrderableProcedureTypesRequest(
							CollectionUtils.Map<ProcedureRequisition, EntityRef>(
								_proceduresTable.Items,
								req => req.ProcedureType.ProcedureTypeRef));

						orderableProcedureTypes = service.ListOrderableProcedureTypes(request).OrderableProcedureTypes;
					});

				var procedureRequisition = new ProcedureRequisition(null, _orderingFacility);
				var procedureEditor = new ProcedureEditorComponent(procedureRequisition, _facilityChoices, _lateralityChoices, orderableProcedureTypes);
				if (LaunchAsDialog(this.Host.DesktopWindow, procedureEditor, "Add Procedure")
					== ApplicationComponentExitCode.Accepted)
				{
					_proceduresTable.Items.Add(procedureRequisition);

					this.Modified = true;
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void EditSelectedProcedure()
		{
			if (_selectedProcedure == null || !_selectedProcedure.CanModify)
				return;

			try
			{
				var procedureEditor = new ProcedureEditorComponent(_selectedProcedure, _facilityChoices, _lateralityChoices);
				if (LaunchAsDialog(this.Host.DesktopWindow, procedureEditor, "Modify Procedure")
					== ApplicationComponentExitCode.Accepted)
				{
					_proceduresTable.Items.NotifyItemUpdated(_selectedProcedure);

					this.Modified = true;
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void CancelSelectedProcedure()
		{
			if (_selectedProcedure == null || !_selectedProcedure.CanModify)
				return;

			if (_selectedProcedure.Status == null)
			{
				// unsaved procedure
				_proceduresTable.Items.Remove(_selectedProcedure);
				_selectedProcedure = null;
				NotifyPropertyChanged("SelectedProcedure");
			}
			else
			{
				_selectedProcedure.Cancelled = true;
				_proceduresTable.Items.NotifyItemUpdated(_selectedProcedure);
			}

			this.Modified = true;
		}

		public void UpdateProcedureActionModel()
		{
			_proceduresActionModel.Add.Enabled = _selectedDiagnosticService != null;
			_proceduresActionModel.Edit.Enabled = (_selectedProcedure != null && _selectedProcedure.CanModify);
			_proceduresActionModel.Delete.Enabled = (_selectedProcedure != null && _selectedProcedure.CanModify && !_selectedProcedure.Cancelled);
		}

		public void AddRecipient()
		{
			if (_recipientToAdd != null && _recipientContactPointToAdd != null)
			{
				_recipientsTable.Items.Add(new ResultRecipientDetail(_recipientToAdd, _recipientContactPointToAdd, new EnumValueInfo("ANY", null, null)));
				this.Modified = true;
			}
		}

		public void RemoveSelectedRecipient()
		{
			_recipientsTable.Items.Remove(_selectedRecipient);
			_selectedRecipient = null;
			NotifyPropertyChanged("SelectedRecipient");
			this.Modified = true;
		}

		public void UpdateRecipientsActionModel()
		{
			_recipientsActionModel.Add.Enabled = (_recipientToAdd != null && _recipientContactPointToAdd != null);
			_recipientsActionModel.Delete.Enabled = (_selectedRecipient != null);
		}

		public event EventHandler ChangeCommitted
		{
			add { _changeCommitted += value; }
			remove { _changeCommitted -= value; }
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				//DEBUG: this.Host.ShowMessageBox(this.Validation.GetErrorsString(this), MessageBoxActions.Ok);
				this.ShowValidation(true);
				return;
			}

			if (SubmitOrder())
			{
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		public int ProcedureCount
		{
			get { return this._proceduresTable.Items.Count; }
		}

		#endregion

		private void UpdateDiagnosticService(DiagnosticServiceSummary summary)
		{
			_selectedDiagnosticService = summary;

			// update the table of procedures
			_proceduresTable.Items.Clear();
			if (_selectedDiagnosticService != null)
			{
				Platform.GetService<IOrderEntryService>(service =>
				{
					var response = service.LoadDiagnosticServiceBreakdown(new LoadDiagnosticServiceBreakdownRequest(summary.DiagnosticServiceRef));
					_proceduresTable.Items.AddRange(
						CollectionUtils.Map<ProcedureTypeSummary, ProcedureRequisition>(
							response.DiagnosticServiceDetail.ProcedureTypes,
							rpt => new ProcedureRequisition(rpt, _orderingFacility)));
				});
			}

			UpdateProcedureActionModel();

			NotifyPropertyChanged("SelectedDiagnosticService");
		}

		private OrderRequisition BuildOrderRequisition()
		{
			var requisition = new OrderRequisition
			{
				Patient = _selectedVisit.PatientRef,
				Visit = _selectedVisit,
				DiagnosticService = _selectedDiagnosticService,
				ReasonForStudy = _indication,
				Priority = _selectedPriority,
				OrderingFacility = _orderingFacility,
				SchedulingRequestTime = _schedulingRequestTime,
				OrderingPractitioner = _selectedOrderingPractitioner,
				Procedures = new List<ProcedureRequisition>(_proceduresTable.Items),
				Attachments = new List<OrderAttachmentSummary>(_attachmentSummaryComponent.OrderAttachments),
				Notes = new List<OrderNoteDetail>(_noteSummaryComponent.Notes),
				ExtendedProperties = _extendedProperties,
				ResultRecipients = new List<ResultRecipientDetail>(_recipientsTable.Items)
			};

			// only send the downtime number if a new downtime order is being entered
			if (this.IsDowntimeAccessionNumberVisible)
			{
				requisition.DowntimeAccessionNumber = _downtimeAccessionNumber;
				requisition.Notes.Insert(0, new OrderNoteDetail(OrderNoteCategory.General.Key, SR.MessageDowntimeOrderNote, null, false, null, null));
			}
			else
			{
				requisition.DowntimeAccessionNumber = null;
			}

			// there should always be a selected contact point, unless the ordering practitioner has 0 contact points
			if (_selectedOrderingPractitionerContactPoint != null)
			{
				// add the ordering practitioner as a result recipient
				requisition.ResultRecipients.Add(new ResultRecipientDetail(
					_selectedOrderingPractitioner,
					_selectedOrderingPractitionerContactPoint,
					new EnumValueInfo("ANY", null)));
			}

			return requisition;
		}

		private void UpdateFromRequisition(OrderRequisition existingOrder)
		{
			_patientRef = existingOrder.Patient;
			_selectedVisit = existingOrder.Visit;
			_selectedDiagnosticService = existingOrder.DiagnosticService;
			_indication = existingOrder.ReasonForStudy;
			_selectedPriority = existingOrder.Priority;
			_orderingFacility = existingOrder.OrderingFacility;
			_schedulingRequestTime = existingOrder.SchedulingRequestTime;
			_selectedOrderingPractitioner = existingOrder.OrderingPractitioner;

			_proceduresTable.Items.Clear();
			_proceduresTable.Items.AddRange(existingOrder.Procedures);

			var attachments = new List<OrderAttachmentSummary>(existingOrder.Attachments);
			attachments.AddRange(_newAttachments);
			_attachmentSummaryComponent.OrderAttachments = attachments;

			_noteSummaryComponent.Notes = existingOrder.Notes;
			_orderAdditionalInfoComponent.OrderExtendedProperties = _extendedProperties = existingOrder.ExtendedProperties;

			// initialize contact point choices for ordering practitioner
			UpdateOrderingPractitionerContactPointChoices();

			// what follows is some logic to try hide the ordering practitioner recipient from showing up in the
			// recipients table, since he already appears on the main part of the screen

			// select the recipient representing the ordering practitioner at the default contact point
			var orderingRecipient = CollectionUtils.SelectFirst(
				existingOrder.ResultRecipients,
				recipient => recipient.Practitioner.PractitionerRef == existingOrder.OrderingPractitioner.PractitionerRef 
					&& recipient.ContactPoint.IsDefaultContactPoint);

			// if not found, then select the first recipient representing the ordering practitioner
			if (orderingRecipient == null)
			{
				orderingRecipient = CollectionUtils.SelectFirst(
					existingOrder.ResultRecipients,
					recipient => recipient.Practitioner.PractitionerRef == existingOrder.OrderingPractitioner.PractitionerRef);
			}

			// if the recipient object exists for the ordering practitioner (and this *should* always be the case)
			if (orderingRecipient != null)
			{
				// initialize the ordering practitioner contact point
				_selectedOrderingPractitionerContactPoint = CollectionUtils.SelectFirst(
					_orderingPractitionerContactPointChoices,
					contactPoint => contactPoint.ContactPointRef == orderingRecipient.ContactPoint.ContactPointRef);

				// populate the recipients table, excuding the orderingRecipient 
				_recipientsTable.Items.Clear();
				_recipientsTable.Items.AddRange(CollectionUtils.Reject(existingOrder.ResultRecipients, r => r == orderingRecipient));
			}
			else
			{
				// just add all recipients to the table
				_recipientsTable.Items.Clear();
				_recipientsTable.Items.AddRange(existingOrder.ResultRecipients);
			}
		}

		private bool SubmitOrder()
		{
			// give additional info page a chance to save data
			_orderAdditionalInfoComponent.SaveData();

			// give extension pages a chance to save data prior to commit
			_extensionPages.ForEach(page => page.Save());

			var requisition = BuildOrderRequisition();

			try
			{
				switch (_mode)
				{
					case Mode.NewOrder:
						SubmitNewOrder(requisition);
						break;
					case Mode.ModifyOrder:
						SubmitModifyOrder(requisition);
						break;
					case Mode.ReplaceOrder:
						SubmitReplaceOrder(requisition);
						break;
				}

				EventsHelper.Fire(_changeCommitted, this, EventArgs.Empty);
				return true;
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, "", this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
				return false;
			}
		}

		private void SubmitNewOrder(OrderRequisition requisition)
		{
			Platform.GetService<IOrderEntryService>(service =>
			{
				var response = service.PlaceOrder(new PlaceOrderRequest(requisition));
				_orderRef = response.Order.OrderRef;

				this.Host.ShowMessageBox(
					string.Format(
						"Order {0} placed successfully.",
						AccessionFormat.Format(response.Order.AccessionNumber)),
					MessageBoxActions.Ok);
			});
		}

		private void SubmitModifyOrder(OrderRequisition requisition)
		{
			Platform.GetService<IOrderEntryService>(service =>
			{
				var response = service.ModifyOrder(new ModifyOrderRequest(_orderRef, requisition));
				_orderRef = response.Order.OrderRef;
			});
		}

		private void SubmitReplaceOrder(OrderRequisition requisition)
		{
			Platform.GetService<IOrderEntryService>(service =>
			{
				var request = new ReplaceOrderRequest(_orderRef, _selectedCancelReason, requisition, true);
				var response = service.ReplaceOrder(request);

				if (response.WarnUser &&
					DialogBoxAction.Yes == this.Host.DesktopWindow.ShowMessageBox(
											response.Warning + "\n\nAre you sure you want to cancel and replace this order?",
											MessageBoxActions.YesNo))
				{
					response = service.ReplaceOrder(new ReplaceOrderRequest(_orderRef, _selectedCancelReason, requisition, false));
				}
				else
					return;

				_orderRef = response.Order.OrderRef;

				this.Host.ShowMessageBox(
					string.Format("Order successfully replaced with new order {0}.", AccessionFormat.Format(response.Order.AccessionNumber)),
					MessageBoxActions.Ok);
			});
		}

		private void InitializeTabPages()
		{
			_orderNoteSummaryComponentHost = new ChildComponentHost(this.Host, _noteSummaryComponent);
			_orderNoteSummaryComponentHost.StartComponent();

			_rightHandComponentContainer = new TabComponentContainer();
			_rightHandComponentContainerHost = new ChildComponentHost(this.Host, _rightHandComponentContainer);


			var attachmentsTabPage = new TabPage("Documents", _attachmentSummaryComponent);
			_rightHandComponentContainer.Pages.Add(attachmentsTabPage);
			_rightHandComponentContainer.Pages.Add(new TabPage("Additional Info", _orderAdditionalInfoComponent));

			// instantiate all extension pages
			_extensionPages = new List<IOrderEditorPage>();
			foreach (IOrderEditorPageProvider pageProvider in new OrderEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new OrderEditorContext(this)));
			}

			// add extension pages to navigator
			// the navigator will start those components if the user goes to that page
			foreach (var page in _extensionPages)
			{
				_rightHandComponentContainer.Pages.Add(new TabPage(page.Path.LocalizedPath, page.GetComponent()));
			}

			_rightHandComponentContainerHost.StartComponent();

			if (_newAttachments.Count > 0)
			{
				_rightHandComponentContainer.CurrentPage = attachmentsTabPage;
				_attachmentSummaryComponent.SetInitialSelection(_newAttachments[0]);
			}
		}

		private void UpdateOrderingPractitionerContactPointChoices()
		{
			_orderingPractitionerContactPointChoices = GetPractitionerContactPoints(_selectedOrderingPractitioner);
		}

		private void UpdateConsultantContactPointChoices()
		{
			_recipientContactPointChoices = GetPractitionerContactPoints(_recipientToAdd);
		}

		private List<ExternalPractitionerContactPointDetail> GetPractitionerContactPoints(ExternalPractitionerSummary prac)
		{
			var choices = new List<ExternalPractitionerContactPointDetail>();
			if (prac != null)
			{
				Platform.GetService<IOrderEntryService>(service =>
				{
					var response = service.GetExternalPractitionerContactPoints(new GetExternalPractitionerContactPointsRequest(prac.PractitionerRef));
					choices = response.ContactPoints;
				});
			}
			return choices;
		}

	}
}
