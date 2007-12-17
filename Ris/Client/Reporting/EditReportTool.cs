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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using System.Security.Permissions;

namespace ClearCanvas.Ris.Client.Reporting
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Edit Report", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Edit Report", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolSmall.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [LabelValueObserver("apply", "Label", "LabelChanged")]
    [ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.DraftFolder.DropHandlerExtensionPoint))]
    public class EditReportTool : WorkflowItemTool
    {

        public EditReportTool()
            : base("EditReport")
        {
        }

        public string Label
        {
            get
            {
                ReportingWorklistItem item = GetSelectedItem();
                if (item != null && item.ProcedureStepName == StepType.Interpretation && item.ActivityStatus.Code == StepState.Scheduled)
                    return SR.TitleCreateReport;
                else 
                    return SR.TitleEditReport;
            }
        }

        public event EventHandler LabelChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }

        public override bool Enabled
        {
            get
            {
                ReportingWorklistItem item = GetSelectedItem();
                return 
                    this.Context.GetWorkflowOperationEnablement("StartInterpretation") ||
                    this.Context.GetWorkflowOperationEnablement("StartVerification") ||

                    // there is no workflow operation for editing a previously created draft,
                    // so we enable the tool if it looks like a draft
                    // (ideally we need to check that the Performer is the same, but we don't have that information)
                    (item != null && item.ActivityStatus.Code == StepState.InProgress
                    && (item.ProcedureStepName == StepType.Interpretation || item.ProcedureStepName == StepType.Verification));
            }
        }

        public override bool CanAcceptDrop(IDropContext dropContext, ICollection<ReportingWorklistItem> items)
        {
            IReportingWorkflowFolderDropContext ctxt = (IReportingWorkflowFolderDropContext)dropContext;

            // this tool is only registered as a drop handler for the Drafts folder
            // and the only operation that would make sense in this context is StartInterpretation
            return ctxt.GetOperationEnablement("StartInterpretation");
        }

        protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem)
        {
            // check if the document is already open
            if(ActivateIfAlreadyOpen(item))
                return true;

            try
            {
                if (item.ProcedureStepName == StepType.Interpretation && item.ActivityStatus.Code == StepState.Scheduled)
                {
                    // if creating a new report, check for linked interpretations

                    List<ReportingWorklistItem> linkedInterpretations;
                    bool ok = PromptForLinkedInterpretations(item, out linkedInterpretations);
                    if (!ok)
                        return false;

                    // start the interpretation step
                    // note: updating only the ProcedureStepRef is hacky - the service should return an updated item
                    item.ProcedureStepRef = StartInterpretation(item, linkedInterpretations);
                }
                else if (item.ProcedureStepName == StepType.Verification && item.ActivityStatus.Code == StepState.Scheduled)
                {
                    // start the verification step
                    // note: updating only the ProcedureStepRef is hacky - the service should return an updated item
                    item.ProcedureStepRef = StartVerification(item);
                }

                // open the report editor
                OpenReportEditor(item);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }

            return true;
        }


        private bool PromptForLinkedInterpretations(ReportingWorklistItem item, out List<ReportingWorklistItem> linkedItems)
        {
            linkedItems = new List<ReportingWorklistItem>();

            // query server for link candidates
            List<ReportingWorklistItem> candidates = null;
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetLinkableInterpretationsRequest request = new GetLinkableInterpretationsRequest(item.ProcedureStepRef);
                    candidates = service.GetLinkableInterpretations(request).IntepretationItems;
                });

            // if there are candidates, prompt user to select
            if (candidates.Count > 0)
            {
                LinkedInterpretationComponent component = new LinkedInterpretationComponent(candidates);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Context.DesktopWindow, component, SR.TitleLinkProcedures);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    linkedItems.AddRange(component.SelectedItems);
                    return true;
                }
                return false;
            }
            else
            {
                // no candidates
                return true;
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.VerifyReport)]
        private EntityRef StartVerification(ReportingWorklistItem item)
        {
            EntityRef result = null;
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    StartVerificationResponse response = service.StartVerification(new StartVerificationRequest(item.ProcedureStepRef));
                    result = response.VerificationStepRef;
                });

            return result;
        }

        private EntityRef StartInterpretation(ReportingWorklistItem item, List<ReportingWorklistItem> linkedInterpretations)
        {
            List<EntityRef> linkedInterpretationRefs = linkedInterpretations.ConvertAll<EntityRef>(
                delegate(ReportingWorklistItem x) { return x.ProcedureStepRef; });


            EntityRef result = null;
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    StartInterpretationRequest request = new StartInterpretationRequest(item.ProcedureStepRef, linkedInterpretationRefs);
                    StartInterpretationResponse response = service.StartInterpretation(request);
                    result = response.InterpretationStepRef;
                });

            return result;
        }
    }
}
