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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    public abstract class WorkflowItemTool : Tool<IReportingWorkflowItemToolContext>, IDropHandler<ReportingWorklistItem>
    {
        public class StepType
        {
            public const string Interpretation = "Interpretation";
            public const string Transcription = "Transcription";
            public const string Verification = "Verification";
            public const string Publication = "Publication";
        }

        public class StepState
        {
            public const string Scheduled = "SC";
            public const string InProgress = "IP";
            public const string Completed = "CM";
        }



        protected string _operationName;

        public WorkflowItemTool(string operationName)
        {
            _operationName = operationName;
        }

        public virtual bool Enabled
        {
            get
            {
                return this.Context.GetWorkflowOperationEnablement(_operationName);
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }

        public virtual void Apply()
        {
            ReportingWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
            bool success = Execute(item, this.Context.DesktopWindow, this.Context.FolderSystem);
            if (success)
            {
                this.Context.FolderSystem.InvalidateSelectedFolder();
            }
        }

        protected string OperationName
        {
            get { return _operationName; }
        }

        protected bool ActivateIfAlreadyOpen(ReportingWorklistItem item)
        {
            Workspace workspace = DocumentManager.Get<ReportDocument>(item.ProcedureStepRef);
            if (workspace != null)
            {
                workspace.Activate();
                return true;
            }
            return false;
        }

        protected void OpenReportEditor(ReportingWorklistItem item)
        {
            if(!ActivateIfAlreadyOpen(item))
            {
                if (ReportDocumentSettings.Default.AllowOnlyOneReportingComponent)
                {
                    List<Workspace> documents = DocumentManager.GetAll<ReportDocument>();

                    // Show warning message and ask if the existing document should be closed or not
                    if (documents.Count > 0)
                    {
                        Workspace firstDocument = CollectionUtils.FirstElement(documents);
                        firstDocument.Activate();

                        if (DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(SR.MessageReportingComponentAlreadyOpened, MessageBoxActions.YesNo))
                        {
                            // Leave the existing document open
                            return;                            
                        }
                        else
                        {
                            // close documents and continue
                            CollectionUtils.ForEach(documents, delegate(Workspace document) { document.Close(); });
                        }
                    }
                }

                // open the report editor
                ReportDocument doc = new ReportDocument(item, this.Context.DesktopWindow);
                doc.Open();

                // open the images
                try
                {
                    IViewerIntegration viewerIntegration = (IViewerIntegration)(new ViewerIntegrationExtensionPoint()).CreateExtension();
                    if (viewerIntegration != null)
                        viewerIntegration.OpenStudy(item.AccessionNumber);
                }
                catch (NotSupportedException)
                {
                    Platform.Log(LogLevel.Info, "No viewer integration extension found.");
                }
            }
        }

        protected ReportingWorklistItem GetSelectedItem()
        {
            if (this.Context.SelectedItems.Count != 1)
                return null;
            return CollectionUtils.FirstElement(this.Context.SelectedItems);
        }

        protected abstract bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem);

        #region IDropHandler<ReportingWorklistItem> Members

        public virtual bool CanAcceptDrop(IDropContext dropContext, ICollection<ReportingWorklistItem> items)
        {
            IReportingWorkflowFolderDropContext ctxt = (IReportingWorkflowFolderDropContext)dropContext;
            return ctxt.GetOperationEnablement(this.OperationName);
        }

        public virtual bool ProcessDrop(IDropContext dropContext, ICollection<ReportingWorklistItem> items)
        {
            IReportingWorkflowFolderDropContext ctxt = (IReportingWorkflowFolderDropContext)dropContext;
            ReportingWorklistItem item = CollectionUtils.FirstElement(items);
            bool success = Execute(item, ctxt.DesktopWindow, ctxt.FolderSystem);
            if (success)
            {
                ctxt.FolderSystem.InvalidateSelectedFolder();
                return true;
            }
            return false;
        }

        #endregion
    }
}
