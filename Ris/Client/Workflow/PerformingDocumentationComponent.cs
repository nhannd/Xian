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
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.PerformingDocumentation;

namespace ClearCanvas.Ris.Client.Workflow
{
    /// <summary>
    /// Defines an interface for providing custom documentation pages to be displayed in the documentation workspace.
    /// </summary>
    public interface IPerformingDocumentationPageProvider : IExtensionPageProvider<IPerformingDocumentationPage, IPerformingDocumentationContext>
    {
    }

    /// <summary>
    /// Defines an extension point for adding custom documentation pages to the performing documentation workspace.
    /// </summary>
    [ExtensionPoint]
    public class PerformingDocumentationPageProviderExtensionPoint : ExtensionPoint<IPerformingDocumentationPageProvider>
    {
    }

    /// <summary>
    /// Defines an interface for providing a custom documentation page with access to the documentation
    /// context.
    /// </summary>
    public interface IPerformingDocumentationContext
    {
        /// <summary>
        /// Exposes the order reference.
        /// </summary>
        EntityRef OrderRef { get; }

        /// <summary>
        /// Exposes the extended properties associated with the Order.  Modifications made to these
        /// properties by the documentation page will be persisted whenever the documentation workspace is saved.
        /// </summary>
        IDictionary<string, string> OrderExtendedProperties { get; }

        /// <summary>
        /// Exposes the order notes associated with the order.  Modifications made to this
        /// collection will be persisted when the documentation workspace is saved.
        /// </summary>
        List<OrderNoteDetail> OrderNotes { get; }

        /// <summary>
        /// Gets the <see cref="ProcedurePlanDetail"/> representing this order.
        /// </summary>
        ProcedurePlanDetail ProcedurePlan { get; }

        /// <summary>
        /// Occurs when the value of the <see cref="ProcedurePlan"/> property changes.
        /// </summary>
        event EventHandler ProcedurePlanChanged;
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="PerformingDocumentationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PerformingDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
	/// PerformingDocumentationComponent class
    /// </summary>
    [AssociateView(typeof(PerformingDocumentationComponentViewExtensionPoint))]
    public class PerformingDocumentationComponent : ApplicationComponent
	{
		#region PerformingDocumentationContext class

		class PerformingDocumentationContext : IPerformingDocumentationContext
        {
            private readonly PerformingDocumentationComponent _owner;

            public PerformingDocumentationContext(PerformingDocumentationComponent owner)
            {
                _owner = owner;
			}

			#region IPerformingDocumentationContext Members

			public EntityRef OrderRef
            {
                get { return _owner._procedurePlan.OrderRef; }
            }

            public IDictionary<string, string> OrderExtendedProperties
            {
                get { return _owner._orderExtendedProperties; }
            }

            public List<OrderNoteDetail> OrderNotes
            {
                get { return _owner._orderNotes; }
            }

            public ProcedurePlanDetail ProcedurePlan
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
        private List<OrderNoteDetail> _orderNotes;

        private ProcedurePlanDetail _procedurePlan;
        private ProcedurePlanSummaryTable _procedurePlanSummaryTable;
        private event EventHandler _procedurePlanChanged;

        private SimpleActionModel _procedurePlanActionHandler;
        private ClickAction _startAction;
        private ClickAction _discontinueAction;

        private ChildComponentHost _bannerComponentHost;
        private ChildComponentHost _documentationHost;
        private TabComponentContainer _documentationTabContainer;

        private ILookupHandler _radiologistLookupHandler;
        private StaffSummary _assignedRadiologist;

        private readonly List<IPerformingDocumentationPage> _extensionPages = new List<IPerformingDocumentationPage>();

        private PerformedProcedureComponent _ppsComponent;
        private PerformingDocumentationOrderDetailsComponent _orderDetailsComponent;

        private bool _completeEnabled;
        private bool _alreadyCompleted;

        #endregion

        public PerformingDocumentationComponent(ModalityWorklistItem item)
        {
            _worklistItem = item;
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            InitializeProcedurePlanSummary();
            InitializeDocumentationTabPages();

            // create staff lookup handler, using filters provided by application configuration
			string filters = PerformingDocumentationComponentSettings.Default.RadiologistStaffTypeFilters;
            string[] staffTypes = string.IsNullOrEmpty(filters) ? new string[] { } :
                CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); }).ToArray();
 
            _radiologistLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, staffTypes);

            base.Start();
        }

        public override void Stop()
        {
            if (_bannerComponentHost != null)
            {
                _bannerComponentHost.StopComponent();
                _bannerComponentHost = null;
            }

            if (_documentationHost != null)
            {
                _documentationHost.StopComponent();
                _documentationHost = null;
            }

            base.Stop();
        }

        public override bool HasValidationErrors
        {
            get
            {
                return _documentationTabContainer.HasValidationErrors || base.HasValidationErrors;
            }
        }

        public override void ShowValidation(bool show)
        {
            _documentationTabContainer.ShowValidation(show);
            base.ShowValidation(show);
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

        public ILookupHandler RadiologistLookupHandler
        {
            get { return _radiologistLookupHandler; }
        }

        public StaffSummary AssignedRadiologist
        {
            get { return _assignedRadiologist; }
            set
            {
                if(!Equals(value, _assignedRadiologist))
                {
                    _assignedRadiologist = value;
                    NotifyPropertyChanged("AssignedRadiologist");
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Documentation.Create)]
        public void SaveDocumentation()
        {
            try
            {
                if (Save(false))
                {
                    DocumentManager.InvalidateFolder(typeof(Folders.Performing.InProgressPerformingWorkflowFolder));
                    DocumentManager.InvalidateFolder(typeof(Folders.Performing.UndocumentedPerformingWorkflowFolder));
                    DocumentManager.InvalidateFolder(typeof(Folders.Performing.CancelledPerformingWorkflowFolder));
                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public string SaveText
        {
            get { return _alreadyCompleted ? SR.TextUpdate : SR.TextSave; }    
        }

        public bool SaveEnabled
        {
            get { return true; }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Documentation.Accept)]
        public void CompleteDocumentation()
        {
            try
            {
                // validate first
                if (Save(true))
                {
                    DocumentManager.InvalidateFolder(typeof(Folders.Performing.CancelledPerformingWorkflowFolder));
                    DocumentManager.InvalidateFolder(typeof(Folders.Performing.CompletedPerformingWorkflowFolder));
                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
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

        public bool CompleteVisible
        {
            get
            {
                return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Documentation.Accept);
            }
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
                    DateTime? startTime = Platform.Time;
                    if (DowntimeRecovery.InDowntimeRecoveryMode)
                    {
                        if (!DateTimeEntryComponent.PromptForTime(this.Host.DesktopWindow, "Start Time", false, ref startTime))
                            return;
                    }

                    Platform.GetService<IModalityWorkflowService>(
                        delegate(IModalityWorkflowService service)
                        {
                            StartModalityProcedureStepsRequest request = new StartModalityProcedureStepsRequest(checkedMpsRefs);
                            request.StartTime = DowntimeRecovery.InDowntimeRecoveryMode ? startTime : null;
                            StartModalityProcedureStepsResponse response = service.StartModalityProcedureSteps(request);

                            RefreshProcedurePlanSummary(response.ProcedurePlan);
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
                    DateTime? discontinueTime = Platform.Time;
                    if (DowntimeRecovery.InDowntimeRecoveryMode)
                    {
                        if (!DateTimeEntryComponent.PromptForTime(this.Host.DesktopWindow, "Cancel Time", false, ref discontinueTime))
                            return;
                    }

                    Platform.GetService<IModalityWorkflowService>(
                        delegate(IModalityWorkflowService service)
                        {
                            DiscontinueModalityProcedureStepsRequest request = new DiscontinueModalityProcedureStepsRequest(checkedMpsRefs);
                            request.DiscontinuedTime = DowntimeRecovery.InDowntimeRecoveryMode ? discontinueTime : null;
                            DiscontinueModalityProcedureStepsResponse response = service.DiscontinueModalityProcedureSteps(request);

                            RefreshProcedurePlanSummary(response.ProcedurePlan);
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

        private bool Save(bool completeDocumentation)
        {
            // only do validation if they are completing the documentation, not if they are just saving a draft
            if(completeDocumentation)
            {
                if (this.HasValidationErrors)
                {
                    ShowValidation(true);
                    return false;
                }
            }

            try
            {
                // allow extension pages to save data
                bool veto = false;
                foreach(IPerformingDocumentationPage page in _extensionPages)
                {
                    veto = veto || !page.Save(completeDocumentation);
                }
                if(veto)
                    return false;

                _orderDetailsComponent.SaveData();
                _ppsComponent.SaveData();
				Platform.GetService<IPerformingDocumentationService>(
					delegate(IPerformingDocumentationService service)
                        {
                            // TODO clean this up - this is a bit ugly, not sure if there's a cleaner way
                            Dictionary<EntityRef, Dictionary<string, string>> ppsExtendedProperties
                                                 = new Dictionary<EntityRef, Dictionary<string, string>>();
                            foreach (ModalityPerformedProcedureStepDetail step in _ppsComponent.PerformedProcedureSteps)
                            {
                                ppsExtendedProperties[step.ModalityPerformendProcedureStepRef] = step.ExtendedProperties;
                            }


                            SaveDataRequest saveRequest =
                                new SaveDataRequest(_procedurePlan.OrderRef, _orderExtendedProperties, _orderNotes,
                                        ppsExtendedProperties, _assignedRadiologist);
                            SaveDataResponse saveResponse = service.SaveData(saveRequest);

                            if (completeDocumentation)
                            {
                                CompleteOrderDocumentationRequest completeRequest =
                                    new CompleteOrderDocumentationRequest(saveResponse.ProcedurePlan.OrderRef);
                                CompleteOrderDocumentationResponse completeResponse =
                                    service.CompleteOrderDocumentation(completeRequest);

                                RefreshProcedurePlanSummary(completeResponse.ProcedurePlan);
                            }
                            else
                            {
                                RefreshProcedurePlanSummary(saveResponse.ProcedurePlan);
                            }
                        });

                return true;
            }
            catch(Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            return false;
        }

        private void InitializeProcedurePlanSummary()
        {
            _procedurePlanSummaryTable = new ProcedurePlanSummaryTable();
            _procedurePlanSummaryTable.CheckedRowsChanged += delegate(object sender, EventArgs args) { UpdateActionEnablement(); };

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    GetProcedurePlanRequest procedurePlanRequest = new GetProcedurePlanRequest(_worklistItem.OrderRef);
                    GetProcedurePlanResponse procedurePlanResponse = service.GetProcedurePlan(procedurePlanRequest);
                    _procedurePlan = procedurePlanResponse.ProcedurePlan;
                });

            RefreshProcedurePlanSummary(_procedurePlan);

			Platform.GetService<IPerformingDocumentationService>(
				delegate(IPerformingDocumentationService service)
                {
                    LoadDataResponse response = service.LoadData(new LoadDataRequest(_worklistItem.OrderRef));
                    _orderExtendedProperties = response.OrderExtendedProperties;
                    _orderNotes = response.OrderNotes;
                    this.AssignedRadiologist = response.AssignedInterpreter;
                });

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
            PerformingDocumentationContext context = new PerformingDocumentationContext(this);

            _bannerComponentHost = new ChildComponentHost(this.Host, new BannerComponent(_worklistItem));
            _bannerComponentHost.StartComponent();

            _documentationTabContainer = new TabComponentContainer();
            _documentationTabContainer.ValidationStrategy = new AllComponentsValidationStrategy();

            _orderDetailsComponent = new PerformingDocumentationOrderDetailsComponent(context, _worklistItem);
            _documentationTabContainer.Pages.Add(new TabPage("Order", _orderDetailsComponent));

            _ppsComponent = new PerformedProcedureComponent(_procedurePlan.OrderRef, this);
            _ppsComponent.ProcedurePlanChanged += delegate(object sender, ProcedurePlanChangedEventArgs e) { RefreshProcedurePlanSummary(e.ProcedurePlanDetail); };
            _documentationTabContainer.Pages.Add(new TabPage("Exam", _ppsComponent));

            // create extension pages
            foreach (IPerformingDocumentationPageProvider pageProvider in (new PerformingDocumentationPageProviderExtensionPoint()).CreateExtensions())
            {
                _extensionPages.AddRange(pageProvider.GetPages(context));
            }

            foreach (IPerformingDocumentationPage page in _extensionPages)
            {
                _documentationTabContainer.Pages.Add(new TabPage(page.Path.LocalizedPath, page.GetComponent()));
            }

            _documentationHost = new ChildComponentHost(this.Host, _documentationTabContainer);
            _documentationHost.StartComponent();

            SetInitialDocumentationTabPage();
        }

        private void SetInitialDocumentationTabPage()
        {
			string selectedTabName = PerformingDocumentationComponentSettings.Default.InitiallySelectedTabPageName;
            if(string.IsNullOrEmpty(selectedTabName))
                return;

            TabPage requestedTabPage = CollectionUtils.SelectFirst(
                _documentationTabContainer.Pages,
                delegate(TabPage tabPage) { return tabPage.Name.Equals(selectedTabName, StringComparison.InvariantCultureIgnoreCase); });

            if (requestedTabPage != null)
                _documentationTabContainer.CurrentPage = requestedTabPage;
        }

        private List<ProcedurePlanSummaryTableItem> ListCheckedSummmaryTableItems()
        {
            return CollectionUtils.Map<Checkable<ProcedurePlanSummaryTableItem>, ProcedurePlanSummaryTableItem>(
                CollectionUtils.Select(
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
                _startAction.Enabled = CollectionUtils.TrueForAll(checkedSummaryTableItems,
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.State.Code == "SC"; });

                _discontinueAction.Enabled = CollectionUtils.TrueForAll(checkedSummaryTableItems,
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.State.Code == "SC"; });
            }
        }

        private void RefreshProcedurePlanSummary(ProcedurePlanDetail procedurePlanDetail)
        {
            _procedurePlan = procedurePlanDetail;

            try
            {
				Platform.GetService<IPerformingDocumentationService>(
					delegate(IPerformingDocumentationService service)
                        {
                            CanCompleteOrderDocumentationResponse response = 
                                service.CanCompleteOrderDocumentation(new CanCompleteOrderDocumentationRequest(_procedurePlan.OrderRef));

                            _completeEnabled = response.CanComplete;
                            _alreadyCompleted = response.AlreadyCompleted;
                            this.NotifyPropertyChanged("CompleteEnabled");
                        });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            _procedurePlanSummaryTable.Items.Clear();
            foreach(ProcedureDetail rp in procedurePlanDetail.Procedures)
            {
                foreach(ProcedureStepDetail mps in rp.ProcedureSteps)
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
