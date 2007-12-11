using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Protocol", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Protocol", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ReportingProtocolWorkflowItemToolExtensionPoint))]
    public class ReportingProtocolTool : Tool<IReportingWorkflowItemToolContext>
    {
        public void Apply()
        {
            try
            {
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(this.Context.SelectedItems);
                OpenItem(item);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }

        private void OpenItem(ReportingWorklistItem item)
        {
            if (item == null)
                return;

            Workspace workspace = DocumentManager.Get<ProtocolEditorComponentDocument>(item.OrderRef, this.Context.DesktopWindow);
            if (workspace == null)
            {
                ProtocolEditorComponentDocument protocolEditorComponentDocument = new ProtocolEditorComponentDocument(item, this.Context);
                protocolEditorComponentDocument.Open();
            }
            else
            {
                workspace.Activate();
            }
        }

        public bool Enabled
        {
            get
            {
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(this.Context.SelectedItems);
                return item != null && item.ProcedureStepName == "Protocol";
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }
    }
}
