#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.MimeDocumentService;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="OrderEntryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class OrderEntryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class OrderEntryAttachmentToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// OrderEntryComponent class
    /// </summary>
    [AssociateView(typeof(OrderEntryComponentViewExtensionPoint))]
    public class OrderEntryComponent : ApplicationComponent
    {
        public enum Mode
        {
            NewOrder,
            ModifyOrder,
            ReplaceOrder
        }

        class OrderEntryAttachmentToolContext : ToolContext, IMimeDocumentToolContext
        {
            private readonly OrderEntryComponent _component;

            internal OrderEntryAttachmentToolContext(OrderEntryComponent component)
            {
                _component = component;
            }

            #region IMimeDocumentToolContext Members

            public event EventHandler SelectedDocumentChanged
            {
                add { _component.SelectedAttachmentChanged += value; }
                remove { _component.SelectedAttachmentChanged -= value; }
            }

            public EntityRef SelectedDocumentRef
            {
                get { return ((OrderAttachmentSummary) _component.SelectedAttachment.Item).Document.DocumentRef; }
            }

            public void RemoveSelectedDocument()
            {
                _component.RemoveSelectedAttachment();
            }

            public event EventHandler ChangeCommitted
            {
                add { _component.ChangeCommitted += value; }
                remove { _component.ChangeCommitted -= value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            #endregion
        }

        private readonly Mode _mode;
        private EntityRef _patientRef;
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

        private EnumValueInfo _selectedPriority;
        private EnumValueInfo _selectedCancelReason;

        private DiagnosticServiceSummary _selectedDiagnosticService;

        private DateTime? _schedulingRequestTime;

        private readonly Table<ProcedureRequisition> _proceduresTable;
        private readonly CrudActionModel _proceduresActionModel;
        private ProcedureRequisition _selectedProcedure;

        private readonly Table<ExternalPractitionerSummary> _consultantsTable;
        private readonly CrudActionModel _consultantsActionModel;
        private ExternalPractitionerSummary _selectedConsultant;
        private ExternalPractitionerLookupHandler _consultantLookupHandler;
        private ExternalPractitionerSummary _consultantToAdd;

        private string _indication;
        private List<EnumValueInfo> _lateralityChoices;

        private readonly OrderAttachmentTable _attachmentTable = new OrderAttachmentTable();
        private OrderAttachmentSummary _selectedAttachment;
        private event EventHandler _selectedAttachmentChanged;
        private string _tempAttachmentFileName;

        private event EventHandler _changeCommitted;

        private ToolSet _attachmentToolSet;

        private OrderNoteSummaryComponent _noteSummaryComponent;
        private ChildComponentHost _orderNoteSummaryComponentHost;

        /// <summary>
        /// Constructor for creating a new order.
        /// </summary>
        public OrderEntryComponent(EntityRef patientRef)
            : this(patientRef, null, Mode.NewOrder)
        {
        }

        /// <summary>
        /// Constructor for creating a new order with attachments.
        /// </summary>
        public OrderEntryComponent(EntityRef patientRef, IEnumerable<OrderAttachmentSummary> attachments)
            : this(patientRef, null, Mode.NewOrder)
        {
            _attachmentTable.Items.AddRange(attachments);
        }

        /// <summary>
        /// Constructor for modifying or replacing an order.
        /// </summary>
        /// <param name="patientRef"></param>
        /// <param name="orderRef"></param>
        /// <param name="mode"></param>
        public OrderEntryComponent(EntityRef patientRef, EntityRef orderRef, Mode mode)
        {
            Platform.CheckForNullReference(patientRef, "patientRef");

            _mode = mode;
            if(mode == Mode.ModifyOrder || mode == Mode.ReplaceOrder)
                Platform.CheckForNullReference(orderRef, "orderRef");

            _patientRef = patientRef;
            _orderRef = orderRef;

            _proceduresTable = new Table<ProcedureRequisition>();
            _proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, string>("Name",
                                      delegate(ProcedureRequisition item) { return item.ProcedureType.Name; }));
            _proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, string>("Facility",
                                      delegate(ProcedureRequisition item)
                                          {
                                               return item.PerformingFacility == null ? "" : item.PerformingFacility.Code;
                                          }));
            _proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, string>("Lat.",
                                      delegate(ProcedureRequisition item)
                                          {
                                                  return (item.Laterality == null || item.Laterality.Code == "N")
                                                      ? "" : item.Laterality.Code;
                                          }));
            _proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, bool>("Port.",
                                      delegate(ProcedureRequisition item) { return item.PortableModality; }));
            _proceduresTable.Columns.Add(new TableColumn<ProcedureRequisition, string>("Scheduled Time",
                                      delegate(ProcedureRequisition item)
                                          {
                                              // if new or scheduled
                                              if (item.Status == null || item.Status.Code == "SC")
                                                  return Format.DateTime(item.ScheduledTime);
                                              else
                                                  return item.Status.Value;
                                          }));

            _proceduresActionModel = new CrudActionModel();
            _proceduresActionModel.Add.SetClickHandler(AddProcedure);
            _proceduresActionModel.Edit.SetClickHandler(EditSelectedProcedure);
            _proceduresActionModel.Delete.SetClickHandler(RemoveSelectedProcedure);
            UpdateProcedureActionModel();

            _consultantsTable = new Table<ExternalPractitionerSummary>();
            _consultantsTable.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>("Name",
                                      delegate(ExternalPractitionerSummary item) { return PersonNameFormat.Format(item.Name); }));

            _consultantsActionModel = new CrudActionModel(true, false, true);
            _consultantsActionModel.Add.SetClickHandler(AddConsultant);
            _consultantsActionModel.Add.Visible = false;    // hide this action on the menu/toolbar - we'll use a special button instead
            _consultantsActionModel.Delete.SetClickHandler(RemoveSelectedConsultant);
            UpdateConsultantActionModel();

            this.Validation.Add(OrderEntryComponentSettings.Default.ValidationRules);
            this.Validation.Add(new ValidationRule("SelectedCancelReason",
                delegate
                {
                    // if replacing the order, ensure cancel reason selected
                    return new ValidationResult(_mode != Mode.ReplaceOrder || _selectedCancelReason != null,
                        SR.MessageMissingCancellationReason);
                }));

            _noteSummaryComponent = new OrderNoteSummaryComponent();
        }

        public override void Start()
        {
            _consultantLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);
            _diagnosticServiceLookupHandler = new DiagnosticServiceLookupHandler(this.Host.DesktopWindow);
            _orderingPractitionerLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);

            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    ListActiveVisitsForPatientResponse response = service.ListActiveVisitsForPatient(new ListActiveVisitsForPatientRequest(_patientRef));
                    _activeVisits = response.Visits;
 
                    GetOrderEntryFormDataResponse formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());
                    _facilityChoices = formChoicesResponse.FacilityChoices;
                    _priorityChoices = formChoicesResponse.OrderPriorityChoices;
                    _cancelReasonChoices = formChoicesResponse.CancelReasonChoices;
                    _lateralityChoices = formChoicesResponse.LateralityChoices;

                });

            if (_mode == Mode.NewOrder)
            {
                _selectedVisit = _activeVisits.Count > 0 ? _activeVisits[0] : null;
                _selectedPriority = _priorityChoices.Count > 0 ? _priorityChoices[0] : null;
                _orderingFacility = LoginSession.Current.WorkingFacility;
                _schedulingRequestTime = Platform.Time;
            }
            else
            {
                // Pre-populate the order entry page with details
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        GetOrderRequisitionForEditResponse response = service.GetOrderRequisitionForEdit(new GetOrderRequisitionForEditRequest(_orderRef));
                        
                        // update order ref so we have the latest version
                        _orderRef = response.OrderRef;

                        // update form
                        UpdateFromRequisition(response.Requisition);
                    });
            }

            _attachmentToolSet = new ToolSet(new OrderEntryAttachmentToolExtensionPoint(), new OrderEntryAttachmentToolContext(this));

            InitializeTabPages();

            base.Start();
        }

        public override void Stop()
        {
            _attachmentToolSet.Dispose();
            base.Stop();
        }

        #region Presentation Model

        public ApplicationComponentHost OrderNoteSummaryHost
        {
            get { return _orderNoteSummaryComponentHost; }
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
                if(!object.Equals(value, _selectedVisit))
                {
                    _selectedVisit = value;
                    NotifyPropertyChanged("SelectedVisit");
                }
            }
        }

        public string FormatVisit(object visit)
        {
            // TODO this is kind of temporary - not sure what information should be presented
            // in the dropdown
            VisitSummary v = (VisitSummary)visit;

            StringBuilder visitType = new StringBuilder();
            visitType.Append(v.PatientClass);
            if (!string.IsNullOrEmpty(v.PatientType))
            {
                visitType.Append(" - ");
                visitType.Append(v.PatientType);
            }
            if (!string.IsNullOrEmpty(v.AdmissionType))
            {
                visitType.Append(" - ");
                visitType.Append(v.AdmissionType);
            }

            return string.Format("{0} {1} {2}",
                VisitNumberFormat.Format(v.VisitNumber),
                visitType,
                Format.DateTime(v.AdmitDateTime)
                );
        }

        public void ShowVisitSummary()
        {
            VisitSummaryComponent component = new VisitSummaryComponent(_patientRef);
            if (ApplicationComponentExitCode.Accepted == 
                LaunchAsDialog(
                    this.Host.DesktopWindow,
                    component,
                    SR.TitlePatientVisits))
            {
                EntityRef existingSelectedVisitRef = _selectedVisit == null ? null : _selectedVisit.VisitRef;

                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                        {
                            ListActiveVisitsForPatientResponse response =
                                service.ListActiveVisitsForPatient(new ListActiveVisitsForPatientRequest(_patientRef));
                            _activeVisits = response.Visits;
                        });

                EventsHelper.Fire(_activeVisitsChanged, this, EventArgs.Empty);

                if (existingSelectedVisitRef != null)
                {
                    this.SelectedVisit = CollectionUtils.SelectFirst(_activeVisits,
                        delegate(VisitSummary visit)
                        {
                            return Equals(visit.VisitRef, existingSelectedVisitRef);
                        });
                }
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
                if(value != this.SelectedDiagnosticService)
                {
                    UpdateDiagnosticService(value);
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
                if(value.Item != _selectedProcedure)
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
            set { _selectedPriority = value; }
        }

        public IList CancelReasonChoices
        {
            get { return _cancelReasonChoices; }
        }

        public EnumValueInfo SelectedCancelReason
        {
            get { return _selectedCancelReason; }
            set { _selectedCancelReason = value; }
        }

        public IList FacilityChoices
        {
            get { return _facilityChoices;  }
        }

        public string OrderingFacility
        {
            get { return _orderingFacility.Name; }
        }

        public string FormatFacility(object facility)
        {
            return (facility as FacilitySummary).Name;
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
                if(_selectedOrderingPractitioner != value)
                {
                    _selectedOrderingPractitioner = value;
                    NotifyPropertyChanged("SelectedOrderingPractitioner");
                }
            }
        }

        public ITable Consultants
        {
            get { return _consultantsTable; }
        }

        public CrudActionModel ConsultantsActionModel
        {
            get { return _consultantsActionModel; }
        }

        public ISelection SelectedConsultant
        {
            get { return new Selection(_selectedConsultant); }
            set
            {
                if(!object.Equals(value, _selectedConsultant))
                {
                    _selectedConsultant = (ExternalPractitionerSummary)value.Item;
                    UpdateConsultantActionModel();
                    NotifyPropertyChanged("SelectedConsultant");
                }
            }
        }

        public ILookupHandler ConsultantsLookupHandler
        {
            get { return _consultantLookupHandler; }
        }

        public ExternalPractitionerSummary ConsultantToAdd
        {
            get { return _consultantToAdd; }
            set
            {
                if (!object.Equals(value, _consultantToAdd))
                {
                    _consultantToAdd = value;
                    UpdateConsultantActionModel();
                    NotifyPropertyChanged("ConsultantToAdd");
                }
            }
        }

        [ValidateNotNull]
        public string Indication
        {
            get { return _indication; }
            set { _indication = value; }
        }

        public DateTime? SchedulingRequestTime
        {
            get { return _schedulingRequestTime; }
            set { _schedulingRequestTime = value; }
        }

        public void ApplySchedulingToProcedures()
        {
            foreach (ProcedureRequisition item in _proceduresTable.Items)
            {
                if (item.CanModify)
                {
                    item.ScheduledTime = _schedulingRequestTime;
                    _proceduresTable.Items.NotifyItemUpdated(item);
                }
            }
        }

        public void AddProcedure()
        {
            try
            {
                List<RequestedProcedureTypeSummary> orderableProcedureTypes = null;
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        ListOrderableProcedureTypesRequest request = new ListOrderableProcedureTypesRequest(
                            CollectionUtils.Map<ProcedureRequisition, EntityRef>(_proceduresTable.Items,
                                delegate(ProcedureRequisition req) { return req.ProcedureType.EntityRef; }));

                        orderableProcedureTypes = service.ListOrderableProcedureTypes(request).OrderableProcedureTypes;

                    });

                ProcedureRequisition procReq = new ProcedureRequisition(null, _orderingFacility);
                RequestedProcedureEditorComponent procedureEditor = new RequestedProcedureEditorComponent(procReq, _facilityChoices, _lateralityChoices, orderableProcedureTypes);
                if(ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, procedureEditor, "Add Procedure")
                    == ApplicationComponentExitCode.Accepted)
                {
                    _proceduresTable.Items.Add(procReq);
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void EditSelectedProcedure()
        {
            if(_selectedProcedure == null || !_selectedProcedure.CanModify)
                return;

            try
            {
                RequestedProcedureEditorComponent procedureEditor = new RequestedProcedureEditorComponent(_selectedProcedure, _facilityChoices, _lateralityChoices);
                if (ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, procedureEditor, "Modify Procedure")
                    == ApplicationComponentExitCode.Accepted)
                {
                    _proceduresTable.Items.NotifyItemUpdated(_selectedProcedure);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void RemoveSelectedProcedure()
        {
            if (_selectedProcedure == null || !_selectedProcedure.CanModify)
                return;

            _proceduresTable.Items.Remove(_selectedProcedure);
            _selectedProcedure = null;
            NotifyPropertyChanged("SelectedProcedure");
        }

        public void UpdateProcedureActionModel()
        {
            _proceduresActionModel.Add.Enabled = true;
            _proceduresActionModel.Edit.Enabled = (_selectedProcedure != null && _selectedProcedure.CanModify);
            _proceduresActionModel.Delete.Enabled = (_selectedProcedure != null && _selectedProcedure.CanModify);
        }

        public void AddConsultant()
        {
            if(_consultantToAdd != null)
            {
                _consultantsTable.Items.Add(_consultantToAdd);
            }
        }

        public void RemoveSelectedConsultant()
        {
            _consultantsTable.Items.Remove(_selectedConsultant);
            _selectedConsultant = null;
            NotifyPropertyChanged("SelectedConsultant");
        }

        public void UpdateConsultantActionModel()
        {
            _consultantsActionModel.Add.Enabled = (_consultantToAdd != null);
            _consultantsActionModel.Delete.Enabled = (_selectedConsultant != null);
        }
        
        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                //DEBUG: this.Host.ShowMessageBox(this.Validation.GetErrorsString(this), MessageBoxActions.Ok);
                this.ShowValidation(true);
                return;
            }

            if(SubmitOrder())
            {
                this.Exit(ApplicationComponentExitCode.Accepted);
            }
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        private void UpdateDiagnosticService(DiagnosticServiceSummary summary)
        {
            _selectedDiagnosticService = summary;

            // update the table of procedures
            //TODO: should warn user if there are already procedures in this table???
            _proceduresTable.Items.Clear();
            if (_selectedDiagnosticService != null)
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        LoadDiagnosticServiceBreakdownResponse response = service.LoadDiagnosticServiceBreakdown(
                            new LoadDiagnosticServiceBreakdownRequest(summary.DiagnosticServiceRef));
                        _proceduresTable.Items.AddRange(
                           CollectionUtils.Map<RequestedProcedureTypeDetail, ProcedureRequisition>(
                               response.DiagnosticServiceDetail.RequestedProcedureTypes,
                               delegate(RequestedProcedureTypeDetail rpt)
                               {
                                   return new ProcedureRequisition(rpt.GetSummary(), _orderingFacility);
                               }));
                    });
            }

            NotifyPropertyChanged("SelectedDiagnosticService");
        }

        private OrderRequisition BuildOrderRequisition()
        {
            OrderRequisition requisition = new OrderRequisition();
            requisition.Patient = _selectedVisit.PatientRef;
            requisition.Visit = _selectedVisit;
            requisition.DiagnosticService = _selectedDiagnosticService;
            requisition.ReasonForStudy = _indication;
            requisition.Priority = _selectedPriority;
            requisition.OrderingFacility = _orderingFacility;
            requisition.SchedulingRequestTime = _schedulingRequestTime;
            requisition.OrderingPractitioner = _selectedOrderingPractitioner;
            requisition.RequestedProcedures = new List<ProcedureRequisition>(_proceduresTable.Items);
            requisition.CopiesToPractitioners = new List<ExternalPractitionerSummary>(_consultantsTable.Items);
            requisition.Attachments = new List<OrderAttachmentSummary>(_attachmentTable.Items);
            requisition.Notes = new List<OrderNoteDetail>(_noteSummaryComponent.Notes);
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
            _proceduresTable.Items.AddRange(existingOrder.RequestedProcedures);

            _consultantsTable.Items.Clear();
            _consultantsTable.Items.AddRange(existingOrder.CopiesToPractitioners);

            _attachmentTable.Items.Clear();
            _attachmentTable.Items.AddRange(existingOrder.Attachments);

            _noteSummaryComponent.Notes = existingOrder.Notes;
        }
        
        private bool SubmitOrder()
        {
            OrderRequisition requisition = BuildOrderRequisition();

            try
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        if (_mode == Mode.NewOrder)
                        {
                            PlaceOrderResponse response = service.PlaceOrder(new PlaceOrderRequest(requisition));
                            _orderRef = response.OrderRef;
                        }
                        else if (_mode == Mode.ModifyOrder)
                        {
                            ModifyOrderResponse response = service.ModifyOrder(new ModifyOrderRequest(_orderRef, requisition));
                            _orderRef = response.OrderRef;
                        }
                        else if (_mode == Mode.ReplaceOrder)
                        {
                            ReplaceOrderRequest request = new ReplaceOrderRequest(_orderRef, _selectedCancelReason, requisition);
                            ReplaceOrderResponse response = service.ReplaceOrder(request);
                            _orderRef = response.OrderRef;
                        }
                    });

                EventsHelper.Fire(_changeCommitted, this, EventArgs.Empty);

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, "", this.Host.DesktopWindow, 
                    delegate
                        {
                            this.Exit(ApplicationComponentExitCode.Error);
                        });
                return false;
            }
        }

        #region Attachment Methods

        public ITable Attachments
        {
            get { return _attachmentTable; }
        }

        public ActionModelRoot AttachmentActionModel
        {
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "orderentry-items-tools", _attachmentToolSet.Actions); }
        }

        public override IActionSet ExportedActions
        {
            get { return _attachmentToolSet.Actions; }
        }

        public ISelection SelectedAttachment
        {
            get { return new Selection(_selectedAttachment); }
            set
            {
                if (value.Item != _selectedAttachment)
                {
                    _selectedAttachment = (OrderAttachmentSummary)value.Item;

                    if (_selectedAttachment == null)
                        ClearPreviewData();
                    else 
                        SetPreviewData(
                            _selectedAttachment.Document.MimeType,
                            _selectedAttachment.Document.FileExtension,
                            _selectedAttachment.Document.BinaryDataRef);                    
                }
            }
        }

        public event EventHandler SelectedAttachmentChanged
        {
            add { _selectedAttachmentChanged += value; }
            remove { _selectedAttachmentChanged -= value; }
        }

        public event EventHandler ChangeCommitted
        {
            add { _changeCommitted += value; }
            remove { _changeCommitted -= value; }
        }

        public void RemoveSelectedAttachment()
        {
            if (_selectedAttachment == null)
                return;

            _attachmentTable.Items.Remove(_selectedAttachment);
        }

        public string TempFileName
        {
            get { return _tempAttachmentFileName; }
        }

        private void ClearPreviewData()
        {
            SetPreviewData(null, null, null);    
        }

        private void SetPreviewData(string mimeType, string fileExtension, EntityRef dataRef)
        {
            if (dataRef == null)
            {
                _tempAttachmentFileName = null;                    
            }
            else
            {
                string tempFile = TempFileManager.Instance.GetTempFile(dataRef);
                if (String.IsNullOrEmpty(tempFile))
                {
                    try
                    {
                        Byte[] data = RetrieveAttachmentData(dataRef);
                        _tempAttachmentFileName = TempFileManager.Instance.CreateTemporaryFile(dataRef, fileExtension, data);
                    }
                    catch (Exception e)
                    {
                        _tempAttachmentFileName = null;
                        ExceptionHandler.Report(e, SR.ExceptionFailedToDisplayDocument, this.Host.DesktopWindow);
                    }
                }
                else
                {
                    if (Equals(_tempAttachmentFileName, tempFile))
                        return;  // nothing has changed

                    _tempAttachmentFileName = tempFile;
                }
            }

            EventsHelper.Fire(_selectedAttachmentChanged, this, EventArgs.Empty);
        }

        private static byte[] RetrieveAttachmentData(EntityRef dataRef)
        {
            byte[] data = null;

            Platform.GetService<IMimeDocumentService>(
                delegate(IMimeDocumentService service)
                    {
                        GetDocumentDataResponse response = service.GetDocumentData(new GetDocumentDataRequest(dataRef));
                        data = response.BinaryData;
                    });

            return data;
        }

        #endregion

        private void InitializeTabPages()
        {
            _orderNoteSummaryComponentHost = new ChildComponentHost(this.Host, _noteSummaryComponent);
            _orderNoteSummaryComponentHost.StartComponent();
        }
    
    }
}
