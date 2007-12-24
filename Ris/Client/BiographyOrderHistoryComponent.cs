#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="BiographyOrderHistoryComponent"/>
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
        private readonly EntityRef _patientRef;
        private readonly Table<OrderListItem> _orderList;
        private OrderListItem _selectedOrder;
        private OrderDetail _orderDetail;
        private ModalityProcedureStepDetail _selectedMPS;

        private event EventHandler _diagnosticServiceChanged;
        private Tree<RequestedProcedureDetail> _diagnosticServiceBreakdown;
        private object _selectedDiagnosticServiceBreakdownItem;


        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyOrderHistoryComponent(EntityRef patientRef)
        {
            _patientRef = patientRef;
            _orderList = new Table<OrderListItem>();
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>("Requested For",
                delegate(OrderListItem order) { return Format.DateTime(order.SchedulingRequestTime); }));
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnAccessionNumber,
                delegate(OrderListItem order) { return order.AccessionNumber; }));
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnDiagnosticService,
                delegate(OrderListItem order) { return order.DiagnosticService.Name; }));
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnPriority,
                delegate(OrderListItem order) { return order.OrderPriority.Value; }));
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnStatus,
                delegate(OrderListItem order) { return order.OrderStatus.Value; }));
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>("Ordered by",
                delegate(OrderListItem order) { return PersonNameFormat.Format(order.OrderingPractitioner.Name); }));
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>("Ordering Facility",
                delegate(OrderListItem order) { return order.OrderingFacility.Name; }));
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>("Indication",
                delegate(OrderListItem order) { return order.ReasonForStudy; }));
            _orderList.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnCreatedOn,
                delegate(OrderListItem order) { return Format.DateTime(order.EnteredTime); }));
        }

        public override void Start()
        {


            Platform.GetService<IBrowsePatientDataService>(
                delegate(IBrowsePatientDataService service)
                {
                    GetDataRequest request = new GetDataRequest();
                    request.ListOrdersRequest = new ListOrdersRequest(_patientRef, PatientOrdersQueryDetailLevel.Order);
                    GetDataResponse response = service.GetData(request);

                    _orderList.Items.AddRange(response.ListOrdersResponse.Orders);
                });

            base.Start();
        }

        #region Presentation Model - Order

        public ITable Orders
        {
            get { return _orderList; }
        }

        public ISelection SelectedOrder
        {
            get { return new Selection(_selectedOrder); }
            set
            {
                OrderListItem newSelection = (OrderListItem)value.Item;
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
            get { return _orderDetail == null || _orderDetail.CancelReason == null ? null : _orderDetail.CancelReason.Value; }
        }

        public string Priority
        {
            get { return _orderDetail == null ? null : _orderDetail.OrderPriority.Value; }
        }

        public string SchedulingRequestDateTime
        {
            get { return _orderDetail == null ? null : Format.DateTime(_orderDetail.SchedulingRequestTime); }
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
            get { return _orderDetail == null ? null : VisitNumberFormat.Format(_orderDetail.Visit.VisitNumber); }
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
            get { return _orderDetail == null ? null : Format.DateTime(_orderDetail.Visit.AdmitTime); }
        }

        public string DischargeDateTime
        {
            get { return _orderDetail == null ? null : Format.DateTime(_orderDetail.Visit.DischargeTime); }
        }

        public string AmbulatoryStatus
        {
            get
            {
                return _orderDetail == null ? null :
                    StringUtilities.Combine(
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
            get { return _selectedMPS == null ? null : _selectedMPS.ModalityName; }
        }

        public string MPSState
        {
            get { return _selectedMPS == null ? null : _selectedMPS.State.Value; }
        }

        public string PerformerStaff
        {
            get { return _selectedMPS == null || _selectedMPS.Performer == null ? null : String.Format("{0}, {1}", _selectedMPS.Performer.Name.FamilyName, _selectedMPS.Performer.Name.GivenName); }
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
            get { return _selectedMPS == null || _selectedMPS.ScheduledPerformer == null ? null : String.Format("{0}, {1}", _selectedMPS.ScheduledPerformer.Name.FamilyName, _selectedMPS.ScheduledPerformer.Name.GivenName); }
        }

        public string ScheduledStartTime
        {
            get { return _selectedMPS == null ? null : Format.DateTime(_selectedMPS.ScheduledStartTime); }
        }

        public string ScheduledEndTime
        {
            get { return "FooBar"; }
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
                if (_selectedDiagnosticServiceBreakdownItem == null || _selectedDiagnosticServiceBreakdownItem is ModalityProcedureStepDetail)
                {
                    _selectedMPS = _selectedDiagnosticServiceBreakdownItem as ModalityProcedureStepDetail;
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
                    Platform.GetService<IBrowsePatientDataService>(
                        delegate(IBrowsePatientDataService service)
                        {
                            GetDataRequest request = new GetDataRequest();
                            request.GetOrderDetailRequest = new GetOrderDetailRequest(_selectedOrder.OrderRef, true, true, false, false);
                            GetDataResponse response = service.GetData(request);

                            _orderDetail = response.GetOrderDetailResponse.Order;
                        });

                    _diagnosticServiceBreakdown = new Tree<RequestedProcedureDetail>(
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
                if (_orderDetail.RequestedProcedures[0].ModalityProcedureSteps.Count > 0)
                {
                    this.SelectedDiagnosticServiceBreakdownItem = new Selection(_orderDetail.RequestedProcedures[0].ModalityProcedureSteps[0]);
                }
            }
        }

        private TreeItemBinding<RequestedProcedureDetail> GetRequestedProcedureBinding()
        {
            TreeItemBinding<RequestedProcedureDetail> binding =
                new TreeItemBinding<RequestedProcedureDetail>(
                delegate(RequestedProcedureDetail rp) { return rp.Type.Name; },
                delegate(RequestedProcedureDetail rp)
                {
                    return new Tree<ModalityProcedureStepDetail>(
                        GetModalityProcedureStepBinding(), rp.ModalityProcedureSteps);
                });

            binding.ResourceResolverProvider = delegate{ return new ResourceResolver(this.GetType().Assembly); };
			binding.IconSetProvider = delegate{ return new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenLarge.png"); };
            
            return binding;
        }

        private TreeItemBinding<ModalityProcedureStepDetail> GetModalityProcedureStepBinding()
        {
            TreeItemBinding<ModalityProcedureStepDetail> binding =
                new TreeItemBinding<ModalityProcedureStepDetail>(
                delegate(ModalityProcedureStepDetail mps) { return String.Format("{0} ({1})", mps.ProcedureStepName, mps.ModalityName); });

            binding.ResourceResolverProvider = delegate{ return new ResourceResolver(this.GetType().Assembly); };
            binding.IconSetProvider =
                delegate(ModalityProcedureStepDetail mps)
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
