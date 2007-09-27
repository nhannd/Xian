using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Enterprise.Common;
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

            //public string GetData(string tag)
            //{
            //    return _component.GetData(tag);
            //}

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

        public class Checkable<TItem>
        {
            private bool _isChecked;
            private TItem _item;

            public Checkable(TItem item, bool isChecked)
            {
                _isChecked = isChecked;
                _item = item;
            }

            public Checkable(TItem item)
                : this(item, false)
            {
            }

            public TItem Item
            {
                get { return _item; }
                set { _item = value; }
            }

            public bool IsChecked
            {
                get { return _isChecked; }
                set { _isChecked = value; }
            }
        }

        #region Private Members

        private readonly ScriptCallback _scriptCallback;
        private readonly ModalityWorklistItem _worklistItem;

        private readonly List<Checkable<ModalityProcedureStepDetail>> _allCheckableModalityProcedureSteps;

        private Tree<RequestedProcedureDetail> _procedurePlanTree;
        private SimpleActionModel _procedurePlanActionHandler;
        private readonly string _startModalityProcedureStepKey = "StartModalityProcedureStep";
        private readonly string _discontinueRequestedProcedureOrModalityProcedureStepKey = "DiscontinueRequestedProcedureOrModalityProcedureStepKey";

        private readonly TechnologistDocumentationMppsSummaryTable _mppsTable;
        private ModalityPerformedProcedureStepSummary _selectedMpps;
        private SimpleActionModel _mppsActionHandler;
        private readonly string _stopPerformedProcedureStepKey = "StopPerformedProcedureStepKey";
        private readonly string _discontinuePerformedProcedureStepKey = "DiscontinuePerformedProcedureStepKey";

        private EntityRef _orderRef;

        private event EventHandler _procedurePlanTreeChanged;

        #endregion

        public TechnologistDocumentationComponent(ModalityWorklistItem item)
        {
            _scriptCallback = new ScriptCallback(this);

            _worklistItem = item;
            _mppsTable = new TechnologistDocumentationMppsSummaryTable();
            _allCheckableModalityProcedureSteps = new List<Checkable<ModalityProcedureStepDetail>>();
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

            _procedurePlanActionHandler = new SimpleActionModel(resolver);
            _procedurePlanActionHandler.AddAction(_startModalityProcedureStepKey, "START", "Icons.StartToolSmall.png", "START", StartModalityProcedureStep);
            _procedurePlanActionHandler.AddAction(_discontinueRequestedProcedureOrModalityProcedureStepKey, "DISCONTINUE", "Icons.DeleteToolSmall.png", "START", DiscontinueRequestedProcedureOrModalityProcedureStep);
            _procedurePlanActionHandler[_startModalityProcedureStepKey].Enabled = true;
            _procedurePlanActionHandler[_discontinueRequestedProcedureOrModalityProcedureStepKey].Enabled = true;

            _mppsActionHandler = new SimpleActionModel(resolver);
            _mppsActionHandler.AddAction(_stopPerformedProcedureStepKey, "STOP", "Icons.CompleteToolSmall.png", "STOP", StopPerformedProcedureStep);
            _mppsActionHandler.AddAction(_discontinuePerformedProcedureStepKey, "DISCONTINUE", "Icons.DeleteToolSmall.png", "START", DiscontinuePerformedProcedureStep);
            _mppsActionHandler[_stopPerformedProcedureStepKey].Enabled = true;
            _mppsActionHandler[_discontinuePerformedProcedureStepKey].Enabled = true;

            Platform.GetService<ITechnologistDocumentationService>(
                delegate(ITechnologistDocumentationService service)
                {
                    GetProcedurePlanForWorklistItemRequest procedurePlanRequest = new GetProcedurePlanForWorklistItemRequest(_worklistItem.ProcedureStepRef);
                    GetProcedurePlanForWorklistItemResponse procedurePlanResponse = service.GetProcedurePlanForWorklistItem(procedurePlanRequest);

                    _orderRef = procedurePlanResponse.OrderRef;
                    RefreshProcedurePlanTree(procedurePlanResponse.RequestedProcedures);

                    ListPerformedProcedureStepsRequest mppsRequest = new ListPerformedProcedureStepsRequest(procedurePlanResponse.OrderRef);
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

        //public string GetData(string tag)
        //{
        //    return _worklistItem.AccessionNumber;
        //}

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

        public event EventHandler ProcedurePlanTreeChanged
        {
            add { _procedurePlanTreeChanged += value; }
            remove { _procedurePlanTreeChanged -= value; }
        }

        public ActionModelNode ProcedurePlanTreeActionModel
        {
            get { return _procedurePlanActionHandler; }
        }

        public ITable MppsTable
        {
            get { return _mppsTable; }
        }

        public ISelection SelectedMpps
        {
            get { return _selectedMpps == null ? Selection.Empty : new Selection(_selectedMpps); }
            set
            {
                _selectedMpps = (ModalityPerformedProcedureStepSummary) value.Item;
                SelectedMppsChanged();
            }
        }

        public ActionModelNode MppsTableActionModel
        {
            get { return _mppsActionHandler; }
        }

        public void OnComplete()
        {
            try
            {
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
                    {
                        CompleteModalityProcedureStepsRequest request = new CompleteModalityProcedureStepsRequest(_orderRef);
                        CompleteModalityProcedureStepsResponse response = service.CompleteModalityProcedureSteps(request);

                        RefreshProcedurePlanTree(response.RequestedProcedures);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

        }

        #endregion

        #region Action Handler Methods

        private void StartModalityProcedureStep()
        {
            try
            {
                List<ModalityProcedureStepDetail> checkedMps = GetCheckedMps();

                if (checkedMps.Count > 0)
                {
                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            StartModalityProcedureStepRequest request = new StartModalityProcedureStepRequest(checkedMps);
                            StartModalityProcedureStepResponse response = service.StartModalityProcedureStep(request);

                            RefreshProcedurePlanTree(response.RequestedProcedures);

                            _mppsTable.Items.Add(response.ModalityPerformedProcedureStep);
                            _mppsTable.Sort();
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void DiscontinueRequestedProcedureOrModalityProcedureStep()
        {
            //try
            //{
            //    List<ModalityProcedureStepDetail> checkedMps = GetCheckedMps();
            //    List<RequestedProcedureDetail> checkedRps = GetCheckedRps();

            //    if (checkedMps.Count > 0)
            //    {
            //        Platform.GetService<ITechnologistDocumentationService>(
            //            delegate(ITechnologistDocumentationService service)
            //            {
            //                DiscontinueRequestedProcedureOrModalityProcedureStepRequest request = new DiscontinueRequestedProcedureOrModalityProcedureStepRequest();
            //                request.RequestedProcedures = checkedRps;
            //                request.ModalityProcedureSteps = checkedMps;
            //                DiscontinueRequestedProcedureOrModalityProcedureStepResponse response = service.DiscontinueRequestedProcedureOrModalityProcedureStep(request);

            //                RefreshProcedurePlanTree(response.RequestedProcedures);
            //            });
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionHandler.Report(e, this.Host.DesktopWindow);
            //}
        }

        private void StopPerformedProcedureStep()
        {
            try
            {
                ModalityPerformedProcedureStepSummary selectedMpps = _selectedMpps;

                if (selectedMpps != null)
                {
                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            StopModalityPerformedProcedureStepRequest request = new StopModalityPerformedProcedureStepRequest(selectedMpps.ModalityPerformendProcedureStepRef);
                            StopModalityPerformedProcedureStepResponse response = service.StopModalityPerformedProcedureStep(request);

                            RefreshProcedurePlanTree(response.RequestedProcedures);

                            _mppsTable.Items.Replace(
                                delegate(ModalityPerformedProcedureStepSummary mppsSummary)
                                {
                                    return mppsSummary.ModalityPerformendProcedureStepRef == selectedMpps.ModalityPerformendProcedureStepRef;
                                }, 
                                response.StoppedMpps);
                            _mppsTable.Sort();
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void DiscontinuePerformedProcedureStep()
        {
            try
            {
                ModalityPerformedProcedureStepSummary selectedMpps = _selectedMpps;

                if (selectedMpps != null)
                {
                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            DiscontinueModalityPerformedProcedureStepRequest request = new DiscontinueModalityPerformedProcedureStepRequest( selectedMpps.ModalityPerformendProcedureStepRef);
                            DiscontinueModalityPerformedProcedureStepResponse response = service.DiscontinueModalityPerformedProcedureStep(request);

                            RefreshProcedurePlanTree(response.RequestedProcedures);

                            _mppsTable.Items.Replace(
                                delegate(ModalityPerformedProcedureStepSummary mppsSummary)
                                {
                                    return mppsSummary.ModalityPerformendProcedureStepRef == selectedMpps.ModalityPerformendProcedureStepRef;
                                },
                                response.DiscontinuedMpps);
                            _mppsTable.Sort();
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        #region Private methods

        private List<ModalityProcedureStepDetail> GetCheckedMps()
        {
            return CollectionUtils.Map<Checkable<ModalityProcedureStepDetail>, ModalityProcedureStepDetail>(
                CollectionUtils.Select<Checkable<ModalityProcedureStepDetail>>(
                    _allCheckableModalityProcedureSteps,
                    delegate(Checkable<ModalityProcedureStepDetail> checkable) { return checkable.IsChecked; }),
                delegate(Checkable<ModalityProcedureStepDetail> checkable) { return checkable.Item; });
        }

        private List<RequestedProcedureDetail> GetCheckedRps()
        {
            return new List<RequestedProcedureDetail>();
        }

        private void RefreshProcedurePlanTree(List<RequestedProcedureDetail> procedures)
        {
            _allCheckableModalityProcedureSteps.Clear();

            TreeItemBinding<RequestedProcedureDetail> rpBinding = new TreeItemBinding<RequestedProcedureDetail>(delegate(RequestedProcedureDetail rp) { return rp.Name + " - " + rp.Status.Value; });
            rpBinding.SubTreeProvider = delegate(RequestedProcedureDetail rp)
                    {
                        TreeItemBinding<Checkable<ModalityProcedureStepDetail>> binding = new TreeItemBinding<Checkable<ModalityProcedureStepDetail>>(
                            delegate(Checkable<ModalityProcedureStepDetail> checkable) { return checkable.Item.Name + " - " + checkable.Item.Status.Value; });
                        binding.CanHaveSubTreeHandler = delegate { return false; };
                        binding.IconSetProvider = delegate { return null; };
                        binding.IsCheckedGetter = delegate(Checkable<ModalityProcedureStepDetail> mps) { return mps.IsChecked; };
                        binding.IsCheckedSetter = delegate(Checkable<ModalityProcedureStepDetail> mps, bool isChecked) { mps.IsChecked = isChecked; };

                        List<Checkable<ModalityProcedureStepDetail>> checkableMpsList =
                            CollectionUtils.Map<ModalityProcedureStepDetail, Checkable<ModalityProcedureStepDetail>>(
                                rp.ModalityProcedureSteps,
                                delegate(ModalityProcedureStepDetail mps)
                                    {
                                        return new Checkable<ModalityProcedureStepDetail>(mps);
                                    });

                        _allCheckableModalityProcedureSteps.AddRange(checkableMpsList);

                        return new Tree<Checkable<ModalityProcedureStepDetail>>(binding, checkableMpsList);
                    };

            _procedurePlanTree = new Tree<RequestedProcedureDetail>(rpBinding, procedures);

            EventsHelper.Fire(_procedurePlanTreeChanged, this, EventArgs.Empty);
        }

        private void SelectedMppsChanged()
        {
            // trigger url change here?
        }

        #endregion
    }
}
