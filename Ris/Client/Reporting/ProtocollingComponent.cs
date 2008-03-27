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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ProtocollingOrderDetailViewComponent : DHtmlComponent
    {
        private WorklistItemSummaryBase _worklistItem;

        public ProtocollingOrderDetailViewComponent(WorklistItemSummaryBase worklistItem)
        {
            _worklistItem = worklistItem;
        }

        public override void Start()
        {
            SetUrl(ProtocollingComponentSettings.Default.OrderDetailPageUrl);
            base.Start();
        }

        public WorklistItemSummaryBase WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                Refresh();
            }
        }

        public void Refresh()
        {
            NotifyAllPropertiesChanged();
        }

        protected override DataContractBase GetHealthcareContext()
        {
            return _worklistItem;
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="ProtocollingComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocollingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocollingComponent class
    /// </summary>
    [AssociateView(typeof(ProtocollingComponentViewExtensionPoint))]
    public class ProtocollingComponent : ApplicationComponent
    {
        #region Private Fields

        private readonly ProtocollingComponentMode _componentMode;

        private ReportingWorklistItem _worklistItem;

        private readonly List<ReportingWorklistItem> _skippedItems;
        private readonly Stack<ReportingWorklistItem> _worklistCache;

        private ChildComponentHost _bannerComponentHost;
        private ChildComponentHost _protocolEditorComponentHost;
        private ProtocolEditorComponent _protocolEditorComponent;
        private ChildComponentHost _orderDetailViewComponentHost;
        private ChildComponentHost _priorReportsComponentHost;
        private ApplicationComponentHost _orderNotesComponentHost;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocollingComponent(ReportingWorklistItem worklistItem, ProtocollingComponentMode mode)
        {
            _worklistItem = worklistItem;
            _componentMode = mode;

            _skippedItems = new List<ReportingWorklistItem>();
            _worklistCache = new Stack<ReportingWorklistItem>();
        }

        #endregion

        #region ApplicationComponent overrides

        public override void Start()
        {
            _bannerComponentHost = new ChildComponentHost(this.Host, new BannerComponent(_worklistItem));
            _bannerComponentHost.StartComponent();

            _orderNotesComponentHost = new ChildComponentHost(this.Host, new OrderNoteSummaryComponent());
            _orderNotesComponentHost.StartComponent();

            _protocolEditorComponent = new ProtocolEditorComponent(_worklistItem, _componentMode);

            _protocolEditorComponent.ProtocolAccepted += OnProtocolAccepted;
            _protocolEditorComponent.ProtocolRejected += OnProtocolRejected;
            _protocolEditorComponent.ProtocolSuspended += OnProtocolSuspended;
            _protocolEditorComponent.ProtocolSaved += OnProtocolSaved;
            _protocolEditorComponent.ProtocolSkipped += OnProtocolSkipped;
            _protocolEditorComponent.ProtocolCancelled += OnProtocolCancelled;

            _protocolEditorComponentHost = new ChildComponentHost(this.Host, _protocolEditorComponent);
            _protocolEditorComponentHost.StartComponent();

            _priorReportsComponentHost = new ChildComponentHost(this.Host, new PriorReportComponent(_worklistItem));
            _priorReportsComponentHost.StartComponent();

            _orderDetailViewComponentHost = new ChildComponentHost(this.Host, new ProtocollingOrderDetailViewComponent(_worklistItem));
            _orderDetailViewComponentHost.StartComponent();

            this.Host.Title = ProtocollingComponentDocument.GetTitle(_worklistItem);

            base.Start();
        }

        #endregion

        #region Public members

        public ApplicationComponentHost BannerComponentHost
        {
            get { return _bannerComponentHost; }
        }

        public ApplicationComponentHost ProtocolEditorComponentHost
        {
            get { return _protocolEditorComponentHost; }
        }

        public ApplicationComponentHost OrderNotesComponentHost
        {
            get { return _orderNotesComponentHost; }
        }

        public ApplicationComponentHost OrderDetailViewComponentHost
        {
            get { return _orderDetailViewComponentHost; }
        }

        public ApplicationComponentHost PriorReportsComponentHost
        {
            get { return _priorReportsComponentHost; }
        }

        #endregion

        #region ProtocolEditorComponent event handlers

        private void OnProtocolAccepted(object sender, EventArgs e)
        {
            DocumentManager.InvalidateFolder(typeof(Folders.CompletedProtocolFolder));
            OnProtocolEndedHelper();
        }

        private void OnProtocolRejected(object sender, EventArgs e)
        {
            DocumentManager.InvalidateFolder(typeof(Folders.RejectedProtocolFolder));
            OnProtocolEndedHelper();
        }

        private void OnProtocolSuspended(object sender, EventArgs e)
        {
            DocumentManager.InvalidateFolder(typeof(Folders.SuspendedProtocolFolder));
            OnProtocolEndedHelper();
        }

        private void OnProtocolEndedHelper()
        {
            if (_componentMode == ProtocollingComponentMode.Assign)
            {
                DocumentManager.InvalidateFolder(typeof(Folders.ToBeProtocolledFolder));
            }
            else if (_componentMode == ProtocollingComponentMode.Edit)
            {
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
            }

            if (_protocolEditorComponent.ProtocolNextItem)
            {
                LoadNextProtocol();
            }
            else
            {
                this.Exit(ApplicationComponentExitCode.Accepted);
            }
        }

        private void OnProtocolSaved(object sender, EventArgs e)
        {
            DocumentManager.InvalidateFolder(typeof(Folders.ToBeProtocolledFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.DraftProtocolFolder));

            if (_protocolEditorComponent.ProtocolNextItem)
            {
                LoadNextProtocol();
            }
            else
            {
                this.Exit(ApplicationComponentExitCode.Accepted);
            }
        }

        private void OnProtocolSkipped(object sender, EventArgs e)
        {

            // To be protocolled folder will be invalid if it is the source of the worklist item;  the original item will have been
            // discontinued with a new scheduled one replacing it
            DocumentManager.InvalidateFolder(typeof(Folders.ToBeProtocolledFolder));

            _skippedItems.Add(_worklistItem);
            LoadNextProtocol();
        }

        private void OnProtocolCancelled(object sender, EventArgs e)
        {

            // To be protocolled folder will be invalid if it is the source of the worklist item;  the original item will have been
            // discontinued with a new scheduled one replacing it
            DocumentManager.InvalidateFolder(typeof(Folders.ToBeProtocolledFolder));

            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        #region Private methods

        private void LoadNextProtocol()
        {
            try
            {
                _worklistItem = GetNextWorklistItem();
                if (_worklistItem != null)
                {
                    ((BannerComponent) _bannerComponentHost.Component).HealthcareContext = _worklistItem;
                    ((PriorReportComponent) _priorReportsComponentHost.Component).WorklistItem = _worklistItem;
                    ((ProtocolEditorComponent) _protocolEditorComponentHost.Component).WorklistItem = _worklistItem;
                    ((ProtocollingOrderDetailViewComponent) _orderDetailViewComponentHost.Component).WorklistItem = _worklistItem;
                    _orderNotesComponentHost.StopComponent();
                    _orderNotesComponentHost.StartComponent();

                    // Update title
                    this.Host.Title = ProtocollingComponentDocument.GetTitle(_worklistItem);
                }
                else
                {
                    // TODO : Dialog "No more"
                    this.Exit(ApplicationComponentExitCode.None);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

        }

        private ReportingWorklistItem GetNextWorklistItem()
        {
            // Use next item from cached worklist if one exists
            ReportingWorklistItem worklistItem = _worklistCache.Count > 0 ? _worklistCache.Pop() : null;

            // Otherwise, re-populate the cached worklist
            if (worklistItem == null)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                            {
                                GetWorklistItemsRequest request = new GetWorklistItemsRequest(WorklistTokens.ReportingToBeProtocolledWorklist);
                                GetWorklistItemsResponse<ReportingWorklistItem> response = service.GetWorklistItems(request);

                                foreach (ReportingWorklistItem item in response.WorklistItems)
                                {
                                    // Remove any items that were previously skipped
                                    bool itemSkipped = CollectionUtils.Contains(_skippedItems,
                                                            delegate(ReportingWorklistItem skippedItem)
                                                            {
                                                                 return skippedItem.AccessionNumber == item.AccessionNumber;
                                                            });

                                    if (itemSkipped == false)
                                    {
                                        _worklistCache.Push(item);
                                    }
                                }

                                worklistItem = _worklistCache.Count > 0 ? _worklistCache.Pop() : null;
                            });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }

            return worklistItem;
        }

        #endregion
    }
}
