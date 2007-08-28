using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PatientBiography;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientOrderHistoryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientOrderHistoryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientOrderHistoryComponent class
    /// </summary>
    [AssociateView(typeof(PatientOrderHistoryComponentViewExtensionPoint))]
    public class BiographyOrderHistoryComponent : ApplicationComponent
    {
        private EntityRef _patientProfileRef;
        private OrderSummaryTable _orderList;
        private OrderSummary _selectedOrder;
        private OrderDetail _orderDetail;
        private ModalityProcedureStepSummary _selectedMPS;

        private event EventHandler _diagnosticServiceChanged;
        private Tree<RequestedProcedureSummary> _diagnosticServiceBreakdown;
        private object _selectedDiagnosticServiceBreakdownItem;


        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyOrderHistoryComponent(EntityRef patientProfileRef)
        {
            _patientProfileRef = patientProfileRef;
            _orderList = new OrderSummaryTable();
        }

        public override void Start()
        {
            base.Start();

            Platform.GetService<IPatientBiographyService>(
                delegate(IPatientBiographyService service)
                {
                    ListOrdersForPatientResponse response = service.ListOrdersForPatient(new ListOrdersForPatientRequest(_patientProfileRef));
                    _orderList.Items.AddRange(response.Orders);
                });
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model - Order

        public ITable Orders
        {
            get { return _orderList; }
        }

        public ISelection SelectedOrder
        {
            get { return _selectedOrder == null ? Selection.Empty : new Selection(_selectedOrder); }
            set
            {
                OrderSummary newSelection = (OrderSummary)value.Item;
                if (_selectedOrder != newSelection)
                {
                    _selectedOrder = newSelection;
                    OrderSelectionChanged();
                }
            }
        }

        public string PlacerNumber
        {
            get { return _orderDetail == null ? null : _orderDetail.PlacerNumber; }
        }

        public string AccessionNumber
        {
            get { return _orderDetail == null ? null : _orderDetail.AccessionNumber; }
        }

        public string ReasonForStudy
        {
            get { return _orderDetail == null ? null : _orderDetail.ReasonForStudy; }
        }

        public string CancelReason
        {
            get { return _orderDetail == null ? null : _orderDetail.CancelReason.Value; }
        }

        public string Priority
        {
            get { return _orderDetail == null ? null : _orderDetail.OrderPriority.Value; }
        }

        public string SchedulingRequestDateTime
        {
            get { return _orderDetail == null ? null : Format.DateTime(_orderDetail.SchedulingRequestDateTime); }
        }

        public string OrderingPhysician
        {
            get { return _orderDetail == null ? null : String.Format("{0}, {1}", _orderDetail.OrderingPractitioner.Name.FamilyName, _orderDetail.OrderingPractitioner.Name.GivenName); }
        }

        public string OrderingFacility
        {
            get { return _orderDetail == null ? null : _orderDetail.OrderingFacility.Name; }
        }

        #endregion

        #region Presentation Model - Visit

        public string VisitNumber
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.VisitNumberId; }
        }

        public string Site
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.VisitNumberAssigningAuthority; }
        }

        public string PreAdmitNumber
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.PreadmitNumber; }
        }

        public string PatientClass
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.PatientClass.Value; }
        }

        public string PatientType
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.PatientType.Value; }
        }

        public string AdmissionType
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.AdmissionType.Value; }
        }

        public string VisitStatus
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.Status.Value; }
        }

        public string AdmitDateTime
        {
            get { return _orderDetail == null ? null : Format.DateTime(_orderDetail.Visit.AdmitDateTime); }
        }

        public string DischargeDateTime
        {
            get { return _orderDetail == null ? null : Format.DateTime(_orderDetail.Visit.DischargeDateTime); }
        }

        public string AmbulatoryStatus
        {
            get
            {
                return _orderDetail == null ? null :
                    StringUtilities.Combine<string>(
                        CollectionUtils.Map<EnumValueInfo, String, List<string>>(_orderDetail.Visit.AmbulatoryStatuses,
                            delegate(EnumValueInfo status)
                            {
                                return status.Value;
                            }),
                            "/");
            }
        }

        public bool VIP
        {
            get { return _orderDetail == null ? false : _orderDetail.Visit.VipIndicator; }
        }

        #endregion

        #region Presentation Model - ProcedureStep

        public string Modality
        {
            get { return _selectedMPS == null ? null : _selectedMPS.Type.DefaultModality.Name; }
        }

        public string MPSState
        {
            get { return _selectedMPS == null ? null : _selectedMPS.State.Value; }
        }

        public string PerformerStaff
        {
            get { return _selectedMPS == null ? null : String.Format("{0}, {1}", _selectedMPS.PerformerStaff.Name.FamilyName, _selectedMPS.PerformerStaff.Name.GivenName); }
        }

        public string StartTime
        {
            get { return _selectedMPS == null ? null : Format.DateTime(_selectedMPS.StartTime); }
        }

        public string EndTime
        {
            get { return _selectedMPS == null ? null : Format.DateTime(_selectedMPS.EndTime); }
        }

        public string ScheduledPerformerStaff
        {
            get { return _selectedMPS == null ? null : String.Format("{0}, {1}", _selectedMPS.ScheduledPerformerStaff.Name.FamilyName, _selectedMPS.ScheduledPerformerStaff.Name.GivenName); }
        }

        public string ScheduledStartTime
        {
            get { return _selectedMPS == null ? null : Format.DateTime(_selectedMPS.ScheduledStartTime); }
        }

        public string ScheduledEndTime
        {
            get { return _selectedMPS == null ? null : Format.DateTime(_selectedMPS.ScheduledEndTime); }
        }

        #endregion

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
                if (_selectedDiagnosticServiceBreakdownItem == null || _selectedDiagnosticServiceBreakdownItem is ModalityProcedureStepSummary)
                {
                    _selectedMPS = _selectedDiagnosticServiceBreakdownItem as ModalityProcedureStepSummary;
                    NotifyAllPropertiesChanged();
                }
            }
        }

        private void OrderSelectionChanged()
        {
            try
            {
                if (_selectedOrder != null)
                {
                    Platform.GetService<IPatientBiographyService>(
                        delegate(IPatientBiographyService service)
                        {
                            LoadOrderDetailResponse response = service.LoadOrderDetail(new LoadOrderDetailRequest(_selectedOrder.OrderRef));
                            _orderDetail = response.OrderDetail;
                        });

                    _diagnosticServiceBreakdown = new Tree<RequestedProcedureSummary>(
                        GetRequestedProcedureBinding(), _orderDetail.RequestedProcedures);

                    EventsHelper.Fire(_diagnosticServiceChanged, this, EventArgs.Empty);

                    SelectFirstProcedureStep();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            NotifyAllPropertiesChanged();
        }

        private void SelectFirstProcedureStep()
        {
            if (_orderDetail.RequestedProcedures.Count > 0)
            {
                if (_orderDetail.RequestedProcedures[0].ProcedureSteps.Count > 0)
                {
                    this.SelectedDiagnosticServiceBreakdownItem = new Selection(_orderDetail.RequestedProcedures[0].ProcedureSteps[0]);
                }
            }
        }

        private TreeItemBinding<RequestedProcedureSummary> GetRequestedProcedureBinding()
        {
            TreeItemBinding<RequestedProcedureSummary> binding = 
                new TreeItemBinding<RequestedProcedureSummary>(
                delegate(RequestedProcedureSummary rp) { return rp.Type.Name; },
                delegate(RequestedProcedureSummary rp)
                {
                    return new Tree<ModalityProcedureStepSummary>(
                        GetModalityProcedureStepBinding(), rp.ProcedureSteps);
                });

            binding.ResourceResolverProvider = delegate(RequestedProcedureSummary rp) { return new ResourceResolver(this.GetType().Assembly); };
			binding.IconSetProvider = delegate(RequestedProcedureSummary rp) { return new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenLarge.png"); };
            
            return binding;
        }

        private TreeItemBinding<ModalityProcedureStepSummary> GetModalityProcedureStepBinding()
        {
            TreeItemBinding<ModalityProcedureStepSummary> binding =
                new TreeItemBinding<ModalityProcedureStepSummary>(
                delegate(ModalityProcedureStepSummary mps) { return String.Format("{0} ({1})", mps.Type.Name, mps.Type.DefaultModality.Name); });

            binding.ResourceResolverProvider = delegate(ModalityProcedureStepSummary mps) { return new ResourceResolver(this.GetType().Assembly); };
            binding.IconSetProvider =
                delegate(ModalityProcedureStepSummary mps)
                {
                    if (mps.State.Code == "SC")
						return new IconSet(IconScheme.Colour, "EditToolSmall.png", "EditToolSmall.png", "EditToolSmall.png");
                    else if (mps.State.Code == "IP")
                        return new IconSet(IconScheme.Colour, "AlertClock.png", "AlertClock.png", "AlertClock.png");
                    else if (mps.State.Code == "CM")
                        return new IconSet(IconScheme.Colour, "CheckSmall.png", "CheckSmall.png", "CheckSmall.png");
                    else if (mps.State.Code == "DC")
						return new IconSet(IconScheme.Colour, "DeleteToolSmall.png", "DeleteToolSmall.png", "DeleteToolSmall.png");

                    return null;
                };

            return binding;
        }
    }
}
