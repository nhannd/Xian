using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Replace Order", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Replace Order", "Apply")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(RegistrationMainWorkflowItemToolExtensionPoint))]
    public class RegistrationReplaceOrderTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public event EventHandler EnabledChanged
        {
            add { this.Context.SelectedItemsChanged += value; }
            remove { this.Context.SelectedItemsChanged -= value; }
        }

        public bool Enabled
        {
            get { return this.Context.SelectedItems.Count > 0; }
        }

        public void Apply()
        {
            try
            {
                RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
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
