using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;
using System.Collections;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("neworder", "global-menus/Orders/New")]
    [ClickHandler("neworder", "NewOrder")]
    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class OrderEntryTool : Tool<IWorklistToolContext>
    {
        public void NewOrder()
        {
            OrderEntryComponent component = new OrderEntryComponent(this.Context.SelectedPatientProfile);

            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow,
                component,
                "New Order",
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
        private EntityRef<PatientProfile> _patientProfile;
        private IOrderEntryService _orderEntryService;

        private Patient _patient;

        private VisitTable _visitChoices;
        private Visit _selectedVisit;

        private List<DiagnosticService> _diagnosticServiceChoices;
        private DiagnosticService _selectedDiagnosticService;

        private List<Facility> _facilityChoices;
        private Facility _selectedFacility;

        private List<Practitioner> _orderingPhysicianChoices;
        private Practitioner _selectedOrderingPhysician;

        private OrderPriorityEnumTable _priorityChoices;
        private OrderPriority _selectedPriority;

        private event EventHandler _diagnosticServiceChanged;
        private TreeNodeCollection<RequestedProcedureType> _diagnosticServiceBreakdown;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderEntryComponent(EntityRef<PatientProfile> patientProfile)
        {
            _patientProfile = patientProfile;
        }

        public override void Start()
        {
            _orderEntryService = ApplicationContext.GetService<IOrderEntryService>();
            PatientProfile profile = _orderEntryService.LoadPatientProfile(_patientProfile);
            _patient = profile.Patient;

            IList<Visit> visits = _orderEntryService.ListActiveVisits(new EntityRef<Patient>(_patient));
            _visitChoices = new VisitTable();
            _visitChoices.Items.AddRange(visits);

            _diagnosticServiceChoices = new List<DiagnosticService>();
            _diagnosticServiceChoices.AddRange(_orderEntryService.ListDiagnosticServiceChoices());

            _facilityChoices = new List<Facility>(_orderEntryService.ListOrderingFacilityChoices());
            _priorityChoices = _orderEntryService.GetOrderPriorityEnumTable();

            _orderingPhysicianChoices = new List<Practitioner>(_orderEntryService.ListOrderingPhysicianChoices());

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITable VisitChoices
        {
            get { return _visitChoices; }
        }

        public void SetVisitSelection(ISelection sel)
        {
            _selectedVisit = (Visit)sel.Item;
        }

        public IList DiagnosticServiceChoices
        {
            get { return _diagnosticServiceChoices; }
        }

        public DiagnosticService SelectedDiagnosticService
        {
            get { return _selectedDiagnosticService; }
            set
            {
                if (value == null || !value.Equals(_selectedDiagnosticService))
                {
                    _selectedDiagnosticService = value;
                    UpdateDiagnosticServiceBreakdown();
                }
            }
        }

        public string[] PriorityChoices
        {
            get { return _priorityChoices.Values; }
        }

        public string SelectedPriority
        {
            get { return _priorityChoices[_selectedPriority].Value; }
            set { _selectedPriority = _priorityChoices[value].Code; }
        }

        public IList FacilityChoices
        {
            get { return _facilityChoices; }
        }

        public Facility SelectedFacility
        {
            get { return _selectedFacility; }
            set { _selectedFacility = value; }
        }

        public IList OrderingPhysicianChoices
        {
            get { return _orderingPhysicianChoices; }
        }

        public Practitioner SelectedOrderingPhysician
        {
            get { return _selectedOrderingPhysician; }
            set { _selectedOrderingPhysician = value; }
        }


        public event EventHandler DiagnosticServiceChanged
        {
            add { _diagnosticServiceChanged += value; }
            remove { _diagnosticServiceChanged -= value; }
        }

        public ITreeNodeCollection DiagnosticServiceBreakdown
        {
            get { return _diagnosticServiceBreakdown; }
        }

        public void PlaceOrder()
        {
            try
            {
                _orderEntryService.PlaceOrder(
                        _patient,
                        _selectedVisit,
                        _selectedDiagnosticService,
                        _selectedPriority,
                        _selectedOrderingPhysician,
                        _selectedFacility);

            }
            catch (Exception e)
            {
                // TODO fix this up!!!
                Platform.Log(e, LogLevel.Error);
                this.Host.ShowMessageBox("Failed to place order", MessageBoxActions.Ok);
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
                _selectedDiagnosticService = _orderEntryService.LoadDiagnosticServiceBreakdown(new EntityRef<DiagnosticService>(_selectedDiagnosticService));

                _diagnosticServiceBreakdown = new TreeNodeCollection<RequestedProcedureType>(
                    _selectedDiagnosticService.RequestedProcedureTypes,
                    delegate(RequestedProcedureType rpt)
                    {
                        return new TreeNode<RequestedProcedureType>(rpt,
                            delegate(RequestedProcedureType rpt1) { return rpt1.ToString(); },
                            delegate(RequestedProcedureType rpt1)
                            {
                                return new TreeNodeCollection<ScheduledProcedureStepType>(
                                    rpt1.ScheduledProcedureStepTypes,
                                    delegate(ScheduledProcedureStepType spt)
                                    {
                                        return new TreeNode<ScheduledProcedureStepType>(spt,
                                            delegate(ScheduledProcedureStepType spt1) { return spt1.ToString(); },
                                            null);
                                    });
                            });
                    });
            }

            EventsHelper.Fire(_diagnosticServiceChanged, this, EventArgs.Empty);
        }
    }
}
