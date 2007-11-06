using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
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

        private ProtocolEditorProcedurePlanSummaryTable _procedurePlanSummaryTable;
        private ProtocolEditorProcedurePlanSummaryTableItem _selectedRequestedProcedure;

        private ProtocolDetail _selectedProtocolDetail;
        private ProtocolCodeTable _availableProtocolCodes;
        private ProtocolCodeTable _selectedProtocolCodes;

        private event EventHandler _selectionChanged;

        private ChildComponentHost _orderSummaryComponentHost;
        private ChildComponentHost _protocolNoteSummaryComponentHost;

        private bool _acceptEnabled;
        private bool _rejectEnabled;
        private bool _suspendEnabled;
        private bool _saveEnabled;

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

            InitializeProcedurePlanSummary();
            InitializeActionEnablement();

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
            get { return new Selection(_selectedRequestedProcedure); }
            set
            {
                ProtocolEditorProcedurePlanSummaryTableItem item = (ProtocolEditorProcedurePlanSummaryTableItem)value.Item;
                if(item != null)
                {
                    if( item != _selectedRequestedProcedure)
                    {
                        _selectedRequestedProcedure = item;
                        OnSelectionChanged();
                    }
                }
                else
                {
                    _selectedRequestedProcedure = null;
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
            throw new NotImplementedException();
        }

        public bool AcceptEnabled
        {
            get { return _acceptEnabled; }
        }

        public void Reject()
        {
            throw new NotImplementedException();
        }

        public bool RejectEnabled
        {
            get { return _rejectEnabled; }
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public bool SuspendEnabled
        {
            get { return _suspendEnabled;  }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public bool SaveEnabled
        {
            get { return _saveEnabled; }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void AddNote()
        {
            throw new NotImplementedException();
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
                        _procedurePlanSummaryTable.Items.Clear();
                        foreach (RequestedProcedureDetail rp in procedurePlanResponse.ProcedurePlanSummary.RequestedProcedures)
                        {
                            if (rp.ProtocolProcedureStepDetail != null)
                            {
                                _procedurePlanSummaryTable.Items.Add(new ProtocolEditorProcedurePlanSummaryTableItem(rp, rp.ProtocolProcedureStepDetail));
                            }
                        }
                        _procedurePlanSummaryTable.Sort();
                    }
                });
        }

        public void InitializeActionEnablement()
        {
            _acceptEnabled = false;
            _suspendEnabled = false;
            _rejectEnabled = false;
            _saveEnabled = false;
        }

        private void OnSelectionChanged()
        {
            ResetDocument();

            //Refresh protocol
            Platform.GetService<IProtocollingWorkflowService>(
                delegate(IProtocollingWorkflowService service)
                {
                    ListProtocolCodesRequest protocolCodesRequest = new ListProtocolCodesRequest();
                    ListProtocolCodesResponse protocolCodesResponse = service.ListProtocolCodes(protocolCodesRequest);

                    _availableProtocolCodes.Items.AddRange(protocolCodesResponse.Codes);

                    GetProcedureProtocolRequest procedureProtocolRequest = new GetProcedureProtocolRequest(_selectedRequestedProcedure.RequestedProcedureDetail.RequestedProcedureRef);
                    GetProcedureProtocolResponse procedureProtocolResponse = service.GetProcedureProtocol(procedureProtocolRequest);

                    _selectedProtocolCodes.Items.AddRange(procedureProtocolResponse.ProtocolDetail.Codes);
                    _selectedProtocolDetail = procedureProtocolResponse.ProtocolDetail;

                    _acceptEnabled = true;
                    _rejectEnabled = true;
                    _suspendEnabled = true;
                    _saveEnabled = true;
                });

            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }

        private void ResetDocument()
        {
            InitializeActionEnablement();
            _availableProtocolCodes.Items.Clear();
            _selectedProtocolCodes.Items.Clear();
        }

        //private void InitializeProcedurePlanSummaryActionHandlers()
        //{
        //    _procedurePlanActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
        //    _startAction = _procedurePlanActionHandler.AddAction("start", SR.TitleStartMps, "Icons.StartToolSmall.png", SR.TitleStartMps, StartModalityProcedureSteps);
        //    _discontinueAction = _procedurePlanActionHandler.AddAction("discontinue", SR.TitleDiscontinueMps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMps, DiscontinueModalityProcedureSteps);
        //    UpdateActionEnablement();
        //}
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
