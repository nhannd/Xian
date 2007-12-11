using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
