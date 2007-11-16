using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="ProtocolEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolEditorComponent class
    /// </summary>
    [AssociateView(typeof(ProtocolEditorComponentViewExtensionPoint))]
    public class ProtocolEditorComponent : ApplicationComponent
    {
        #region Order Summary Component class

        class OrderSummaryComponent : DHtmlComponent
        {
            private readonly ProtocolEditorComponent _owner;

            public OrderSummaryComponent(ProtocolEditorComponent owner)
            {
                _owner = owner;
            }

            public override void Start()
            {
                //SetUrl(ProtocolEditorComponentSettings.Default.OrderSummaryUrl);
                SetUrl("http://localhost/RIS/technologistdocumentation-ordersummary.htm");
                base.Start();
            }

            protected override object GetWorklistItem()
            {
                return _owner._worklistItem;
            }
        }

        #endregion

        #region ProtocolNotesSummaryComponent class

        class ProtocolNotesSummaryComponent : DHtmlComponent
        {
            private readonly ProtocolEditorComponent _owner;

            public ProtocolNotesSummaryComponent(ProtocolEditorComponent owner)
            {
                this._owner = owner;
            }

            public override void Start()
            {
                SetUrl("http://localhost/RIS/protocolnotessummary.htm");
                base.Start();
            }

            protected override object GetWorklistItem()
            {
                return _owner._worklistItem;
            }

            protected override string GetTagData(string tag)
            {
                if(string.Equals(tag, "Notes"))
                {
                    
                }
                    
                //return base.GetTagData(tag);
                return "";
            }
        }

        #endregion

        #region Private Fields

        private readonly ReportingWorklistItem _worklistItem;
        private EntityRef _orderRef;

        private ProtocolEditorProcedurePlanSummaryTable _procedurePlanSummaryTable;
        private ProtocolEditorProcedurePlanSummaryTableItem _selectedProcodurePlanSummaryTableItem;

        private ProtocolCodeTable _availableProtocolCodes;
        private ProtocolCodeTable _selectedProtocolCodes;

        private event EventHandler _selectionChanged;

        private ChildComponentHost _orderSummaryComponentHost;
        private ChildComponentHost _protocolNoteSummaryComponentHost;

        private bool _acceptEnabled;
        private bool _rejectEnabled;
        private bool _suspendEnabled;
        private bool _saveEnabled;

        private List<ProtocolGroupSummary> _protocolGroupChoices;
        private ProtocolGroupSummary _protocolGroup;

        private event EventHandler _protocolAccepted;
        private event EventHandler _protocolRejected;
        private event EventHandler _protocolSuspended;
        private event EventHandler _protocolSaved;
        private event EventHandler _protocolCancelled;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolEditorComponent(ReportingWorklistItem worklistItem)
        {
            _worklistItem = worklistItem;
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            _availableProtocolCodes = new ProtocolCodeTable();
            _selectedProtocolCodes = new ProtocolCodeTable();
            _protocolGroupChoices = new List<ProtocolGroupSummary>();

            InitializeProcedurePlanSummary();

            _orderSummaryComponentHost = new ChildComponentHost(this.Host, new OrderSummaryComponent(this));
            _orderSummaryComponentHost.StartComponent();

            _protocolNoteSummaryComponentHost = new ChildComponentHost(this.Host, new ProtocolNotesSummaryComponent(this));
            _protocolNoteSummaryComponentHost.StartComponent();

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        #region Presentation Model

        public ApplicationComponentHost OrderSummaryComponentHost
        {
            get { return _orderSummaryComponentHost; }
        }

        public ApplicationComponentHost ProtocolNotesSummaryComponentHost
        {
            get { return _protocolNoteSummaryComponentHost; }
        }

        public IList<string> ProtocolGroupChoices
        {
            get
            {
                return CollectionUtils.Map<ProtocolGroupSummary, string>(
                    _protocolGroupChoices,
                    delegate(ProtocolGroupSummary summary) { return summary.Name; });
            }
        }

        public string ProtocolGroup
        {
            get { return _protocolGroup == null ? "" : _protocolGroup.Name; }
            set
            {
                _protocolGroup = (value == null) 
                    ? null 
                    : CollectionUtils.SelectFirst<ProtocolGroupSummary>(
                        _protocolGroupChoices, 
                        delegate(ProtocolGroupSummary summary) { return summary.Name == value; });

                OnProtocolGroupSelectionChanged();
            }
        }

        public ITable ProcedurePlanSummaryTable
        {
            get { return _procedurePlanSummaryTable; }
        }

        public ITable AvailableProtocolCodesTable
        {
            get { return _availableProtocolCodes; }
        }

        public ITable SelectedProtocolCodesTable
        {
            get { return _selectedProtocolCodes; }
        }

        public ISelection SelectedRequestedProcedure
        {
            get { return new Selection(_selectedProcodurePlanSummaryTableItem); }
            set
            {
                ProtocolEditorProcedurePlanSummaryTableItem item = (ProtocolEditorProcedurePlanSummaryTableItem)value.Item;
                if(item != null)
                {
                    if( item != _selectedProcodurePlanSummaryTableItem)
                    {
                        OnProcedureSelectionChanged(item);
                    }
                }
                else
                {
                    SavePreviouslySelectedItemsProtocolCodes();
                    _selectedProcodurePlanSummaryTableItem = null;
                    ResetDocument();
                }
            }
        }

        public event EventHandler SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        public void Accept()
        {
            try
            {
                Platform.GetService<IProtocollingWorkflowService>(
                    delegate(IProtocollingWorkflowService service)
                        {
                            SaveProtocols(service);
                            service.AcceptOrderProtocol(new AcceptOrderProtocolRequest(_orderRef));
                        });

                EventsHelper.Fire(_protocolAccepted, this, EventArgs.Empty);
            }
            catch(Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public bool AcceptEnabled
        {
            get { return _acceptEnabled; }
        }

        public event EventHandler ProtocolAccepted
        {
            add { _protocolAccepted += value; }
            remove { _protocolAccepted += value; }
        }

        public void Reject()
        {
            try
            {
                EnumValueInfo reason = GetReason("Reject Reason");

                if (reason == null)
                    return;

                Platform.GetService<IProtocollingWorkflowService>(
                    delegate(IProtocollingWorkflowService service)
                    {
                        SaveProtocols(service);
                        service.RejectOrderProtocol(new RejectOrderProtocolRequest(_orderRef, reason));
                    });

                EventsHelper.Fire(_protocolRejected, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private EnumValueInfo GetReason(string title)
        {
            ProtocolReasonComponent component = new ProtocolReasonComponent();

            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, component, title);

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                return component.Reason;
            }

            return null;
        }

        public bool RejectEnabled
        {
            get { return _rejectEnabled; }
        }

        public event EventHandler ProtocolRejected
        {
            add { _protocolRejected += value; }
            remove { _protocolRejected += value; }
        }

        public void Suspend()
        {
            try
            {
                EnumValueInfo reason = GetReason("Suspend Reason");

                if(reason == null)
                    return;

                Platform.GetService<IProtocollingWorkflowService>(
                    delegate(IProtocollingWorkflowService service)
                    {
                        SaveProtocols(service);
                        service.SuspendOrderProtocol(new SuspendOrderProtocolRequest(_orderRef, reason));
                    });

                EventsHelper.Fire(_protocolSuspended, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public bool SuspendEnabled
        {
            get { return _suspendEnabled;  }
        }

        public event EventHandler ProtocolSuspended
        {
            add { _protocolSuspended += value; }
            remove { _protocolSuspended += value; }
        }

        public void Save()
        {
            try
            {
                Platform.GetService<IProtocollingWorkflowService>(
                    delegate(IProtocollingWorkflowService service)
                    {
                        SaveProtocols(service);
                    });

                EventsHelper.Fire(_protocolSaved, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public bool SaveEnabled
        {
            get { return _saveEnabled; }
        }

        public event EventHandler ProtocolSaved
        {
            add { _protocolSaved += value; }
            remove { _protocolSaved += value; }
        }

        public void Close()
        {
            EventsHelper.Fire(_protocolCancelled, this, EventArgs.Empty);
        }

        public event EventHandler ProtocolCancelled
        {
            add { _protocolCancelled += value; }
            remove { _protocolCancelled += value; }
        }

        public void AddNote()
        {
            
        }

        #endregion

        private void InitializeProcedurePlanSummary()
        {
            _procedurePlanSummaryTable = new ProtocolEditorProcedurePlanSummaryTable();

            Platform.GetService<IProtocollingWorkflowService>(
                delegate(IProtocollingWorkflowService service)
                {
                    GetProcedurePlanForProtocollingWorklistItemRequest procedurePlanRequest = new GetProcedurePlanForProtocollingWorklistItemRequest(_worklistItem.ProcedureStepRef);
                    GetProcedurePlanForProtocollingWorklistItemResponse procedurePlanResponse = service.GetProcedurePlanForProtocollingWorklistItem(procedurePlanRequest);

                    if (procedurePlanResponse.ProcedurePlanSummary != null)
                    {
                        _orderRef = procedurePlanResponse.ProcedurePlanSummary.OrderRef;

                        _procedurePlanSummaryTable.Items.Clear();

                        foreach (RequestedProcedureDetail rp in procedurePlanResponse.ProcedurePlanSummary.RequestedProcedures)
                        {
                            if (rp.ProtocolProcedureStepDetail != null)
                            {
                                GetProtocolRequest protocolRequest = new GetProtocolRequest(rp.ProtocolProcedureStepDetail.ProtocolRef);
                                GetProtocolResponse protocolResponse = service.GetProtocol(protocolRequest);

                                _procedurePlanSummaryTable.Items.Add(new ProtocolEditorProcedurePlanSummaryTableItem(rp, rp.ProtocolProcedureStepDetail, protocolResponse.ProtocolDetail));
                            }
                        }
                        _procedurePlanSummaryTable.Sort();

                        GetProtocolOperationEnablementResponse response = service.GetProtocolOperationEnablement(new GetProtocolOperationEnablementRequest(_orderRef));

                        _acceptEnabled = response.AcceptEnabled;
                        _suspendEnabled = response.SuspendEnabled;
                        _rejectEnabled = response.RejectEnabled;
                        _saveEnabled = response.SaveEnabled;
                    }
                });
        }

        private void OnProcedureSelectionChanged(ProtocolEditorProcedurePlanSummaryTableItem item)
        {
            ResetDocument();

            SavePreviouslySelectedItemsProtocolCodes();

            //Refresh protocol
            Platform.GetService<IProtocollingWorkflowService>(
                delegate(IProtocollingWorkflowService service)
                {
                    // Load available protocol groups
                    ListProtocolGroupsForProcedureRequest request = new ListProtocolGroupsForProcedureRequest(item.RequestedProcedureDetail.RequestedProcedureRef);
                    ListProtocolGroupsForProcedureResponse response = service.ListProtocolGroupsForProcedure(request);

                    _protocolGroupChoices = response.ProtocolGroups;
                    _protocolGroup = response.InitialProtocolGroup;

                    RefreshAvailableProtocolCodes(item.ProtocolDetail.Codes, service);

                    // fill out selected item codes
                    _selectedProtocolCodes.Items.Clear();
                    _selectedProtocolCodes.Items.AddRange(item.ProtocolDetail.Codes);
                });

            _selectedProcodurePlanSummaryTableItem = item;

            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
            NotifyPropertyChanged("ProtocolGroupChoices");
        }

        private void OnProtocolGroupSelectionChanged()
        {
            Platform.GetService<IProtocollingWorkflowService>(
                delegate(IProtocollingWorkflowService service)
                    {
                        RefreshAvailableProtocolCodes(_selectedProcodurePlanSummaryTableItem.ProtocolDetail.Codes, service);
                    });
        }

        private void RefreshAvailableProtocolCodes(IList<ProtocolCodeDetail> existingSelectedCodes, IProtocollingWorkflowService service)
        {
            GetProtocolGroupDetailRequest protocolCodesDetailRequest = new GetProtocolGroupDetailRequest(_protocolGroup);
            GetProtocolGroupDetailResponse protocolCodesDetailResponse = service.GetProtocolGroupDetail(protocolCodesDetailRequest);

            _availableProtocolCodes.Items.Clear();
            _availableProtocolCodes.Items.AddRange(protocolCodesDetailResponse.ProtocolGroup.Codes);

            foreach (ProtocolCodeDetail code in existingSelectedCodes)
            {
                _availableProtocolCodes.Items.Remove(code);
            }
        }

        private void ResetDocument()
        {
            _availableProtocolCodes.Items.Clear();
            _selectedProtocolCodes.Items.Clear();
        }

        private void SaveProtocols(IProtocollingWorkflowService service)
        {
            SavePreviouslySelectedItemsProtocolCodes();

            foreach (ProtocolEditorProcedurePlanSummaryTableItem item in _procedurePlanSummaryTable.Items)
            {
                service.SaveProtocol(new SaveProtocolRequest(item.ProtocolStepDetail.ProtocolRef, item.ProtocolDetail));
            }
        }

        private void SavePreviouslySelectedItemsProtocolCodes()
        {
            //Save codes to existing 
            if (_selectedProcodurePlanSummaryTableItem != null)
            {
                _selectedProcodurePlanSummaryTableItem.ProtocolDetail.Codes.Clear();
                _selectedProcodurePlanSummaryTableItem.ProtocolDetail.Codes.AddRange(_selectedProtocolCodes.Items);
            }
        }
    }

    public class ProtocolCodeTable : Table<ProtocolCodeDetail>
    {
        public ProtocolCodeTable()
        {
            this.Columns.Add(new TableColumn<ProtocolCodeDetail, string>("Code",
                                                                         delegate(ProtocolCodeDetail detail)
                                                                             { return detail.Name; },
                                                                         0.5f));
        }
    }
}
