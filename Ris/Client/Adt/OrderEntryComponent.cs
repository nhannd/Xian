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
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("neworder", "folderexplorer-items-contextmenu/New Order")]
    [ButtonAction("neworder", "folderexplorer-items-toolbar/New Order")]
    [MenuAction("neworder", "RegistrationPreview-menu/NewOrders")]
    [MenuAction("neworder", "global-menus/Orders/New")]
    [ButtonAction("neworder", "patientsearch-items-toolbar/New Order")]
    [MenuAction("neworder", "patientsearch-items-contextmenu/New Order")]
    [IconSet("neworder", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
	[EnabledStateObserver("neworder", "Enabled", "EnabledChanged")]
    [ClickHandler("neworder", "NewOrder")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationPreviewToolExtensionPoint))]
    [ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
    public class OrderEntryTool : Tool<IToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            _enabled = false;   // disable by default

            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItemsChanged += delegate
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                    && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else if (this.ContextBase is IPatientSearchToolContext)
            {
                ((IPatientSearchToolContext)this.ContextBase).SelectedProfileChanged += delegate
                {
                    IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                    this.Enabled = (context.SelectedProfile != null && context.SelectedProfile.ProfileRef != null);
                };
            }
            else if (this.ContextBase is IPatientBiographyToolContext)
            {
                this.Enabled = true;
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        public void NewOrder()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                string title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(item.Name), MrnFormat.Format(item.Mrn));
                NewOrder(item.PatientProfileRef, title, context.DesktopWindow);
            }
            else if (this.ContextBase is IPatientSearchToolContext)
            {
                IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                string title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(context.SelectedProfile.Name), MrnFormat.Format(context.SelectedProfile.Mrn));
                NewOrder(context.SelectedProfile.ProfileRef, title, context.DesktopWindow);
            }
            else if (this.ContextBase is IPatientBiographyToolContext)
            {
                IPatientBiographyToolContext context = (IPatientBiographyToolContext)this.ContextBase;
                NewOrder(context.PatientProfile, "New Order", context.DesktopWindow);
            }
        }

        private void NewOrder(EntityRef profileRef, string title, IDesktopWindow desktopWindow)
        {
            try
            {
                ApplicationComponent.LaunchAsWorkspace(
                    desktopWindow,
                    new OrderEntryComponent(profileRef),
                    title,
                    delegate(IApplicationComponent c)
                    {
                        if (c.ExitCode == ApplicationComponentExitCode.Normal)
                        {
                            OrderEntryComponent component = (OrderEntryComponent) c;

                            Platform.GetService<IOrderEntryService>(
                                delegate(IOrderEntryService service)
                                {
                                    service.PlaceOrder(component.PlaceOrderRequest);
                                });

                            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
                            {
                                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;

                                // Refresh the schedule folder is a new folder is placed
                                IFolder scheduledFolder = CollectionUtils.SelectFirst<IFolder>(context.Folders,
                                    delegate(IFolder f) { return f is Folders.ScheduledFolder; });

                                if (scheduledFolder.IsOpen)
                                    scheduledFolder.Refresh();
                                else
                                    scheduledFolder.RefreshCount();
                            }
                        }
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionCannotPlaceOrder, desktopWindow);
            }
        }
    }


    /// <summary>
    /// Extension point for views onto <see cref="OrderEntryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class OrderEntryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// OrderEntryComponent class
    /// </summary>
    [AssociateView(typeof(OrderEntryComponentViewExtensionPoint))]
    public class OrderEntryComponent : ApplicationComponent
    {
        private readonly EntityRef _patientProfileRef;
        private readonly OrderDetail _reOrderDetail;

        private VisitSummaryTable _visitTable;
        private List<DiagnosticServiceSummary> _diagnosticServiceChoices;
        private List<FacilitySummary> _facilityChoices;
        private List<ExternalPractitionerSummary> _orderingPhysicianChoices;
        private List<EnumValueInfo> _priorityChoices;
        private List<EnumValueInfo> _cancelReasonChoices;

        private VisitSummary _selectedVisit;
        //private DiagnosticServiceSummary _selectedDiagnosticService;
        private FacilitySummary _selectedFacility;
        private ExternalPractitionerSummary _selectedOrderingPhysician;
        private EnumValueInfo _selectedPriority;
        private EnumValueInfo _selectedCancelReason;

        private event EventHandler _diagnosticServiceChanged;
        private Tree<RequestedProcedureTypeDetail> _diagnosticServiceBreakdown;
        private object _selectedDiagnosticServiceBreakdownItem;

        private Tree<DiagnosticServiceTreeItem> _diagnosticServiceTree;
        private DiagnosticServiceTreeItem _selectedDiagnosticServiceTreeItem;

        private bool _scheduleOrder;
        private DateTime _schedulingRequestDateTime;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderEntryComponent(EntityRef patientProfileRef)
        {
            this.Validation.Add(OrderEntryComponentSettings.Default.ValidationRules);

            _patientProfileRef = patientProfileRef;
        }

        public OrderEntryComponent(OrderDetail orderDetail)
        {
            this.Validation.Add(OrderEntryComponentSettings.Default.ValidationRules);

            _reOrderDetail = orderDetail;
        }

        public override void Start()
        {
            _visitTable = new VisitSummaryTable();

            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    ListActiveVisitsForPatientRequest request = new ListActiveVisitsForPatientRequest();
                    if (_patientProfileRef != null)
                        request.PatientProfileRef = _patientProfileRef;
                    else if (_reOrderDetail != null)
                        request.PatientRef = _reOrderDetail.PatientRef;

                    ListActiveVisitsForPatientResponse response = service.ListActiveVisitsForPatient(request);
                    _visitTable.Items.AddRange(response.Visits);

                    GetOrderEntryFormDataResponse formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());
                    _diagnosticServiceChoices = formChoicesResponse.DiagnosticServiceChoices;
                    _facilityChoices = formChoicesResponse.OrderingFacilityChoices;
                    _orderingPhysicianChoices = formChoicesResponse.OrderingPhysicianChoices;
                    _priorityChoices = formChoicesResponse.OrderPriorityChoices;
                    _cancelReasonChoices = formChoicesResponse.CancelReasonChoices;

                    TreeItemBinding<DiagnosticServiceTreeItem> binding = new TreeItemBinding<DiagnosticServiceTreeItem>(
                            delegate(DiagnosticServiceTreeItem ds) { return ds.Description; },
                            ExpandDiagnosticServiceTree);
                    binding.CanHaveSubTreeHandler = delegate(DiagnosticServiceTreeItem ds) { return ds.DiagnosticService == null; };
                    _diagnosticServiceTree = new Tree<DiagnosticServiceTreeItem>(binding, formChoicesResponse.TopLevelDiagnosticServiceTree);

                    if (_reOrderDetail == null)
                    {
                        _selectedPriority = _priorityChoices[0];
                    }
                    else
                    {
                        // Pre-populate the order entry page with details

                        _selectedVisit = CollectionUtils.SelectFirst<VisitSummary>(response.Visits,
                            delegate(VisitSummary summary)
                                {
                                    return Equals(summary.VisitNumberId, _reOrderDetail.Visit.VisitNumberId)
                                        && Equals(summary.VisitNumberAssigningAuthority, _reOrderDetail.Visit.VisitNumberAssigningAuthority);
                                });

                        _selectedFacility = CollectionUtils.SelectFirst<FacilitySummary>(_facilityChoices,
                            delegate(FacilitySummary summary)
                                {
                                    return summary.Code == _reOrderDetail.OrderingFacility.Code;
                                });

                        _selectedOrderingPhysician = CollectionUtils.SelectFirst<ExternalPractitionerSummary>(_orderingPhysicianChoices,
                            delegate(ExternalPractitionerSummary summary)
                                {
                                    return Equals(summary.LicenseNumber.Id, _reOrderDetail.OrderingPractitioner.LicenseNumber.Id)
                                     && Equals(summary.LicenseNumber.AssigningAuthority, _reOrderDetail.OrderingPractitioner.LicenseNumber.AssigningAuthority);
                                });

                        _selectedPriority = CollectionUtils.SelectFirst<EnumValueInfo>(_priorityChoices,
                            delegate(EnumValueInfo summary)
                                {
                                    return Equals(summary.Code, _reOrderDetail.OrderPriority.Code);
                                });

                        _selectedPriority = CollectionUtils.SelectFirst<EnumValueInfo>(_priorityChoices,
                            delegate(EnumValueInfo summary)
                            {
                                return Equals(summary.Code, _reOrderDetail.OrderPriority.Code);
                            });
                    }

                });

            _schedulingRequestDateTime = Platform.Time;
            _scheduleOrder = true;

            base.Start();
        }

        #region Presentation Model

        public bool IsReOrdering
        {
            get { return _reOrderDetail != null; }    
        }

        public ITable VisitTable
        {
            get { return _visitTable; }
        }

        public ISelection SelectedVisit
        {
            get { return _selectedVisit == null ? Selection.Empty : new Selection(_selectedVisit); }
            set { _selectedVisit = (VisitSummary)value.Item; }
        }

        public List<string> DiagnosticServiceChoices
        {
            get
            {
                List<string> dsStrings = new List<string>();
                dsStrings.Add("");
                dsStrings.AddRange(
                    CollectionUtils.Map<DiagnosticServiceSummary, string>(
                        _diagnosticServiceChoices, delegate(DiagnosticServiceSummary ds) { return ds.Name; }));

                return dsStrings;
            }
        }

        public object SelectedDiagnosticService
        {
            get
            {
                return _selectedDiagnosticServiceTreeItem == null
                    ? null
                    : _selectedDiagnosticServiceTreeItem.DiagnosticService == null 
                        ? null
                        : _selectedDiagnosticServiceTreeItem.DiagnosticService.Name;
            }
        }

        public ISelection SelectedDiagnosticServiceTreeItem
        {
            get { return _selectedDiagnosticServiceTreeItem == null ? Selection.Empty : new Selection(_selectedDiagnosticServiceTreeItem); }
            set
            {
                _selectedDiagnosticServiceTreeItem = value.Item as DiagnosticServiceTreeItem;
                UpdateDiagnosticServiceBreakdown();
                
                //DiagnosticServiceSummary diagnosticService = _selectedDiagnosticServiceTreeItem == null ? null : _selectedDiagnosticServiceTreeItem.DiagnosticService;

                //if (diagnosticService != null && diagnosticService.Equals(_selectedDiagnosticService))
                //{
                //    // Do nothing
                //}
                //else
                //{
                //    _selectedDiagnosticService = diagnosticService;
                //    UpdateDiagnosticServiceBreakdown();
                //}
            }
        }

        public List<string> PriorityChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_priorityChoices); }
        }

        public string SelectedPriority
        {
            get { return _selectedPriority == null ? "" : _selectedPriority.Value; }
            set
            {
                _selectedPriority = (value == "") ? null : 
                    CollectionUtils.SelectFirst<EnumValueInfo>(_priorityChoices,
                        delegate(EnumValueInfo info) { return info.Value == value; });
            }
        }

        public List<string> CancelReasonChoices
        {
            get
            {
                List<string> displayValue = EnumValueUtils.GetDisplayValues(_cancelReasonChoices);
                displayValue.Insert(0, "");
                return displayValue;
            }
        }

        public string SelectedCancelReason
        {
            get { return _selectedCancelReason == null ? "" : _selectedCancelReason.Value; }
            set
            {
                _selectedCancelReason = (value == "") ? null :
                    CollectionUtils.SelectFirst<EnumValueInfo>(_cancelReasonChoices,
                        delegate(EnumValueInfo reason) { return reason.Value == value; });
            }
        }

        public List<string> FacilityChoices
        {
            get
            {
                List<string> facilityStrings = new List<string>();
                facilityStrings.Add("");
                facilityStrings.AddRange(
                    CollectionUtils.Map<FacilitySummary, string>(_facilityChoices,
                            delegate(FacilitySummary f) { return f.Name; }));

                return facilityStrings;
            }
        }

        public string SelectedFacility
        {
            get { return _selectedFacility == null ? "" : _selectedFacility.Name; }
            set
            {
                _selectedFacility = (value == "") ? null : 
                    CollectionUtils.SelectFirst<FacilitySummary>(_facilityChoices,
                        delegate(FacilitySummary f) { return f.Name == value; });
            }
        }

        public List<string> OrderingPhysicianChoices
        {
            get
            {
                List<string> physicianStrings = new List<string>();
                physicianStrings.Add("");
                physicianStrings.AddRange(
                    CollectionUtils.Map<ExternalPractitionerSummary, string, List<string>>(_orderingPhysicianChoices,
                            delegate(ExternalPractitionerSummary p) { return PersonNameFormat.Format(p.Name); }));

                return physicianStrings;
            }
        }

        public string SelectedOrderingPhysician
        {
            get { return _selectedOrderingPhysician == null ? "" : PersonNameFormat.Format(_selectedOrderingPhysician.Name); }
            set
            {
                _selectedOrderingPhysician = (value == "") ? null :
                   CollectionUtils.SelectFirst<ExternalPractitionerSummary>(_orderingPhysicianChoices,
                       delegate(ExternalPractitionerSummary p) { return PersonNameFormat.Format(p.Name) == value; });
            }
        }


        public event EventHandler DiagnosticServiceChanged
        {
            add { _diagnosticServiceChanged += value; }
            remove { _diagnosticServiceChanged -= value; }
        }

        public ITree DiagnosticServiceBreakdown
        {
            get { return _diagnosticServiceBreakdown; }
        }

        public ITree DiagnosticServiceTree
        {
            get { return _diagnosticServiceTree; }
        }

        public ISelection SelectedDiagnosticServiceBreakdownItem
        {
            get { return _selectedDiagnosticServiceBreakdownItem == null ? Selection.Empty : new Selection(_selectedDiagnosticServiceBreakdownItem); }
            set
            {
                _selectedDiagnosticServiceBreakdownItem = value.Item;
            }
        }

        public DateTime SchedulingRequestDateTime
        {
            get { return _schedulingRequestDateTime; }
            set { _schedulingRequestDateTime = value; }
        }

        public bool ScheduleOrder
        {
            get { return _scheduleOrder; }
            set { _scheduleOrder = value; }
        }

        public PlaceOrderRequest PlaceOrderRequest
        {
            get
            {
                return new PlaceOrderRequest(
                                _selectedVisit.Patient,
                                _selectedVisit.entityRef,
                                //_selectedDiagnosticService.DiagnosticServiceRef,
                                _selectedDiagnosticServiceTreeItem.DiagnosticService.DiagnosticServiceRef,
                                _selectedPriority,
                                _selectedOrderingPhysician.PractitionerRef,
                                _selectedFacility.FacilityRef,
                                _scheduleOrder,
                                _schedulingRequestDateTime);
            }
        }

        public CancelOrderRequest CancelOrderRequest
        {
            get
            {

                return new CancelOrderRequest(_reOrderDetail.OrderRef, _selectedCancelReason);
            }
        }

        public void PlaceOrder()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            if (_reOrderDetail != null && _selectedCancelReason == null)
            {
                this.Host.DesktopWindow.ShowMessageBox(SR.MessageMissingCancellationReason, MessageBoxActions.Ok);
                return;
            }

            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion

        private void UpdateDiagnosticServiceBreakdown()
        {
            //if (_selectedDiagnosticService == null)
            if(_selectedDiagnosticServiceTreeItem == null || _selectedDiagnosticServiceTreeItem.DiagnosticService == null)
            {
                //_diagnosticServiceBreakdown = null;
                _diagnosticServiceBreakdown = new Tree<RequestedProcedureTypeDetail>(new TreeItemBinding<RequestedProcedureTypeDetail>());
            }
            else
            {
                try
                {
                    DiagnosticServiceDetail diagnosticServiceDetail;

                    Platform.GetService<IOrderEntryService>(
                        delegate(IOrderEntryService service)
                        {
                            //LoadDiagnosticServiceBreakdownResponse response = service.LoadDiagnosticServiceBreakdown(new LoadDiagnosticServiceBreakdownRequest(_selectedDiagnosticService.DiagnosticServiceRef));
                            LoadDiagnosticServiceBreakdownResponse response = service.LoadDiagnosticServiceBreakdown(new LoadDiagnosticServiceBreakdownRequest(_selectedDiagnosticServiceTreeItem.DiagnosticService.DiagnosticServiceRef));
                            diagnosticServiceDetail = response.DiagnosticServiceDetail;

                            _diagnosticServiceBreakdown = new Tree<RequestedProcedureTypeDetail>(
                                new TreeItemBinding<RequestedProcedureTypeDetail>(
                                    delegate(RequestedProcedureTypeDetail rpt) { return rpt.Name; },
                                    delegate(RequestedProcedureTypeDetail rpt)
                                    {
                                        return new Tree<ModalityProcedureStepTypeDetail>(
                                            new TreeItemBinding<ModalityProcedureStepTypeDetail>(
                                                delegate(ModalityProcedureStepTypeDetail spt) { return spt.Name; }),
                                                rpt.ModalityProcedureStepTypes);
                                    }), diagnosticServiceDetail.RequestedProcedureTypes);
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionCannotUpdateDiagnosticServiceBreakdown, this.Host.DesktopWindow,
                        delegate
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }

            EventsHelper.Fire(_diagnosticServiceChanged, this, EventArgs.Empty);
        }

        private ITree ExpandDiagnosticServiceTree(DiagnosticServiceTreeItem item)
        {
            ITree subtree = null;

            try
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        GetDiagnosticServiceSubTreeResponse response = service.GetDiagnosticServiceSubTree(new GetDiagnosticServiceSubTreeRequest(item.NodeRef));

                        TreeItemBinding<DiagnosticServiceTreeItem> binding = new TreeItemBinding<DiagnosticServiceTreeItem>(
                                delegate(DiagnosticServiceTreeItem ds) { return ds.Description; },
                                ExpandDiagnosticServiceTree);
                        binding.CanHaveSubTreeHandler = delegate(DiagnosticServiceTreeItem ds) { return ds.DiagnosticService == null; };
                        subtree = new Tree<DiagnosticServiceTreeItem>(binding, response.DiagnosticServiceSubTree);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionCannotExpandDiagnositicServiceTree, this.Host.DesktopWindow, 
                    delegate
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        this.Host.Exit();
                    });
            }

            return subtree;
        }
    }
}
