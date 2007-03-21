using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;
using System.Collections;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("neworder", "RegistrationPreview-menu/NewOrders")]
    [MenuAction("neworder", "global-menus/Orders/New")]
    [EnabledStateObserver("neworder", "Enabled", "EnabledChanged")]
    [ClickHandler("neworder", "NewOrder")]
    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
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
                this.Enabled = (((IRegistrationPreviewToolContext)this.ContextBase).WorklistItem.PatientProfileRef != null);
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
            if (this.ContextBase is IWorklistToolContext)
            {
                IWorklistToolContext context = (IWorklistToolContext)this.ContextBase;
                OrderEntryComponent component = new OrderEntryComponent(context.SelectedPatientProfile);
                ApplicationComponent.LaunchAsWorkspace(
                    context.DesktopWindow,
                    component,
                    SR.TitleNewOrder,
                    null);
            }
            else if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                WorklistItem item = CollectionUtils.FirstElement<WorklistItem>(context.SelectedItems);
                OrderEntryComponent component = new OrderEntryComponent(item.PatientProfile);
                ApplicationComponent.LaunchAsWorkspace(
                    context.DesktopWindow,
                    component,
                    SR.TitleNewOrder,
                    null);
            }
            else if (this.ContextBase is IRegistrationPreviewToolContext)
            {
                IRegistrationPreviewToolContext context = (IRegistrationPreviewToolContext)this.ContextBase;
                OrderEntryComponent component = new OrderEntryComponent(context.WorklistItem.PatientProfileRef);
                ApplicationComponent.LaunchAsWorkspace(
                    context.DesktopWindow,
                    component,
                    SR.TitleNewOrder,
                    null);
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
        private EntityRef _patientProfileRef;
        private EntityRef _patientRef;

        private VisitTable _visitTable;
        private List<DiagnosticServiceSummary> _diagnosticServiceChoices;
        private List<FacilitySummary> _facilityChoices;
        private List<PractitionerSummary> _orderingPhysicianChoices;
        private List<EnumValueInfo> _priorityChoices;

        private VisitSummary _selectedVisit;
        private DiagnosticServiceSummary _selectedDiagnosticService;
        private FacilitySummary _selectedFacility;
        private PractitionerSummary _selectedOrderingPhysician;
        private EnumValueInfo _selectedPriority;

        private event EventHandler _diagnosticServiceChanged;
        private Tree<RequestedProcedureTypeDetail> _diagnosticServiceBreakdown;
        private object _selectedDiagnosticServiceBreakdownItem;

        private DateTime _schedulingRequestDateTime;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderEntryComponent(EntityRef patientProfileRef)
        {
            _patientProfileRef = patientProfileRef;
        }

        public override void Start()
        {
            try
            {
                _visitTable = new VisitTable();

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
                    });

                _schedulingRequestDateTime = Platform.Time;            
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
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
                    CollectionUtils.Map<DiagnosticService, string>(
                        _diagnosticServiceChoices, delegate(DiagnosticService ds) { return ds.Name; }));

                return dsStrings;
            }
        }

        public string SelectedDiagnosticService
        {
            get { return _selectedDiagnosticService == null ? "" : _selectedDiagnosticService.Name; }
            set
            {
                DiagnosticServiceSummary diagnosticService = (value == "") ? null :
                    CollectionUtils.SelectFirst<DiagnosticServiceSummary>(_diagnosticServiceChoices,
                            delegate(DiagnosticServiceSummary ds) { return ds.Name == value; });

                if (diagnosticService == null || !diagnosticService.Equals(_selectedDiagnosticService))
                {
                    _selectedDiagnosticService = diagnosticService;
                    UpdateDiagnosticServiceBreakdown();
                }
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
                    CollectionUtils.SelectFirst<Facility>(_facilityChoices,
                        delegate(Facility f) { return f.Name == value; });
            }
        }

        public List<string> OrderingPhysicianChoices
        {
            get
            {
                List<string> physicianStrings = new List<string>();
                physicianStrings.Add("");
                physicianStrings.AddRange(
                    CollectionUtils.Map<PractitionerSummary, string>(_orderingPhysicianChoices,
                            delegate(PractitionerSummary p) { return Format.Custom(p.Name); }));

                return physicianStrings;
            }
        }

        public string SelectedOrderingPhysician
        {
            get { return _selectedOrderingPhysician == null ? "" : Format.Custom(_selectedOrderingPhysician.PersonNameDetail); }
            set
            {
                _selectedOrderingPhysician = (value == "") ? null :
                   CollectionUtils.SelectFirst<PractitionerSummary>(_orderingPhysicianChoices,
                       delegate(PractitionerSummary p) { return Format.Custom(p.PersonNameDetail) == value; });
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
                            _patientRef,
                            _selectedVisit,
                            _selectedDiagnosticService,
                            _selectedPriority,
                            _selectedOrderingPhysician,
                            _selectedFacility,
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
            if (_selectedDiagnosticService == null)
            {
                _diagnosticServiceBreakdown = null;
            }
            else
            {
                try
                {
                    DiagnosticServiceDetail diagnosticServiceDetail;

                    Platform.GetService<IOrderEntryService>(
                        delegate(IOrderEntryService service)
                        {
                            LoadDiagnosticServiceBreakdownResponse response = service.LoadDiagnosticServiceBreakdown(new LoadDiagnosticServiceBreakdownRequest(_selectedDiagnosticService.DiagnosticServiceRef));
                            diagnosticServiceDetail = response.DiagnosticServiceDetail;
                        });

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
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }

            EventsHelper.Fire(_diagnosticServiceChanged, this, EventArgs.Empty);
        }
    }
}
