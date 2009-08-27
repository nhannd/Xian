#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
    public class StepType
    {
        public const string TranscriptionReview = "Transcription Review";
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

    public abstract class ReportingWorkflowItemTool : WorkflowItemTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
    {
        protected ReportingWorkflowItemTool(string operationName)
            : base(operationName)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            this.Context.RegisterWorkflowService(typeof(IReportingWorkflowService));
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
            OpenReportEditor(item, true);
        }

        protected void OpenReportEditor(ReportingWorklistItem item, bool shouldOpenImages)
        {
            if (!ActivateIfAlreadyOpen(item))
            {
                if (!ReportingSettings.Default.AllowMultipleReportingWorkspaces)
                {
                    List<Workspace> documents = DocumentManager.GetAll<ReportDocument>();

                    // Show warning message and ask if the existing document should be closed or not
                    if (documents.Count > 0)
                    {
                        Workspace firstDocument = CollectionUtils.FirstElement(documents);
                        firstDocument.Activate();

                        string message = string.Format(SR.MessageReportingComponentAlreadyOpened, firstDocument.Title, PersonNameFormat.Format(item.PatientName));
                        if (DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
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
                ReportDocument doc = new ReportDocument(item, shouldOpenImages, this.Context);
                doc.Open();

                // Need to re-invalidate folders that open a report document, since cancelling the report
                // can re-insert items into the same folder.
                Type selectedFolderType = this.Context.SelectedFolder.GetType();  // use closure to remember selected folder at time tool is invoked.
                doc.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };

                Workspace w = DocumentManager.Get(DocumentManager.GenerateDocumentKey(doc, item.ProcedureStepRef));
                if (((ReportingComponent)w.Component).UserCancelled)
                    doc.Close();
            }
        }

        protected ReportingWorklistItem GetSelectedItem()
        {
            if (this.Context.SelectedItems.Count != 1)
                return null;
            return CollectionUtils.FirstElement(this.Context.SelectedItems);
        }

        protected EntityRef GetSupervisorRef()
        {
            ReportingSupervisorSelectionComponent component = new ReportingSupervisorSelectionComponent();
            if (ApplicationComponentExitCode.Accepted == ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.TitleSelectSupervisor))
                return component.Staff != null ? component.Staff.StaffRef : null;
            else
                return null;
        }
    }
}
