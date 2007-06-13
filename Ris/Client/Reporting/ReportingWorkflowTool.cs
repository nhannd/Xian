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
                bool success = Execute(item, this.Context.DesktopWindow, this.Context.Folders);
                if (success)
                {
                    this.Context.SelectedFolder.Refresh();
                }
            }

            protected string OperationName
            {
                get { return _operationName; }
            }

            protected abstract bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders);

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
                bool success = Execute(item, ctxt.DesktopWindow, ctxt.FolderSystem.Folders);
                if (success)
                {
                    ctxt.FolderSystem.SelectedFolder.Refresh();
                    return true;
                }
                return false;
            }

            #endregion
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Claim Interpretation")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Claim Interpretation")]
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

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.ClaimInterpretation(new ClaimInterpretationRequest(item));
                        });

                    IFolder myInterpretationFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.MyInterpretationFolder; });
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

        [MenuAction("apply", "folderexplorer-items-contextmenu/Edit Interpretation")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Edit Interpretation")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolSmall.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class EditInterpretationTool : WorkflowItemTool
        {
            public EditInterpretationTool()
                : base("StartInterpretation")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
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
                        Platform.GetService<IReportingWorkflowService>(
                            delegate(IReportingWorkflowService service)
                            {
                                StartInterpretationResponse response = service.StartInterpretation(new StartInterpretationRequest(item));
                                item.ProcedureStepRef = response.ReportingStepRef;
                            });

                        doc = new ReportDocument(item, folders, false, this.Context.DesktopWindow);
                        doc.Closed += delegate(object sender, EventArgs e)
                        {
                            IFolder myInterpretationFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                                delegate(IFolder f) { return f is Folders.MyInterpretationFolder; });

                            if (myInterpretationFolder != null)
                            {
                                if (myInterpretationFolder.IsOpen)
                                    myInterpretationFolder.Refresh();
                                else
                                    myInterpretationFolder.RefreshCount();
                            }
                        };

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

        [MenuAction("apply", "folderexplorer-items-contextmenu/Complete for Transcription")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Complete for Transcription")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class CompleteInterpretationForTranscriptionTool : WorkflowItemTool
        {
            public CompleteInterpretationForTranscriptionTool()
                : base("CompleteInterpretationForTranscription")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteInterpretationForTranscription(new CompleteInterpretationForTranscriptionRequest(item));
                        });

                    IFolder myTranscriptionFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.MyTranscriptionFolder; });
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

        [MenuAction("apply", "folderexplorer-items-contextmenu/Complete for Verification")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Complete for Verification")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class CompleteInterpretationForVerificationTool : WorkflowItemTool
        {
            public CompleteInterpretationForVerificationTool()
                : base("CompleteInterpretationForVerification")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteInterpretationForVerification(new CompleteInterpretationForVerificationRequest(item));
                        });

                    IFolder myVerificationFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.MyVerificationFolder; });
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

        [MenuAction("apply", "folderexplorer-items-contextmenu/Complete and Verify")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Complete and Verify")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class CompleteInterpretationAndVerifyTool : WorkflowItemTool
        {
            public CompleteInterpretationAndVerifyTool()
                : base("CompleteInterpretationAndVerify")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteInterpretationAndVerify(new CompleteInterpretationAndVerifyRequest(item));
                        });

                    IFolder myVerifiedFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.MyVerifiedFolder; });
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

        [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel Transcription")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Cancel Transcription")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class CancelPendingTranscriptionTool : WorkflowItemTool
        {
            public CancelPendingTranscriptionTool()
                : base("CancelPendingTranscription")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CancelPendingTranscription(new CancelPendingTranscriptionRequest(item));
                        });

                    IFolder myTranscriptionFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.MyTranscriptionFolder; });
                    myTranscriptionFolder.Refresh();

                    return true;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Edit Verification")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Edit Verification")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.StartToolSmall.png", "Icons.StartToolMedium.png", "Icons.StartToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class EditVerificationTool : WorkflowItemTool
        {
            public EditVerificationTool()
                : base("StartVerification")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
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
                        Platform.GetService<IReportingWorkflowService>(
                            delegate(IReportingWorkflowService service)
                            {
                                StartVerificationResponse response = service.StartVerification(new StartVerificationRequest(item));
                                item.ProcedureStepRef = response.ReportingStepRef;
                            });

                        doc = new ReportDocument(item, folders, false, this.Context.DesktopWindow);
                        doc.Closed += delegate(object sender, EventArgs e)
                        {
                            IFolder myVerificationFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                                delegate(IFolder f) { return f is Folders.MyVerificationFolder; });

                            if (myVerificationFolder != null)
                            {
                                if (myVerificationFolder.IsOpen)
                                    myVerificationFolder.Refresh();
                                else
                                    myVerificationFolder.RefreshCount();
                            }
                        };

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

        [MenuAction("apply", "folderexplorer-items-contextmenu/Complete Verification")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Complete Verification")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
        public class CompleteVerificationTool : WorkflowItemTool
        {
            public CompleteVerificationTool()
                : base("CompleteVerification")
            {
            }

            protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteVerification(new CompleteVerificationRequest(item));
                        });

                    IFolder myVerifiedFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                        delegate(IFolder f) { return f is Folders.MyVerifiedFolder; });
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
    }
}

