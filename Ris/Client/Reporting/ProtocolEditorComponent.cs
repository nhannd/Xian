using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
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

        private ReportingWorklistItem _worklistItem;

        private ProcedurePlanSummary _procedurePlan;
        private ProcedurePlanSummaryTable _procedurePlanSummaryTable;
        private event EventHandler _procedurePlanChanged;

        private ChildComponentHost _orderSummaryComponentHost;
        private ChildComponentHost _protocolNoteSummaryComponentHost;

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

        public ITable ProcedurePlanSummaryTable
        {
            get { return _procedurePlanSummaryTable; }
        }

        public event EventHandler ProcedurePlanChanged
        {
            add { _procedurePlanChanged += value; }
            remove { _procedurePlanChanged -= value; }
        }

        #endregion

        private void InitializeProcedurePlanSummary()
        {
            _procedurePlanSummaryTable = new ProcedurePlanSummaryTable();
            //_procedurePlanSummaryTable.CheckedRowsChanged += delegate(object sender, EventArgs args) { UpdateActionEnablement(); };

            Platform.GetService<ITechnologistDocumentationService>(
                delegate(ITechnologistDocumentationService service)
                {
                    GetProcedurePlanForWorklistItemRequest procedurePlanRequest = new GetProcedurePlanForWorklistItemRequest(_worklistItem.ProcedureStepRef);
                    GetProcedurePlanForWorklistItemResponse procedurePlanResponse = service.GetProcedurePlanForWorklistItem(procedurePlanRequest);
                    _procedurePlan = procedurePlanResponse.ProcedurePlanSummary;
                    //_orderExtendedProperties = procedurePlanResponse.OrderExtendedProperties;
                });

            RefreshProcedurePlanSummary(_procedurePlan);

            //InitializeProcedurePlanSummaryActionHandlers();
        }

        //private void InitializeProcedurePlanSummaryActionHandlers()
        //{
        //    _procedurePlanActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
        //    _startAction = _procedurePlanActionHandler.AddAction("start", SR.TitleStartMps, "Icons.StartToolSmall.png", SR.TitleStartMps, StartModalityProcedureSteps);
        //    _discontinueAction = _procedurePlanActionHandler.AddAction("discontinue", SR.TitleDiscontinueMps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMps, DiscontinueModalityProcedureSteps);
        //    UpdateActionEnablement();
        //}

        private void RefreshProcedurePlanSummary(ProcedurePlanSummary procedurePlanSummary)
        {
            _procedurePlan = procedurePlanSummary;

            _procedurePlanSummaryTable.Items.Clear();
            foreach (RequestedProcedureDetail rp in procedurePlanSummary.RequestedProcedures)
            {
                foreach (ModalityProcedureStepDetail mps in rp.ModalityProcedureSteps)
                {
                    _procedurePlanSummaryTable.Items.Add(
                        new Checkable<ProcedurePlanSummaryTableItem>(
                            new ProcedurePlanSummaryTableItem(rp, mps)));
                }
            }
            _procedurePlanSummaryTable.Sort();

            EventsHelper.Fire(_procedurePlanChanged, this, EventArgs.Empty);
        }
    }
}
