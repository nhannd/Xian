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
    [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class ProtocolEditorTool : Tool<IReportingWorkflowItemToolContext>
    {
        public void Apply()
        {
            try
            {
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(this.Context.SelectedItems);

                if (item != null)
                {
                    Document doc = DocumentManager.Get(item.AccessionNumber);
                    if (doc == null)
                    {
                        doc = new ProtocolEditorComponentDocument(item.AccessionNumber, item, this.Context.DesktopWindow);
                        doc.Open();
                    }
                    else
                    {
                        doc.Activate();
                    }
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }

        public virtual bool Enabled
        {
            get
            {
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(this.Context.SelectedItems);
                if(item != null)
                {
                    return item.ProcedureStepName == "Protocol";
                }
                return false;
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectedItemsChanged += value; }
            remove { this.Context.SelectedItemsChanged -= value; }
        }

    }
}
