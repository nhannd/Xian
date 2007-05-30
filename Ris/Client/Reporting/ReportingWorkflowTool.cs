using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
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

    }
}

