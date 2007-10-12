#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    public interface ITechnologistDocumentationModule
    {
        void Initialize(ITechnologistDocumentationContext context);
        void SaveData();
    }


    [ExtensionPoint]
    public class TechnologistDocumentationModuleExtensionPoint : ExtensionPoint<ITechnologistDocumentationModule>
    {
    }

    public interface ITechnologistDocumentationContext : IToolContext
    {
        DesktopWindow DesktopWindow { get; }
        void AddPage(IDocumentationPage page);
        void InsertPage(IDocumentationPage page, int position);
        ProcedurePlanSummary ProcedurePlan { get; }
        event EventHandler ProcedurePlanChanged;
   }


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
        #region Order Summary Component class

        class OrderSummaryComponent : DHtmlComponent
        {
            private readonly TechnologistDocumentationComponent _owner;

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

        #region TechnologistDocumentationContext class

        class TechnologistDocumentationContext : ToolContext, ITechnologistDocumentationContext
        {
            private TechnologistDocumentationComponent _owner;

            public TechnologistDocumentationContext(TechnologistDocumentationComponent owner)
            {
                _owner = owner;
            }

            #region ITechnologistDocumentationContext Members

            public DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

            public void AddPage(IDocumentationPage page)
            {
                _owner.InsertDocumentationPage(page, _owner._documentationTabContainer.Pages.Count);
            }

            public void InsertPage(IDocumentationPage page, int position)
            {
                _owner.InsertDocumentationPage(page, position);
            }

            public ProcedurePlanSummary ProcedurePlan
            {
                get { return _owner._procedurePlan; }
            }

            public event EventHandler ProcedurePlanChanged
            {
                add { _owner._procedurePlanChanged += value; }
                remove { _owner._procedurePlanChanged -= value; }
            }


            #endregion
        }

        #endregion

        #region Private Members

        private readonly ModalityWorklistItem _worklistItem;
        private EntityRef _orderRef;
        private Dictionary<string, string> _orderExtendedProperties;
        private ProcedurePlanSummary _procedurePlan;
        private event EventHandler _procedurePlanChanged;

        private Tree<RequestedProcedureDetail> _procedurePlanTree;
        private event EventHandler _procedurePlanTreeChanged;
        private readonly List<Checkable<ModalityProcedureStepDetail>> _allCheckableModalityProcedureSteps;
        private SimpleActionModel _procedurePlanActionHandler;
        private ClickAction _startAction;
        private ClickAction _discontinueAction;

        private ChildComponentHost _orderSummaryComponentHost;
        private ChildComponentHost _documentationHost;
        private TabComponentContainer _documentationTabContainer;

        private List<ITechnologistDocumentationModule> _documentationModules = new List<ITechnologistDocumentationModule>();

        private ExamDetailsComponent _preExamComponent;
        private ExamDetailsComponent _postExamComponent;
        private PerformedProcedureComponent _ppsComponent;

        #endregion

        public TechnologistDocumentationComponent(ModalityWorklistItem item)
        {
            _worklistItem = item;
            _allCheckableModalityProcedureSteps = new List<Checkable<ModalityProcedureStepDetail>>();
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            Platform.GetService<ITechnologistDocumentationService>(
                delegate(ITechnologistDocumentationService service)
                {
                    GetProcedurePlanForWorklistItemRequest procedurePlanRequest = new GetProcedurePlanForWorklistItemRequest(_worklistItem.ProcedureStepRef);
                    GetProcedurePlanForWorklistItemResponse procedurePlanResponse = service.GetProcedurePlanForWorklistItem(procedurePlanRequest);
                    _procedurePlan = procedurePlanResponse.ProcedurePlanSummary;
                    _orderExtendedProperties = procedurePlanResponse.OrderExtendedProperties;
                });

            RefreshProcedurePlanTree(_procedurePlan);

            ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
            _procedurePlanActionHandler = new SimpleActionModel(resolver);
            _startAction = _procedurePlanActionHandler.AddAction("start", "START", "Icons.StartToolSmall.png", "START", StartModalityProcedureSteps);
            _discontinueAction = _procedurePlanActionHandler.AddAction("discontinue", "DISCONTINUE", "Icons.DeleteToolSmall.png", "START", DiscontinueModalityProcedureSteps);
            UpdateActionEnablement();

            _orderSummaryComponentHost = new ChildComponentHost(this.Host, new OrderSummaryComponent(this));
            _orderSummaryComponentHost.StartComponent();

            _documentationTabContainer = new TabComponentContainer();

            _preExamComponent = new ExamDetailsComponent("Pre-exam",
                TechnologistDocumentationComponentSettings.Default.PreExamDetailsPageUrlSelectorScript,
                _procedurePlan, _orderExtendedProperties);
            InsertDocumentationPage(_preExamComponent, 0);

            _ppsComponent = new PerformedProcedureComponent("Exam", _orderRef);
            _ppsComponent.ProcedurePlanChanged += ProcedurePlanChangedEventHandler;
            InsertDocumentationPage(_ppsComponent, 1);

            _postExamComponent = new ExamDetailsComponent("Post-exam",
                TechnologistDocumentationComponentSettings.Default.PostExamDetailsPageUrlSelectorScript,
                _procedurePlan, _orderExtendedProperties);
            InsertDocumentationPage(_postExamComponent, 2);

            // create extension modules, which may add documentation pages to the tab container
            TechnologistDocumentationContext context = new TechnologistDocumentationContext(this);
            foreach (ITechnologistDocumentationModule module in (new TechnologistDocumentationModuleExtensionPoint()).CreateExtensions())
            {
                module.Initialize(context);
                _documentationModules.Add(module);
            }

            _documentationHost = new ChildComponentHost(this.Host, _documentationTabContainer);
            _documentationHost.StartComponent();


            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        #region Presentation Model Methods

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

        public void SaveData()
        {
            try
            {
                Save();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void CompleteDocumentation()
        {
            try
            {
                // validate first
                Save();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        #region Action Handler Methods

        private void StartModalityProcedureSteps()
        {
            try
            {
                List<ModalityProcedureStepDetail> checkedMps = GetCheckedMps();
                List<EntityRef> checkedMpsRefs = CollectionUtils.Map<ModalityProcedureStepDetail, EntityRef, List<EntityRef>>(checkedMps,
                    delegate(ModalityProcedureStepDetail detail)
                    {
                        return detail.ModalityProcedureStepRef;
                    });

                if (checkedMps.Count > 0)
                {
                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            StartModalityProcedureStepsRequest request = new StartModalityProcedureStepsRequest(checkedMpsRefs);
                            StartModalityProcedureStepsResponse response = service.StartModalityProcedureSteps(request);

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

        private void DiscontinueModalityProcedureSteps()
        {
            try
            {
                List<ModalityProcedureStepDetail> checkedMps = GetCheckedMps();
                List<EntityRef> checkedMpsRefs = CollectionUtils.Map<ModalityProcedureStepDetail, EntityRef, List<EntityRef>>(checkedMps,
                    delegate(ModalityProcedureStepDetail detail)
                    {
                        return detail.ModalityProcedureStepRef;
                    });

                if (checkedMps.Count > 0)
                {
                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            DiscontinueModalityProcedureStepsRequest request = new DiscontinueModalityProcedureStepsRequest(checkedMpsRefs);
                            DiscontinueModalityProcedureStepsResponse response = service.DiscontinueModalityProcedureSteps(request);

                            RefreshProcedurePlanTree(response.ProcedurePlanSummary);
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

        private void Save()
        {
            _preExamComponent.SaveData();
            _postExamComponent.SaveData();

            foreach(ITechnologistDocumentationModule module in _documentationModules)
            {
                module.SaveData();
            }

            SaveDataRequest request = new SaveDataRequest(_procedurePlan.OrderRef, _orderExtendedProperties);
            Platform.GetService<ITechnologistDocumentationService>(
                delegate(ITechnologistDocumentationService service)
                {
                    service.SaveData(request);
                });
        }

        private void InsertDocumentationPage(IDocumentationPage page, int position)
        {
            _documentationTabContainer.Pages.Insert(position, new TabPage(page.Title, page.Component));
        }

        private List<ModalityProcedureStepDetail> GetCheckedMps()
        {
            return CollectionUtils.Map<Checkable<ModalityProcedureStepDetail>, ModalityProcedureStepDetail>(
                CollectionUtils.Select<Checkable<ModalityProcedureStepDetail>>(
                    _allCheckableModalityProcedureSteps,
                    delegate(Checkable<ModalityProcedureStepDetail> checkable) { return checkable.IsChecked; }),
                delegate(Checkable<ModalityProcedureStepDetail> checkable) { return checkable.Item; });
        }

        private void UpdateActionEnablement()
        {
            IList<ModalityProcedureStepDetail> checkedMps = GetCheckedMps();
            if (checkedMps.Count == 0)
            {
                _startAction.Enabled = _discontinueAction.Enabled = false;
            }
            else
            {
                // TODO: defer enablement to server
                _startAction.Enabled = CollectionUtils.TrueForAll<ModalityProcedureStepDetail>(checkedMps,
                    delegate(ModalityProcedureStepDetail mps) { return mps.Status.Code == "SC"; });

                _discontinueAction.Enabled = CollectionUtils.TrueForAll<ModalityProcedureStepDetail>(checkedMps,
                    delegate(ModalityProcedureStepDetail mps) { return mps.Status.Code == "SC"; });
            }
        }

        private void ProcedurePlanChangedEventHandler(object sender, ProcedurePlanChangedEventArgs e)
        {
            RefreshProcedurePlanTree(e.ProcedurePlanSummary);
            EventsHelper.Fire(_procedurePlanChanged, this, EventArgs.Empty);
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
                        binding.IsCheckedSetter = delegate(Checkable<ModalityProcedureStepDetail> mps, bool isChecked)
                              {
                                  mps.IsChecked = isChecked;
                                  UpdateActionEnablement();
                              };

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
