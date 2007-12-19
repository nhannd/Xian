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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
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
        #region TechnologistDocumentationContext class

        class TechnologistDocumentationContext : ToolContext, ITechnologistDocumentationContext
        {
            private readonly TechnologistDocumentationComponent _owner;

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
        private Dictionary<string, string> _orderExtendedProperties;

        private ProcedurePlanSummary _procedurePlan;
        private ProcedurePlanSummaryTable _procedurePlanSummaryTable;
        private event EventHandler _procedurePlanChanged;

        private SimpleActionModel _procedurePlanActionHandler;
        private ClickAction _startAction;
        private ClickAction _discontinueAction;

        private ChildComponentHost _bannerComponentHost;
        private ChildComponentHost _documentationHost;
        private TabComponentContainer _documentationTabContainer;

        private readonly List<ITechnologistDocumentationModule> _documentationModules = new List<ITechnologistDocumentationModule>();

        private ExamDetailsComponent _preExamComponent;
        private ExamDetailsComponent _postExamComponent;
        private PerformedProcedureComponent _ppsComponent;
        private TechnologistDocumentationOrderDetailsComponent _orderDetailsComponent;

        private bool _completeEnabled;
        private bool _saveEnabled = true;

        private event EventHandler _documentCompleted;
        private event EventHandler _documentSaved;

        #endregion

        public TechnologistDocumentationComponent(ModalityWorklistItem item)
        {
            _worklistItem = item;
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            InitializeProcedurePlanSummary();
            InitializeDocumentationTabPages();

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

        public ApplicationComponentHost BannerHost
        {
            get { return _bannerComponentHost; }
        }

        public ApplicationComponentHost DocumentationHost
        {
            get { return _documentationHost; }
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

        public ActionModelNode ProcedurePlanTreeActionModel
        {
            get { return _procedurePlanActionHandler; }
        }

        public void SaveDocumentation()
        {
            try
            {
                Save(false);

                EventsHelper.Fire(_documentSaved, this, EventArgs.Empty);
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

        public event EventHandler DocumentSaved
        {
            add { _documentSaved += value; }
            remove { _documentSaved -= value; }
        }

        public void CompleteDocumentation()
        {
            try
            {
                // validate first
                Save(true);

                EventsHelper.Fire(_documentCompleted, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public bool CompleteEnabled
        {
            get { return _completeEnabled; }
        }

        public event EventHandler DocumentCompleted
        {
            add { _documentCompleted += value; }
            remove { _documentCompleted -= value; }
        }

        #endregion

        #region Action Handler Methods

        private void StartModalityProcedureSteps()
        {
            try
            {
                List<EntityRef> checkedMpsRefs = CollectionUtils.Map<ProcedurePlanSummaryTableItem, EntityRef, List<EntityRef>>(
                    ListCheckedSummmaryTableItems(),
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.ProcedureStepRef; });

                if (checkedMpsRefs.Count > 0)
                {
                    Platform.GetService<IModalityWorkflowService>(
                        delegate(IModalityWorkflowService service)
                        {
                            StartModalityProcedureStepsRequest request = new StartModalityProcedureStepsRequest(checkedMpsRefs);
                            StartModalityProcedureStepsResponse response = service.StartModalityProcedureSteps(request);

                            RefreshProcedurePlanSummary(response.ProcedurePlanSummary);
                            UpdateActionEnablement();

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
                List<EntityRef> checkedMpsRefs = CollectionUtils.Map<ProcedurePlanSummaryTableItem, EntityRef, List<EntityRef>>(
                    ListCheckedSummmaryTableItems(),
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.ProcedureStepRef; });

                if (checkedMpsRefs.Count > 0)
                {
                    Platform.GetService<IModalityWorkflowService>(
                        delegate(IModalityWorkflowService service)
                        {
                            DiscontinueModalityProcedureStepsRequest request = new DiscontinueModalityProcedureStepsRequest(checkedMpsRefs);
                            DiscontinueModalityProcedureStepsResponse response = service.DiscontinueModalityProcedureSteps(request);

                            RefreshProcedurePlanSummary(response.ProcedurePlanSummary);
                            UpdateActionEnablement();
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

        private void Save(bool completeDocumentation)
        {
            _preExamComponent.SaveData();
            _postExamComponent.SaveData();

            foreach(ITechnologistDocumentationModule module in _documentationModules)
            {
                module.SaveData();
            }

            try
            {
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
                        {
                            SaveDataRequest saveRequest =
                                new SaveDataRequest(_procedurePlan.OrderRef, _orderExtendedProperties);
                            SaveDataResponse saveResponse = service.SaveData(saveRequest);

                            if (completeDocumentation)
                            {
                                CompleteOrderDocumentationRequest completeRequest =
                                    new CompleteOrderDocumentationRequest(saveResponse.ProcedurePlanSummary.OrderRef);
                                CompleteOrderDocumentationResponse completeResponse =
                                    service.CompleteOrderDocumentation(completeRequest);

                                RefreshProcedurePlanSummary(completeResponse.ProcedurePlanSummary);
                            }
                            else
                            {
                                RefreshProcedurePlanSummary(saveResponse.ProcedurePlanSummary);
                            }
                        });
            }
            catch(Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void InitializeProcedurePlanSummary()
        {
            _procedurePlanSummaryTable = new ProcedurePlanSummaryTable();
            _procedurePlanSummaryTable.CheckedRowsChanged += delegate(object sender, EventArgs args) { UpdateActionEnablement(); };

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    GetProcedurePlanForWorklistItemRequest procedurePlanRequest = new GetProcedurePlanForWorklistItemRequest(_worklistItem.ProcedureStepRef);
                    GetProcedurePlanForWorklistItemResponse procedurePlanResponse = service.GetProcedurePlanForWorklistItem(procedurePlanRequest);
                    _procedurePlan = procedurePlanResponse.ProcedurePlanSummary;
                    _orderExtendedProperties = procedurePlanResponse.OrderExtendedProperties;
                });

            RefreshProcedurePlanSummary(_procedurePlan);

            InitializeProcedurePlanSummaryActionHandlers();
        }

        private void InitializeProcedurePlanSummaryActionHandlers()
        {
            _procedurePlanActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _startAction = _procedurePlanActionHandler.AddAction("start", SR.TitleStartMps, "Icons.StartToolSmall.png", SR.TitleStartMps, StartModalityProcedureSteps);
            _discontinueAction = _procedurePlanActionHandler.AddAction("discontinue", SR.TitleDiscontinueMps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMps, DiscontinueModalityProcedureSteps);
            UpdateActionEnablement();
        }

        private void InitializeDocumentationTabPages()
        {
            _bannerComponentHost = new ChildComponentHost(this.Host, new BannerComponent(_worklistItem));
            _bannerComponentHost.StartComponent();

            _documentationTabContainer = new TabComponentContainer();

            _orderDetailsComponent = new TechnologistDocumentationOrderDetailsComponent(_worklistItem);
            InsertDocumentationPage(_orderDetailsComponent, 0);

            _preExamComponent = new ExamDetailsComponent("Pre-exam",
                                                         TechnologistDocumentationComponentSettings.Default.PreExamDetailsPageUrl,
                                                         _orderExtendedProperties);
            InsertDocumentationPage(_preExamComponent, 1);

            _ppsComponent = new PerformedProcedureComponent("Exam", _procedurePlan.OrderRef, this);
            _ppsComponent.ProcedurePlanChanged += delegate(object sender, ProcedurePlanChangedEventArgs e) { RefreshProcedurePlanSummary(e.ProcedurePlanSummary); };
            InsertDocumentationPage(_ppsComponent, 2);

            // create extension modules, which may add documentation pages to the tab container
            TechnologistDocumentationContext context = new TechnologistDocumentationContext(this);
            foreach (ITechnologistDocumentationModule module in (new TechnologistDocumentationModuleExtensionPoint()).CreateExtensions())
            {
                module.Initialize(context);
                _documentationModules.Add(module);
            }

            _postExamComponent = new ExamDetailsComponent("Post-exam",
                                                          TechnologistDocumentationComponentSettings.Default.PostExamDetailsPageUrl,
                                                          _orderExtendedProperties);
            InsertDocumentationPage(_postExamComponent, _documentationTabContainer.Pages.Count);

            _documentationHost = new ChildComponentHost(this.Host, _documentationTabContainer);
            _documentationHost.StartComponent();

            SetInitialDocumentationTabPage();
        }

        private void SetInitialDocumentationTabPage()
        {
            // TODO add a setting for initial page
            string requestedTabPageName = "Exam";

            TabPage requestedTabPage = CollectionUtils.SelectFirst<TabPage>(
                _documentationTabContainer.Pages,
                delegate(TabPage tabPage) { return string.Compare(tabPage.Name, requestedTabPageName, true) == 0; });

            if (requestedTabPage != null)
                _documentationTabContainer.CurrentPage = requestedTabPage;
        }

        private void InsertDocumentationPage(IDocumentationPage page, int position)
        {
            _documentationTabContainer.Pages.Insert(position, new TabPage(page.Title, page.Component));
        }

        private List<ProcedurePlanSummaryTableItem> ListCheckedSummmaryTableItems()
        {
            return CollectionUtils.Map<Checkable<ProcedurePlanSummaryTableItem>, ProcedurePlanSummaryTableItem>(
                CollectionUtils.Select<Checkable<ProcedurePlanSummaryTableItem>>(
                    _procedurePlanSummaryTable.Items,
                    delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.IsChecked; }),
                delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item; });
        }

        private void UpdateActionEnablement()
        {
            IList<ProcedurePlanSummaryTableItem> checkedSummaryTableItems = ListCheckedSummmaryTableItems();
            if (checkedSummaryTableItems.Count == 0)
            {
                _startAction.Enabled = _discontinueAction.Enabled = false;
            }
            else
            {
                // TODO: defer enablement to server
                _startAction.Enabled = CollectionUtils.TrueForAll(checkedSummaryTableItems,
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.State.Code == "SC"; });

                _discontinueAction.Enabled = CollectionUtils.TrueForAll(checkedSummaryTableItems,
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.State.Code == "SC"; });
            }
        }

        private void RefreshProcedurePlanSummary(ProcedurePlanSummary procedurePlanSummary)
        {
            _procedurePlan = procedurePlanSummary;

            try
            {
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
                        {
                            CanCompleteOrderDocumentationResponse response = 
                                service.CanCompleteOrderDocumentation(new CanCompleteOrderDocumentationRequest(_procedurePlan.OrderRef));

                            _completeEnabled = response.CanComplete;
                            this.NotifyPropertyChanged("CompleteEnabled");
                        });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            _procedurePlanSummaryTable.Items.Clear();
            foreach(RequestedProcedureDetail rp in procedurePlanSummary.RequestedProcedures)
            {
                foreach(ModalityProcedureStepDetail mps in rp.ModalityProcedureSteps)
                {
                    _procedurePlanSummaryTable.Items.Add(
                        new Checkable<ProcedurePlanSummaryTableItem>(
                            new ProcedurePlanSummaryTableItem(rp, mps)));
                }
            }
            _procedurePlanSummaryTable.Sort();

            EventsHelper.Fire(_procedurePlanChanged, this, EventArgs.Empty);
        }

        #endregion
    }
}
