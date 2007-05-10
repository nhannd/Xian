using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Trees;

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

            try
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        ListOrdersForPatientResponse response = service.ListOrdersForPatient(new ListOrdersForPatientRequest(_patientProfileRef));
                        _orderList.Items.AddRange(response.Orders);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
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

        public DateTime? SchedulingRequestDateTime
        {
            get { return _orderDetail == null ? null : _orderDetail.SchedulingRequestDateTime; }
        }

        public string OrderingPhysician
        {
            get { return _orderDetail == null ? null : String.Format("{0}, {1}", _orderDetail.OrderingPractitioner.PersonNameDetail.FamilyName, _orderDetail.OrderingPractitioner.PersonNameDetail.GivenName); }
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

        public DateTime? AdmitDateTime
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.AdmitDateTime; }
        }

        public DateTime? DischargeDateTime
        {
            get { return _orderDetail == null ? null : _orderDetail.Visit.DischargeDateTime; }
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
            get { return _selectedMPS == null ? null : String.Format("{0}, {1}", _selectedMPS.PerformerStaff.PersonNameDetail.FamilyName, _selectedMPS.PerformerStaff.PersonNameDetail.GivenName); }
        }

        public DateTime? StartTime
        {
            get { return _selectedMPS == null ? null : _selectedMPS.StartTime; }
        }

        public DateTime? EndTime
        {
            get { return _selectedMPS == null ? null : _selectedMPS.EndTime; }
        }

        public string ScheduledPerformerStaff
        {
            get { return _selectedMPS == null ? null : String.Format("{0}, {1}", _selectedMPS.ScheduledPerformerStaff.PersonNameDetail.FamilyName, _selectedMPS.ScheduledPerformerStaff.PersonNameDetail.GivenName); }
        }

        public DateTime? ScheduledStartTime
        {
            get { return _selectedMPS == null ? null : _selectedMPS.ScheduledStartTime; }
        }

        public DateTime? ScheduledEndTime
        {
            get { return _selectedMPS == null ? null : _selectedMPS.ScheduledEndTime; }
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
                    Platform.GetService<IOrderEntryService>(
                        delegate(IOrderEntryService service)
                        {
                            LoadOrderDetailResponse response = service.LoadOrderDetail(new LoadOrderDetailRequest(_selectedOrder.OrderRef));
                            _orderDetail = response.OrderDetail;
                        });

                    _diagnosticServiceBreakdown = new Tree<RequestedProcedureSummary>(
                        new TreeItemBinding<RequestedProcedureSummary>(
                            delegate(RequestedProcedureSummary rp) { return rp.Type.Name; },
                            delegate(RequestedProcedureSummary rp)
                            {
                                return new Tree<ModalityProcedureStepSummary>(
                                    new TreeItemBinding<ModalityProcedureStepSummary>(
                                        delegate(ModalityProcedureStepSummary mps) { return mps.Type.Name; }),
                                        rp.ProcedureSteps);
                            }), _orderDetail.RequestedProcedures);

                    this.SelectedDiagnosticServiceBreakdownItem = Selection.Empty;

                    EventsHelper.Fire(_diagnosticServiceChanged, this, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            NotifyAllPropertiesChanged();
        }

    }
}
