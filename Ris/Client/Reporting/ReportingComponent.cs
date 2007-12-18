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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="ReportingComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ReportingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ReportingComponent class
    /// </summary>
    [AssociateView(typeof(ReportingComponentViewExtensionPoint))]
    public class ReportingComponent : ApplicationComponent
    {
        private readonly ReportingWorklistItem _worklistItem;

        private readonly BannerComponent _bannerComponent;
        private ChildComponentHost _bannerHost;

        private readonly ReportEditorComponent _reportEditorComponent;
        private ChildComponentHost _reportEditorHost;

        private readonly PriorReportComponent _priorReportComponent;
        private ChildComponentHost _priorReportHost;

        private readonly OrderDetailViewComponent _orderDetailComponent;
        private ChildComponentHost _orderDetailHost;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportingComponent(ReportingWorklistItem worklistItem)
        {
            _worklistItem = worklistItem;

            _bannerComponent = new BannerComponent(_worklistItem);
            _reportEditorComponent = new ReportEditorComponent(_worklistItem);
            _priorReportComponent = new PriorReportComponent(_worklistItem);
            _orderDetailComponent = new OrderDetailViewComponent(_worklistItem);
        }

        public override void Start()
        {
            _bannerHost = new ChildComponentHost(this.Host, _bannerComponent);
            _bannerHost.StartComponent();

            _reportEditorHost = new ChildComponentHost(this.Host, _reportEditorComponent);
            _reportEditorHost.StartComponent();

            _priorReportHost = new ChildComponentHost(this.Host, _priorReportComponent);
            _priorReportHost.StartComponent();

            _orderDetailHost = new ChildComponentHost(this.Host, _orderDetailComponent);
            _orderDetailHost.StartComponent();

            // Setup the various editor closed events.  Do not invalidate the ToBeReported folder type, since it's communual
            _reportEditorComponent.VerifyEvent += _reportEditorComponent_VerifyEvent;
            _reportEditorComponent.SendToVerifyEvent += _reportEditorComponent_SendToVerifyEvent;
            _reportEditorComponent.SendToTranscriptionEvent += _reportEditorComponent_SendToTranscriptionEvent;
            _reportEditorComponent.SaveEvent += _reportEditorComponent_SaveEvent;
            _reportEditorComponent.CloseComponentEvent += _reportEditorComponent_CloseComponentEvent;

            base.Start();
        }

        #region Presentation Model

        public ApplicationComponentHost BannerHost
        {
            get { return _bannerHost; }
        }

        public ApplicationComponentHost ReportEditorHost
        {
            get { return _reportEditorHost; }
        }

        public ApplicationComponentHost PriorReportsHost
        {
            get { return _priorReportHost; }
        }

        public ApplicationComponentHost OrderDetailsHost
        {
            get { return _orderDetailHost; }
        }

        #endregion

        #region Private Event Handlers

        void _reportEditorComponent_VerifyEvent(object sender, EventArgs e)
        {
            // Source Folders
            //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
            // Destination Folders
            DocumentManager.InvalidateFolder(typeof(Folders.VerifiedFolder));
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        void _reportEditorComponent_SendToVerifyEvent(object sender, EventArgs e)
        {
            // Source Folders
            //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
            // Destination Folders
            DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        void _reportEditorComponent_SendToTranscriptionEvent(object sender, EventArgs e)
        {
            // Source Folders
            //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
            // Destination Folders
            DocumentManager.InvalidateFolder(typeof(Folders.InTranscriptionFolder));
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        void _reportEditorComponent_SaveEvent(object sender, EventArgs e)
        {
            // Source Folders
            //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.VerifiedFolder));
            // Destination Folders
            DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        void _reportEditorComponent_CloseComponentEvent(object sender, EventArgs e)
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion
    }
}
