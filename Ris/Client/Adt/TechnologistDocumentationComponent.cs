using System;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="TechnologistDocumentationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class TechnologistDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// TechnologistDocumentationComponent class
    /// </summary>
    [AssociateView(typeof(TechnologistDocumentationComponentViewExtensionPoint))]
    public class TechnologistDocumentationComponent : ApplicationComponent
    {
        [ComVisible(true)]
        public class ScriptCallback
        {
            private readonly TechnologistDocumentationComponent _component;

            public ScriptCallback(TechnologistDocumentationComponent component)
            {
                this._component = component;
            }

            public string GetData(string tag)
            {
                return _component.GetData(tag);
            }

            public string FormatDate(string isoDateString)
            {
                DateTime? dt = JsmlSerializer.ParseIsoDateTime(isoDateString);
                return dt == null ? "" : Format.Date(dt);
            }

            public string FormatTime(string isoDateString)
            {
                DateTime? dt = JsmlSerializer.ParseIsoDateTime(isoDateString);
                return dt == null ? "" : Format.Time(dt);
            }

            public string FormatDateTime(string isoDateString)
            {
                DateTime? dt = JsmlSerializer.ParseIsoDateTime(isoDateString);
                return dt == null ? "" : Format.DateTime(dt);
            }

            public string FormatHealthcard(string jsml)
            {
                HealthcardDetail detail = JsmlSerializer.Deserialize<HealthcardDetail>(jsml);
                return detail == null ? "" : HealthcardFormat.Format(detail);
            }

            public string FormatMrn(string jsml)
            {
                MrnDetail detail = JsmlSerializer.Deserialize<MrnDetail>(jsml);
                return detail == null ? "" : MrnFormat.Format(detail);
            }

            public string FormatPersonName(string jsml)
            {
                PersonNameDetail detail = JsmlSerializer.Deserialize<PersonNameDetail>(jsml);
                return detail == null ? "" : PersonNameFormat.Format(detail);
            }

            public JsmlServiceProxy GetServiceProxy(string serviceContractName)
            {
                return new JsmlServiceProxy(serviceContractName);
            }

            public string GetWorklistItem()
            {
                return JsmlSerializer.Serialize(_component.GetWorklistItem(), "worklistItem");
            }
        }

        #region Private Members

        private readonly ScriptCallback _scriptCallback;
        private readonly ModalityWorklistItem _worklistItem;

        private Tree<RequestedProcedureDetail> _procedurePlanTree;
        private SimpleActionModel _procedurePlanActionHandler;
        private readonly string _startModalityProcedureStepKey = "StartModalityProcedureStep";
        private readonly string _discontinueRequestedProcedureOrModalityProcedureStepKey = "DiscontinueRequestedProcedureOrModalityProcedureStepKey";

        private TechnologistDocumentationMppsSummaryTable _mppsTable;
        private SimpleActionModel _mppsActionHandler;
        private readonly string _stopPerformedProcedureStepKey = "StopPerformedProcedureStepKey";
        private readonly string _discontinuePerformedProcedureStepKey = "DiscontinuePerformedProcedureStepKey";

        #endregion

        public TechnologistDocumentationComponent(ModalityWorklistItem item)
        {
            _scriptCallback = new ScriptCallback(this);

            _worklistItem = item;
            _mppsTable = new TechnologistDocumentationMppsSummaryTable();
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

            _procedurePlanActionHandler = new SimpleActionModel(resolver);
            _procedurePlanActionHandler.AddAction(_startModalityProcedureStepKey, "START", "Icons.StartToolSmall.png", "START", StartModalityProcedureStep);
            _procedurePlanActionHandler.AddAction(_discontinueRequestedProcedureOrModalityProcedureStepKey, "DISCONTINUE", "Icons.DeleteToolSmall.png", "START", DiscontinueRequestedProcedureOrModalityProcedureStep);
            _procedurePlanActionHandler[_startModalityProcedureStepKey].Enabled = false;
            _procedurePlanActionHandler[_discontinueRequestedProcedureOrModalityProcedureStepKey].Enabled = false;

            _mppsActionHandler = new SimpleActionModel(resolver);
            _mppsActionHandler.AddAction(_stopPerformedProcedureStepKey, "STOP", "Icons.CompleteToolSmall.png", "STOP", StopPerformedProcedureStep);
            _mppsActionHandler.AddAction(_discontinuePerformedProcedureStepKey, "DISCONTINUE", "Icons.DeleteToolSmall.png", "START", DiscontinuePerformedProcedureStep);
            _mppsActionHandler[_stopPerformedProcedureStepKey].Enabled = false;
            _mppsActionHandler[_discontinuePerformedProcedureStepKey].Enabled = false;

            Platform.GetService<ITechnologistDocumentationService>(
                delegate(ITechnologistDocumentationService service)
                {
                    GetProcedurePlanForWorklistItemRequest procedurePlanRequest = new GetProcedurePlanForWorklistItemRequest(_worklistItem.ProcedureStepRef);
                    GetProcedurePlanForWorklistItemResponse procedurePlanResponse = service.GetProcedurePlanForWorklistItem(procedurePlanRequest);

                    _procedurePlanTree = new Tree<RequestedProcedureDetail>(
                        new TreeItemBinding<RequestedProcedureDetail>(
                            delegate(RequestedProcedureDetail rp) { return rp.Name; },
                            delegate(RequestedProcedureDetail rp)
                                {
                                    TreeItemBinding<ModalityProcedureStepDetail> binding = new TreeItemBinding<ModalityProcedureStepDetail>(
                                        delegate(ModalityProcedureStepDetail mps) { return mps.Name; });
                                    binding.CanHaveSubTreeHandler = delegate { return false; };
                                    binding.IconSetProvider = delegate { return null; };
                                    return new Tree<ModalityProcedureStepDetail>(binding, rp.ModalityProcedureSteps);
                                }), procedurePlanResponse.RequestedProcedures);

                    ListPerformedProcedureStepsRequest mppsRequest = new ListPerformedProcedureStepsRequest(_worklistItem.ProcedureStepRef);
                    ListPerformedProcedureStepsResponse mppsResponse = service.ListPerformedProcedureSteps(mppsRequest);

                    _mppsTable.Items.AddRange(mppsResponse.PerformedProcedureSteps);
                });

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        #region Scripting Callbacks

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public string GetData(string tag)
        {
            return _worklistItem.AccessionNumber;
        }

        public object GetWorklistItem()
        {
            return _worklistItem;
        }

        #endregion

        #region Presentation Layer Methods

        public string OrderSummaryUrl
        {
            get { return TechnologistDocumentationComponentSettings.Default.OrderSummaryUrl; }
        }

        public ITree ProcedurePlanTree
        {
            get { return _procedurePlanTree; }
        }

        public ActionModelNode ProcedurePlanTreeActionModel
        {
            get { return _procedurePlanActionHandler; }
        }

        public ITable MppsTable
        {
            get { return _mppsTable; }
        }

        public ActionModelNode MppsTableActionModel
        {
            get { return _mppsActionHandler; }
        }

        #endregion

        #region Action Handler Methods

        private void StartModalityProcedureStep()
        {
        }

        private void DiscontinueRequestedProcedureOrModalityProcedureStep()
        {
        }

        private void StopPerformedProcedureStep()
        {
        }

        private void DiscontinuePerformedProcedureStep()
        {
        }

        #endregion
    }
}
