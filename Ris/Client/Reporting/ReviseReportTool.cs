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

namespace ClearCanvas.Ris.Client.Reporting
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Revise Report", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Revise Report", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolSmall.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
    public class ReviseReportTool : WorkflowItemTool
    {

        public ReviseReportTool()
            : base("ReviseReport")
        {
        }

        public override bool Enabled
        {
            get
            {
                return this.Context.GetWorkflowOperationEnablement("ReviseResidentReport") ||
                    this.Context.GetWorkflowOperationEnablement("ReviseUnpublishedReport");
            }
        }

        public override bool CanAcceptDrop(IDropContext dropContext, ICollection<ReportingWorklistItem> items)
        {
            IReportingWorkflowFolderDropContext ctxt = (IReportingWorkflowFolderDropContext)dropContext;

            return ctxt.GetOperationEnablement("ReviseResidentReport") ||
                    ctxt.GetOperationEnablement("ReviseUnpublishedReport");
        }

        protected override bool Execute(ReportingWorklistItem item, IDesktopWindow desktopWindow, ReportingWorkflowFolderSystemBase folderSystem)
        {
            // check if the document is already open
            if(ActivateIfAlreadyOpen(item))
                return true;

            try
            {

                if(this.Context.GetWorkflowOperationEnablement("ReviseResidentReport"))
                {
                    // note: updating only the ProcedureStepRef is hacky - the service should return an updated item
                    item.ProcedureStepRef = ReviseResidentReport(item);
                }
                else if (this.Context.GetWorkflowOperationEnablement("ReviseUnpublishedReport"))
                {
                    // note: updating only the ProcedureStepRef is hacky - the service should return an updated item
                    item.ProcedureStepRef = ReviseUnpublishedReport(item);
                }

                OpenReportEditor(item);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }

            return true;
        }

        private EntityRef ReviseResidentReport(ReportingWorklistItem item)
        {
            EntityRef result = null;
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    ReviseResidentReportResponse response = service.ReviseResidentReport(new ReviseResidentReportRequest(item.ProcedureStepRef));
                    result = response.InterpretationStepRef;
                });

            return result;
        }

        private EntityRef ReviseUnpublishedReport(ReportingWorklistItem item)
        {
            EntityRef result = null;
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    ReviseUnpublishedReportResponse response = service.ReviseUnpublishedReport(new ReviseUnpublishedReportRequest(item.ProcedureStepRef));
                    result = response.VerificationStepRef;
                });

            return result;
        }
    }
}
