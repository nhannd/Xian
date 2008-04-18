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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Send/To Transcription", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.UseTranscriptionWorkflow)]
    [ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.InTranscriptionFolder.DropHandlerExtensionPoint))]
    public class CompleteInterpretationForTranscriptionTool : WorkflowItemTool
    {
        public CompleteInterpretationForTranscriptionTool()
            : base("CompleteInterpretationForTranscription")
        {
        }

        protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem)
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationForTranscription(new CompleteInterpretationForTranscriptionRequest(item.ProcedureStepRef));
                    });

                folderSystem.InvalidateFolder(typeof(Folders.InTranscriptionFolder));

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Send/To be Verified", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.ToBeVerifiedFolder.DropHandlerExtensionPoint))]
    public class CompleteInterpretationForVerificationTool : WorkflowItemTool
    {
        public CompleteInterpretationForVerificationTool()
            : base("CompleteInterpretationForVerification")
        {
        }

        protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem)
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationForVerification(new CompleteInterpretationForVerificationRequest(item.ProcedureStepRef));
                    });

                folderSystem.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Send/To be Reported", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
    public class CancelReportingStepTool : WorkflowItemTool
    {
        public CancelReportingStepTool()
            : base("CancelReportingStep")
        {
        }

        protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem)
        {
            try
            {
                if (desktopWindow.ShowMessageBox("This action will discard the existing report. Continue?", MessageBoxActions.OkCancel)
                    == DialogBoxAction.Cancel)
                    return false;


                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CancelReportingStep(new CancelReportingStepRequest(item.ProcedureStepRef));
                    });

                // no point in invalidating "to be reported" folder because its communal

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Verify", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Verify", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.VerifyReport)]
    [ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.VerifiedFolder.DropHandlerExtensionPoint))]
    public class VerifyTool : WorkflowItemTool
    {
        public VerifyTool()
            : base("Verify")
        {
        }

        public override bool Enabled
        {
            get
            {
                return this.Context.GetWorkflowOperationEnablement("CompleteInterpretationAndVerify") ||
                    this.Context.GetWorkflowOperationEnablement("CompleteVerification");
            }
        }

        public override bool CanAcceptDrop(IDropContext dropContext, ICollection<ReportingWorklistItem> items)
        {
            IReportingWorkflowFolderDropContext ctxt = (IReportingWorkflowFolderDropContext)dropContext;
            return ctxt.GetOperationEnablement("CompleteInterpretationAndVerify") ||
                ctxt.GetOperationEnablement("CompleteVerification");
        }

        protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem)
        {
            try
            {
                if (item.ProcedureStepName == "Interpretation")
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteInterpretationAndVerify(new CompleteInterpretationAndVerifyRequest(item.ProcedureStepRef));
                        });
                }
                else if (item.ProcedureStepName == "Verification")
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteVerification(new CompleteVerificationRequest(item.ProcedureStepRef));
                        });
                }
                else // (item.StepType == "Transcription")
                {
                    // Not defined
                }

                folderSystem.InvalidateFolder(typeof(Folders.VerifiedFolder));

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Add Addendum", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Add Addendum", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
    public class AddendumTool : WorkflowItemTool
    {
        public AddendumTool()
            : base("CreateAddendum")
        {
        }

        protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem)
        {
            if (ActivateIfAlreadyOpen(item))
                return true;

            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        CreateAddendumResponse response = service.CreateAddendum(new CreateAddendumRequest(item.ProcedureStepRef));
                        item.ProcedureStepRef = response.PublicationStepRef;
                    });

                folderSystem.InvalidateFolder(typeof(Folders.DraftFolder));

                OpenReportEditor(item);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }

            return true;
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Publish (testing)", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Publish (testing)", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
    public class PublishReportTool : WorkflowItemTool
    {
        public PublishReportTool()
            : base("PublishReport")
        {
        }

        protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem)
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.PublishReport(new PublishReportRequest(item.ProcedureStepRef));
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }

            return true;
        }
    }
}

