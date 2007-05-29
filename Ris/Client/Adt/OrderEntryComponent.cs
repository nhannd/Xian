using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                _enabled = false;   // disable by default
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItemsChanged += delegate(object sender, EventArgs args)
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                        && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else if (this.ContextBase is IRegistrationPreviewToolContext)
            {
                IRegistrationPreviewToolContext context = (IRegistrationPreviewToolContext)this.ContextBase;
                this.Enabled = (context.WorklistItem != null && context.WorklistItem.PatientProfileRef != null);
            }
            else
            {
                _enabled = true;
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
                NewOrder(item, context.DesktopWindow);
            }
            else if (this.ContextBase is IRegistrationPreviewToolContext)
            {
                IRegistrationPreviewToolContext context = (IRegistrationPreviewToolContext)this.ContextBase;
                NewOrder(context.WorklistItem, context.DesktopWindow);
            }
        }

        private void NewOrder(RegistrationWorklistItem worklistItem, IDesktopWindow desktopWindow)
        {
            OrderEntryComponent component = new OrderEntryComponent(worklistItem.PatientProfileRef);
            ApplicationComponent.LaunchAsWorkspace(
                desktopWindow,
                component,
                string.Format(SR.TitleNewOrder, PersonNameFormat.Format(worklistItem.Name), MrnFormat.Format(worklistItem.Mrn)),
                null);
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
        private EntityRef _patientProfileRef;

        private VisitSummaryTable _visitTable;
        private List<DiagnosticServiceSummary> _diagnosticServiceChoices;
        private List<FacilitySummary> _facilityChoices;
        private List<PractitionerSummary> _orderingPhysicianChoices;
        private List<EnumValueInfo> _priorityChoices;

        private VisitSummary _selectedVisit;
        //private DiagnosticServiceSummary _selectedDiagnosticService;
        private FacilitySummary _selectedFacility;
        private PractitionerSummary _selectedOrderingPhysician;
        private EnumValueInfo _selectedPriority;

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

        public override void Start()
        {
            try
            {
                _visitTable = new VisitSummaryTable();

                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        ListActiveVisitsForPatientResponse response = service.ListActiveVisitsForPatient(new ListActiveVisitsForPatientRequest(_patientProfileRef));
                        _visitTable.Items.AddRange(response.Visits);

                        GetOrderEntryFormDataResponse formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());
                        _diagnosticServiceChoices = formChoicesResponse.DiagnosticServiceChoices;
                        _facilityChoices = formChoicesResponse.OrderingFacilityChoices;
                        _orderingPhysicianChoices = formChoicesResponse.OrderingPhysicianChoices;
                        _priorityChoices = formChoicesResponse.OrderPriorityChoices;

                        _selectedPriority = _priorityChoices[0];

                        TreeItemBinding<DiagnosticServiceTreeItem> binding = new TreeItemBinding<DiagnosticServiceTreeItem>(
                                delegate(DiagnosticServiceTreeItem ds) { return ds.Description; },
                                ExpandDiagnosticServiceTree);
                        binding.CanHaveSubTreeHandler = delegate(DiagnosticServiceTreeItem ds) { return ds.DiagnosticService == null; };
                        _diagnosticServiceTree = new Tree<DiagnosticServiceTreeItem>(binding, formChoicesResponse.TopLevelDiagnosticServiceTree);
                    });

                _schedulingRequestDateTime = Platform.Time;
                _scheduleOrder = true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

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
                    CollectionUtils.Map<PractitionerSummary, string, List<string>>(_orderingPhysicianChoices,
                            delegate(PractitionerSummary p) { return PersonNameFormat.Format(p.PersonNameDetail); }));

                return physicianStrings;
            }
        }

        public string SelectedOrderingPhysician
        {
            get { return _selectedOrderingPhysician == null ? "" : PersonNameFormat.Format(_selectedOrderingPhysician.PersonNameDetail); }
            set
            {
                _selectedOrderingPhysician = (value == "") ? null :
                   CollectionUtils.SelectFirst<PractitionerSummary>(_orderingPhysicianChoices,
                       delegate(PractitionerSummary p) { return PersonNameFormat.Format(p.PersonNameDetail) == value; });
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

        public void PlaceOrder()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            try
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        PlaceOrderResponse response = service.PlaceOrder(new PlaceOrderRequest(
                            _selectedVisit.Patient,
                            _selectedVisit.entityRef,
                            //_selectedDiagnosticService.DiagnosticServiceRef,
                            _selectedDiagnosticServiceTreeItem.DiagnosticService.DiagnosticServiceRef,
                            _selectedPriority,
                            _selectedOrderingPhysician.StaffRef,
                            _selectedFacility.FacilityRef,
                            _scheduleOrder,
                            _schedulingRequestDateTime));
                    });

                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }

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
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }

            EventsHelper.Fire(_diagnosticServiceChanged, this, EventArgs.Empty);
        }

        private ITree ExpandDiagnosticServiceTree(DiagnosticServiceTreeItem item)
        {
            try
            {
                ITree subtree = null;

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

                return subtree;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
                return null;
            }
        }
    }
}
