using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Replace Order", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Replace Order", "Apply")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(RegistrationMainWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(TechnologistMainWorkflowItemToolExtensionPoint))]
    public class ReplaceOrderTool : Tool<IWorkflowItemToolContext>
    {
        public event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }

        public bool Enabled
        {
            get { return this.Context.Selection.Items.Length > 0; }
        }

        public void Apply()
        {
            try
            {
                WorklistItemSummaryBase item = (WorklistItemSummaryBase)this.Context.Selection.Item;
                ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    new OrderEntryComponent(item.PatientRef, item.OrderRef, OrderEntryComponent.Mode.ReplaceOrder),
                    string.Format(SR.TitleReplaceOrder, PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn)));
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }
    }
}
