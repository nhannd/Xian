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
        #region Application Component Host class

        class ChildComponentHost : ApplicationComponentHost
        {
            private TechnologistDocumentationComponent _owner;

            public ChildComponentHost(TechnologistDocumentationComponent owner, IApplicationComponent hostedComponent)
                :base(hostedComponent)
            {
                _owner = owner;
            }

            public override DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

        }

        #endregion

        #region Order Summary Component class

        class OrderSummaryComponent : DHtmlComponent
        {
            private TechnologistDocumentationComponent _owner;

            public OrderSummaryComponent(TechnologistDocumentationComponent owner)
            {
                _owner = owner;
            }

            public override void Start()
            {
                SetUrl(TechnologistDocumentationComponentSettings.Default.OrderSummaryUrl);
                base.Start();
            }

            protected override object GetWorklistItem()
            {
                return _owner._worklistItem;
            }
        }

        #endregion


        #region Private Members

        private readonly ModalityWorklistItem _worklistItem;
        private EntityRef _orderRef;

        private Tree<RequestedProcedureDetail> _procedurePlanTree;
        private readonly List<Checkable<ModalityProcedureStepDetail>> _allCheckableModalityProcedureSteps;
        private SimpleActionModel _procedurePlanActionHandler;
        private ClickAction _startAction;
        private ClickAction _discontinueAction;

        private ChildComponentHost _orderSummaryComponentHost;
        private ChildComponentHost _documentationHost;
        private TabComponentContainer _documentationTabContainer;

        private DHtmlComponent _preExamComponent;
        private DHtmlComponent _postExamComponent;
        private PerformedProcedureComponent _ppsComponent;
        


        private event EventHandler _procedurePlanTreeChanged;

        #endregion

        public TechnologistDocumentationComponent(ModalityWorklistItem item)
        {
            _worklistItem = item;
            _allCheckableModalityProcedureSteps = new List<Checkable<ModalityProcedureStepDetail>>();
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            ProcedurePlanSummary procedurePlanSummary = null;

            Platform.GetService<ITechnologistDocumentationService>(
                delegate(ITechnologistDocumentationService service)
                {
                    GetProcedurePlanForWorklistItemRequest procedurePlanRequest = new GetProcedurePlanForWorklistItemRequest(_worklistItem.ProcedureStepRef);
                    GetProcedurePlanForWorklistItemResponse procedurePlanResponse = service.GetProcedurePlanForWorklistItem(procedurePlanRequest);
                    procedurePlanSummary = procedurePlanResponse.ProcedurePlanSummary;
                });

            RefreshProcedurePlanTree(procedurePlanSummary);

            _orderSummaryComponentHost = new ChildComponentHost(this, new OrderSummaryComponent(this));
            _orderSummaryComponentHost.StartComponent();

            _documentationTabContainer = new TabComponentContainer();
            _preExamComponent = new ExamDetailsComponent(
                TechnologistDocumentationComponentSettings.Default.PreExamDetailsPageUrlSelectorScript,
                procedurePlanSummary);
            _documentationTabContainer.Pages.Add(new TabPage("Pre-exam", _preExamComponent));

            _ppsComponent = new PerformedProcedureComponent(_orderRef);
            _documentationTabContainer.Pages.Add(new TabPage("Exam", _ppsComponent));

            _postExamComponent = new ExamDetailsComponent(
                TechnologistDocumentationComponentSettings.Default.PostExamDetailsPageUrlSelectorScript,
                procedurePlanSummary);
            _documentationTabContainer.Pages.Add(new TabPage("Post-exam", _postExamComponent));

            _documentationHost = new ChildComponentHost(this, _documentationTabContainer);
            _documentationHost.StartComponent();

            ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
            _procedurePlanActionHandler = new SimpleActionModel(resolver);
            _startAction = _procedurePlanActionHandler.AddAction("start", "START", "Icons.StartToolSmall.png", "START", StartModalityProcedureStep);
            _discontinueAction = _procedurePlanActionHandler.AddAction("discontinue", "DISCONTINUE", "Icons.DeleteToolSmall.png", "START", DiscontinueRequestedProcedureOrModalityProcedureStep);

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        #region Presentation Layer Methods

        public ApplicationComponentHost OrderSummaryComponentHost
        {
            get { return _orderSummaryComponentHost; }
        }

        public ApplicationComponentHost DocumentationHost
        {
            get { return _documentationHost; }
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

                            RefreshProcedurePlanTree(response.ProcedurePlanSummary);

                            _ppsComponent.AddPerformedProcedureStep(response.StartedMpps);
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
            //                _orderRef = response.OrderRef;
            //            });
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionHandler.Report(e, this.Host.DesktopWindow);
            //}
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

        private void RefreshProcedurePlanTree(ProcedurePlanSummary procedurePlanSummary)
        {
            _orderRef = procedurePlanSummary.OrderRef;

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

            _procedurePlanTree = new Tree<RequestedProcedureDetail>(rpBinding, procedurePlanSummary.RequestedProcedures);

            EventsHelper.Fire(_procedurePlanTreeChanged, this, EventArgs.Empty);
        }

        #endregion
    }
}
