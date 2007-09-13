using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common;
using System.Collections;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Replace Order")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Replace Order")]
    [Tooltip("apply", "Replace Order")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [ClickHandler("apply", "ReplaceOrder")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    public class ReplaceOrderTool : Tool<IToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();

            _enabled = false;   // disable by default
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                context.SelectedItemsChanged += delegate(object sender, EventArgs args)
                {
                    this.Enabled = (context.SelectedItems != null && context.SelectedItems.Count == 1
                        && ((RegistrationWorklistItem)CollectionUtils.FirstElement(context.SelectedItems)).OrderRef != null);
                };
            }
            else if (this.ContextBase is ITechnologistWorkflowItemToolContext)
            {
                ITechnologistWorkflowItemToolContext context = (ITechnologistWorkflowItemToolContext)this.ContextBase;
                context.SelectedItemsChanged += delegate(object sender, EventArgs args)
                {
                    this.Enabled = (context.SelectedItems != null && context.SelectedItems.Count == 1);
                };
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            protected set
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

        public void ReplaceOrder()
        {
            string accessionNumber = "";
            IDesktopWindow desktopWindow = null;
            string title = "";

            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                accessionNumber = item.AccessionNumber;
                desktopWindow = context.DesktopWindow;
                title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(item.Name), MrnFormat.Format(item.Mrn));
            }
            else if (this.ContextBase is ITechnologistWorkflowItemToolContext)
            {
                ITechnologistWorkflowItemToolContext context = (ITechnologistWorkflowItemToolContext)this.ContextBase;
                ModalityWorklistItem item = CollectionUtils.FirstElement<ModalityWorklistItem>(context.SelectedItems);
                accessionNumber = item.AccessionNumber;
                desktopWindow = context.DesktopWindow;
                title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(item.PersonNameDetail), MrnFormat.Format(item.Mrn));
            }
            else
            {
                return;  // wrong context
            }

            OrderDetail existingOrder = null;
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    LoadOrderDetailResponse response = service.LoadOrderDetail(new LoadOrderDetailRequest(accessionNumber));
                    existingOrder = response.OrderDetail;
                });

            if (desktopWindow.ShowMessageBox(SR.MessageReplaceOrder, MessageBoxActions.OkCancel) == DialogBoxAction.Ok)
            {
                ApplicationComponent.LaunchAsWorkspace(
                    desktopWindow,
                    new OrderEntryComponent(existingOrder),
                    title,
                    delegate(IApplicationComponent c)
                    {
                        if (c.ExitCode == ApplicationComponentExitCode.Normal)
                        {
                            // todo: refresh folder entry and/or counts
                        }
                    });
            }

        }
    }
}
