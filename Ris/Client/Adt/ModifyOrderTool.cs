using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Modify Order", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Modify Order", "Apply")]
    [IconSet("apply", IconScheme.Colour, "EditToolSmall.png", "EditToolMedium.png", "EditToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(RegistrationMainWorkflowItemToolExtensionPoint))]
    public class ModifyOrderTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            _enabled = false;   // disable by default

            this.Context.SelectedItemsChanged += delegate
            {
                // optimistically enable this tool for any selected order, regardless of statuss
                this.Enabled = this.Context.SelectedItems != null
                    && this.Context.SelectedItems.Count == 1;
            };
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        public void Apply()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement(Context.SelectedItems);
            string title = string.Format("Modify Order - {0} {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
            try
            {
                ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    new OrderEntryComponent(item.PatientRef, item.OrderRef, OrderEntryComponent.Mode.ModifyOrder),
                    title,
                    delegate(IApplicationComponent c)
                    {
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }
    }
}
