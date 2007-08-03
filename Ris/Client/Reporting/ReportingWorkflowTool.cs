using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ReportingWorkflowTool
    {
        public abstract class WorkflowItemTool : Tool<IReportingWorkflowItemToolContext>, IDropHandler<ReportingWorklistItem>
        {
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
                add { this.Context.SelectedItemsChanged += value; }
                remove { this.Context.SelectedItemsChanged -= value; }
            }

            public virtual void Apply()
            {
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(this.Context.SelectedItems);
                bool success = Execute(item, this.Context.DesktopWindow, this.Context.SelectedFolder, this.Context.Folders);
                if (success)
                {
                    this.Context.SelectedFolder.Refresh();
                }
            }

            protected string OperationName
            {
                get { return _operationName; }
            }

            protected abstract bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders);

            #region IDropHandler<ReportingWorklistItem> Members

            public virtual bool CanAcceptDrop(IDropContext dropContext, ICollection<ReportingWorklistItem> items)
            {
                IReportingWorkflowFolderDropContext ctxt = (IReportingWorkflowFolderDropContext)dropContext;
                return ctxt.GetOperationEnablement(this.OperationName);
            }

            public virtual bool ProcessDrop(IDropContext dropContext, ICollection<ReportingWorklistItem> items)
            {
                IReportingWorkflowFolderDropContext ctxt = (IReportingWorkflowFolderDropContext)dropContext;
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(items);
                bool success = Execute(item, ctxt.DesktopWindow, ctxt.FolderSystem.SelectedFolder, ctxt.FolderSystem.Folders);
                if (success)
                {
                    ctxt.FolderSystem.SelectedFolder.Refresh();
                    return true;
                }
                return false;
            }

            #endregion
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Claim")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Claim")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class ClaimInterpretationTool : WorkflowItemTool
        {
            public ClaimInterpretationTool()
                : base("ClaimInterpretation")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.ClaimInterpretation(new ClaimInterpretationRequest(item.ProcedureStepRef));
                        });

                    IFolder myInterpretationFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.DraftFolder; });
                    myInterpretationFolder.RefreshCount();

                    return true;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Edit Report")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Edit Report")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolSmall.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [LabelValueObserver("apply", "Label", "LabelChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
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
                    if (this.Context.GetWorkflowOperationEnablement("StartInterpretation") ||
                        this.Context.GetWorkflowOperationEnablement("StartVerification"))
                        return SR.TitleEditReport;
                    else
                        return SR.TitleCreateReport;
                }
            }

            public event EventHandler LabelChanged
            {
                add { this.Context.SelectedItemsChanged += value; }
                remove { this.Context.SelectedItemsChanged -= value; }
            }

            public override bool Enabled
            {
                get
                {
                    return this.Context.GetWorkflowOperationEnablement("ClaimInterpretation") ||
                        this.Context.GetWorkflowOperationEnablement("StartInterpretation") ||
                        this.Context.GetWorkflowOperationEnablement("StartVerification");
                }
            }

            public override bool CanAcceptDrop(IDropContext dropContext, ICollection<ReportingWorklistItem> items)
            {
                IReportingWorkflowFolderDropContext ctxt = (IReportingWorkflowFolderDropContext)dropContext;
                ReportingWorklistItem firstItem = CollectionUtils.FirstElement<ReportingWorklistItem>(items);

                return firstItem.StepType == "Interpretation" &&
                    (ctxt.GetOperationEnablement("ClaimInterpretation") ||
                    ctxt.GetOperationEnablement("StartInterpretation") ||
                    ctxt.GetOperationEnablement("StartVerification"));
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders)
            {
                Document doc = DocumentManager.Get(item.ProcedureStepRef);
                if (doc != null)
                {
                    doc.Activate();
                }
                else
                {
                    try
                    {
                        if (item.StepType == "Interpretation")
                        {
                            Platform.GetService<IReportingWorkflowService>(
                                delegate(IReportingWorkflowService service)
                                {
                                    StartInterpretationResponse response = service.StartInterpretation(new StartInterpretationRequest(item.ProcedureStepRef));
                                    item.ProcedureStepRef = response.InterpretationStepRef;
                                });
                        }
                        else if (item.StepType == "Verification")
                        {
                            Platform.GetService<IReportingWorkflowService>(
                                delegate(IReportingWorkflowService service)
                                {
                                    StartVerificationResponse response = service.StartVerification(new StartVerificationRequest(item.ProcedureStepRef));
                                    item.ProcedureStepRef = response.VerificationStepRef;
                                });

                        }
                        else // (item.StepType == "Transcription")
                        {
                            // Not defined
                        }

                        //bool readOnly = selectedFolder is Folders.InTranscriptionFolder ||
                        //    selectedFolder is Folders.VerifiedFolder;

                        doc = new ReportDocument(item.ProcedureStepRef, item.PersonNameDetail, folders, desktopWindow);
                        doc.Closed += delegate(object sender, EventArgs e)
                        {
                            if (selectedFolder.IsOpen)
                                selectedFolder.Refresh();
                            else
                                selectedFolder.RefreshCount();
                        };                       
                        doc.Open();

                        IViewerIntegration viewerIntegration = (IViewerIntegration)(new ViewerIntegrationExtensionPoint()).CreateExtension();
                        if (viewerIntegration != null)
                            viewerIntegration.OpenStudy(item.AccessionNumber);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, desktopWindow);
                        return false;
                    }
                }

                return true;
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Send to Transcription")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Send to Transcription")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        [ExtensionOf(typeof(Folders.InTranscriptionFolder.DropHandlerExtensionPoint))]
        public class CompleteInterpretationForTranscriptionTool : WorkflowItemTool
        {
            public CompleteInterpretationForTranscriptionTool()
                : base("CompleteInterpretationForTranscription")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteInterpretationForTranscription(new CompleteInterpretationForTranscriptionRequest(item.ProcedureStepRef));
                        });

                    IFolder myTranscriptionFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.InTranscriptionFolder; });
                    myTranscriptionFolder.RefreshCount();

                    return true;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/To be Verified")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/To be Verified")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        [ExtensionOf(typeof(Folders.ToBeVerifiedFolder.DropHandlerExtensionPoint))]
        public class CompleteInterpretationForVerificationTool : WorkflowItemTool
        {
            public CompleteInterpretationForVerificationTool()
                : base("CompleteInterpretationForVerification")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteInterpretationForVerification(new CompleteInterpretationForVerificationRequest(item.ProcedureStepRef));
                        });

                    IFolder myVerificationFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.ToBeVerifiedFolder; });
                    myVerificationFolder.RefreshCount();

                    return true;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel Step")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Cancel Step")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class CancelReportingStepTool : WorkflowItemTool
        {
            public CancelReportingStepTool()
                : base("CancelReportingStep")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CancelReportingStep(new CancelReportingStepRequest(item.ProcedureStepRef));
                        });

                    IFolder toBeReportedFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.ToBeReportedFolder; });
                    toBeReportedFolder.Refresh();

                    return true;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Verify")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Verify")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
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

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders)
            {
                try
                {
                    if (item.StepType == "Interpretation")
                    {
                        Platform.GetService<IReportingWorkflowService>(
                            delegate(IReportingWorkflowService service)
                            {
                                service.CompleteInterpretationAndVerify(new CompleteInterpretationAndVerifyRequest(item.ProcedureStepRef));
                            });
                    }
                    else if (item.StepType == "Verification")
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

                    IFolder myVerifiedFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.VerifiedFolder; });
                    myVerifiedFolder.RefreshCount();

                    return true;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Add Addendum")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Add Addendum")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class AddendumTool : WorkflowItemTool
        {
            public AddendumTool()
                : base("CreateAddendum")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders)
            {
                Document doc = DocumentManager.Get(item.ProcedureStepRef);
                if (doc != null)
                {
                    doc.Activate();
                }
                else
                {
                    try
                    {
                        CreateAddendumResponse response = null;

                        Platform.GetService<IReportingWorkflowService>(
                            delegate(IReportingWorkflowService service)
                            {
                                response = service.CreateAddendum(new CreateAddendumRequest(item.ProcedureStepRef));
                                item.ProcedureStepRef = response.VerificationStepRef;
                            });

                        IFolder myInterpretationFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                            delegate(IFolder f) { return f is Folders.DraftFolder; });
                        myInterpretationFolder.RefreshCount();

                        doc = new ReportDocument(response.InterpretationStepRef, item.PersonNameDetail, folders, desktopWindow);
                        doc.Open();
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, desktopWindow);
                        return false;
                    }
                }

                return true;
            }
        }

    }
}

