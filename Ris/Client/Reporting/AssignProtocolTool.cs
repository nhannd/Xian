using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    public abstract class ReportingProtocolTool : Tool<IReportingWorkflowItemToolContext>
    {
        abstract protected bool ClaimProtocol { get; }

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
                        doc = new ProtocolEditorComponentDocument(item.AccessionNumber, item, this.ClaimProtocol, this.Context.DesktopWindow);
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
                if (item != null)
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


    [MenuAction("apply", "folderexplorer-items-contextmenu/Assign Protocol", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Assign Protocol", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class AssignProtocolTool : ReportingProtocolTool
    {
        protected override bool ClaimProtocol
        {
            get { return true; }
        }

        public override bool Enabled
        {
            get
            {
                // todo: only enabled for to be protocolled folder
                return base.Enabled;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Edit Protocol", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Edit Protocol", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.ProtocolEditorToolSmall.png", "Icons.ProtocolEditorToolMedium.png", "Icons.ProtocolEditorToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class EditProtocolTool : ReportingProtocolTool
    {
        protected override bool ClaimProtocol
        {
            get { return false; }
        }

        public override bool Enabled
        {
            get
            {
                // todo: only enabled for draft folder
                return base.Enabled;
            }
        }
    }
}
