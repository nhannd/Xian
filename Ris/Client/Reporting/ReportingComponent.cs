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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Reporting
{
    ///// <summary>
    ///// Extension point for views onto <see cref="ReportingComponent"/>
    ///// </summary>
    //[ExtensionPoint]
    //public class ReportingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    //{
    //}

    /// <summary>
    /// ReportingComponent class
    /// </summary>
    //[AssociateView(typeof(ReportingComponentViewExtensionPoint))]
    public class ReportingComponent : SplitComponentContainer
    {
        private ReportingWorklistItem _worklistItem;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportingComponent(ReportingWorklistItem worklistItem)
            :base(Desktop.SplitOrientation.Vertical)
        {
            _worklistItem = worklistItem;
        }

        public override void Start()
        {
            List<EntityRef> procedureRefs = new List<EntityRef>();
            procedureRefs.Add(_worklistItem.RequestedProcedureRef);
            PriorReportComponent priorComponent = new PriorReportComponent(_worklistItem);

            // Create tab and tab groups
            TabComponentContainer tabContainer1 = new TabComponentContainer();
            tabContainer1.Pages.Add(new TabPage("Prior Reports", priorComponent));

            TabGroupComponentContainer tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer1, 1.0f));

            ReportEditorComponent reportEditor = new ReportEditorComponent(_worklistItem.ProcedureStepRef);

            // Setup the various editor closed events.  Do not invalidate the ToBeReported folder type, since it's communual
            reportEditor.VerifyEvent += delegate
            {
                // Source Folders
                //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
                // Destination Folders
                DocumentManager.InvalidateFolder(typeof(Folders.VerifiedFolder));
                this.Exit(ApplicationComponentExitCode.Accepted);
            };
            reportEditor.SendToVerifyEvent += delegate
            {
                // Source Folders
                //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
                // Destination Folders
                DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
                this.Exit(ApplicationComponentExitCode.Accepted);
            };
            reportEditor.SendToTranscriptionEvent += delegate
            {
                // Source Folders
                //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
                // Destination Folders
                DocumentManager.InvalidateFolder(typeof(Folders.InTranscriptionFolder));
                this.Exit(ApplicationComponentExitCode.Accepted);
            };
            reportEditor.SaveEvent += delegate
            {
                // Source Folders
                //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.VerifiedFolder));
                // Destination Folders
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
                this.Exit(ApplicationComponentExitCode.Accepted);
            };
            reportEditor.CloseComponentEvent += delegate
            {
                this.Exit(ApplicationComponentExitCode.None);
            };

            this.Pane1 = new SplitPane("", reportEditor, 0.5f);
            this.Pane2 = new SplitPane("", tabGroupContainer, 0.5f);

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }
    }
}
