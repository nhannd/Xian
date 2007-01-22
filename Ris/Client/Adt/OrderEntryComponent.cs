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
                SR.TitleNewOrder,
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

        private IList<DiagnosticService> _diagnosticServiceChoices;
        private DiagnosticService _selectedDiagnosticService;

        private IList<Facility> _facilityChoices;
        private Facility _selectedFacility;

        private IList<Practitioner> _orderingPhysicianChoices;
        private Practitioner _selectedOrderingPhysician;

        private OrderPriorityEnumTable _priorityChoices;
        private OrderPriority _selectedPriority;

        private event EventHandler _diagnosticServiceChanged;
        private Tree<RequestedProcedureType> _diagnosticServiceBreakdown;
        private object _selectedDiagnosticServiceBreakdownItem;

        private DateTime _schedulingRequestDateTime;

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

            _diagnosticServiceChoices = _orderEntryService.ListDiagnosticServiceChoices();
            _facilityChoices = _orderEntryService.ListOrderingFacilityChoices();
            _orderingPhysicianChoices = _orderEntryService.ListOrderingPhysicianChoices();
            _priorityChoices = _orderEntryService.GetOrderPriorityEnumTable();

            _schedulingRequestDateTime = Platform.Time;


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

        public ISelection SelectedVisit
        {
            get { return _selectedVisit == null ? Selection.Empty : new Selection(_selectedVisit); }
            set { _selectedVisit = (Visit)value.Item; }
        }

        public IList DiagnosticServiceChoices
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
                DiagnosticService diagnosticService = (value == "") ? null :
                    CollectionUtils.SelectFirst<DiagnosticService>(_diagnosticServiceChoices,
                            delegate(DiagnosticService ds) { return ds.Name == value; });

                if (diagnosticService == null || !diagnosticService.Equals(_selectedDiagnosticService))
                {
                    _selectedDiagnosticService = diagnosticService;
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
            get
            {
                List<string> facilityStrings = new List<string>();
                facilityStrings.Add("");
                facilityStrings.AddRange(
                    CollectionUtils.Map<Facility, string>(_facilityChoices,
                            delegate(Facility f) { return f.Name; }));

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

        public IList OrderingPhysicianChoices
        {
            get
            {
                List<string> physicianStrings = new List<string>();
                physicianStrings.Add("");
                physicianStrings.AddRange(
                    CollectionUtils.Map<Practitioner, string>(_orderingPhysicianChoices,
                            delegate(Practitioner p) { return Format.Custom(p.Name); }));

                return physicianStrings;
            }
        }

        public string SelectedOrderingPhysician
        {
            get { return _selectedOrderingPhysician == null ? "" : Format.Custom(_selectedOrderingPhysician.Name); }
            set
            {
                _selectedOrderingPhysician = (value == "") ? null :
                   CollectionUtils.SelectFirst<Practitioner>(_orderingPhysicianChoices,
                       delegate(Practitioner p) { return Format.Custom(p.Name) == value; });
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
                _orderEntryService.PlaceOrder(
                        _patient,
                        _selectedVisit,
                        _selectedDiagnosticService,
                        _selectedPriority,
                        _selectedOrderingPhysician,
                        _selectedFacility,
                        _schedulingRequestDateTime);

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

                _diagnosticServiceBreakdown = new Tree<RequestedProcedureType>(
                    new TreeItemBinding<RequestedProcedureType>(
                        delegate(RequestedProcedureType rpt) { return rpt.Name; },
                        delegate(RequestedProcedureType rpt)
                        {
                            return new Tree<ModalityProcedureStepType>(
                                new TreeItemBinding<ModalityProcedureStepType>(
                                    delegate(ModalityProcedureStepType spt) { return spt.Name; }),
                                    rpt.ModalityProcedureStepTypes);
                        }), _selectedDiagnosticService.RequestedProcedureTypes);
            }

            EventsHelper.Fire(_diagnosticServiceChanged, this, EventArgs.Empty);
        }
    }
}
