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
    [MenuAction("neworder", "global-menus/Order Entry/New")]
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

        private Order _order;
        private VisitTable _visitChoices;
        private List<DiagnosticService> _diagnosticServiceChoices;
        private List<Facility> _facilityChoices;
        private List<Practitioner> _orderingPhysicianChoices;
        private OrderPriorityEnumTable _priorityChoices;

        private DiagnosticService _diagnosticService;
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
            _order = new Order();

            _orderEntryService = ApplicationContext.GetService<IOrderEntryService>();
            PatientProfile profile = _orderEntryService.LoadPatientProfile(_patientProfile);
            _order.Patient = profile.Patient;

            IList<Visit> visits = _orderEntryService.ListActiveVisits(new EntityRef<Patient>(_order.Patient));
            _visitChoices = new VisitTable();
            _visitChoices.Items.AddRange(visits);

            _diagnosticServiceChoices = new List<DiagnosticService>(_orderEntryService.ListDiagnosticServiceChoices());
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

        public IList DiagnosticServiceChoices
        {
            get { return _diagnosticServiceChoices; }
        }

        public string[] PriorityChoices
        {
            get { return _priorityChoices.Values; }
        }

        public IList FacilityChoices
        {
            get { return _facilityChoices; }
        }

        public IList OrderingPhysicianChoices
        {
            get { return _orderingPhysicianChoices; }
        }

        public DiagnosticService DiagnosticService
        {
            get { return _diagnosticService; }
            set
            {
                if (value == null || !value.Equals(_diagnosticService))
                {
                    _diagnosticService = value;
                    UpdateDiagnosticServiceBreakdown();
                }
            }
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

        #endregion

        private void UpdateDiagnosticServiceBreakdown()
        {
            if (_diagnosticService == null)
            {
                _diagnosticServiceBreakdown = null;
            }
            else
            {
                _diagnosticService = _orderEntryService.LoadDiagnosticServiceBreakdown(new EntityRef<DiagnosticService>(_diagnosticService));

                _diagnosticServiceBreakdown = new TreeNodeCollection<RequestedProcedureType>(
                    _diagnosticService.RequestedProcedureTypes,
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
