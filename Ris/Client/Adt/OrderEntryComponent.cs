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
	[IconSet("neworder", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
	[EnabledStateObserver("neworder", "Enabled", "EnabledChanged")]
    [ClickHandler("neworder", "NewOrder")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationPreviewToolExtensionPoint))]
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
                NewOrder(item, context);
            }
        }

        private static void NewOrder(RegistrationWorklistItem worklistItem, IRegistrationWorkflowItemToolContext context)
        {
            try
            {
                ApplicationComponent.LaunchAsWorkspace(
                    context.DesktopWindow,
                    new OrderEntryComponent(worklistItem.PatientProfileRef),
                    string.Format(SR.TitleNewOrder, PersonNameFormat.Format(worklistItem.Name), MrnFormat.Format(worklistItem.Mrn)),
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

                            // Refresh the schedule folder is a new folder is placed
                            IFolder scheduledFolder = CollectionUtils.SelectFirst<IFolder>(context.Folders,
                                delegate(IFolder f) { return f is Folders.ScheduledFolder; });

                            if (scheduledFolder.IsOpen)
                                scheduledFolder.Refresh();
                            else
                                scheduledFolder.RefreshCount();
                        }
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionCannotPlaceOrder, context.DesktopWindow);
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
                        _selectedCancelReason = _cancelReasonChoices[0];

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

        public override void Stop()
        {
            base.Stop();
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
            get { return EnumValueUtils.GetDisplayValues(_cancelReasonChoices); }
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
