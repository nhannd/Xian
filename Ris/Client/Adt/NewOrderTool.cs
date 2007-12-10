using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("neworder", "folderexplorer-items-contextmenu/New Order")]
    [ButtonAction("neworder", "folderexplorer-items-toolbar/New Order")]
    [MenuAction("neworder", "global-menus/Orders/New")]
    [ButtonAction("neworder", "patientsearch-items-toolbar/New Order")]
    [MenuAction("neworder", "patientsearch-items-contextmenu/New Order")]
    [IconSet("neworder", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("neworder", "Enabled", "EnabledChanged")]
    [ClickHandler("neworder", "NewOrder")]
    [ExtensionOf(typeof(RegistrationMainWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationBookingWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
    public class NewOrderTool : Tool<IToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        private Workspace _orderEntryWorkspace;

        public override void Initialize()
        {
            base.Initialize();
            _enabled = false;   // disable by default

            if (this.Context is IRegistrationWorkflowItemToolContext)
            {
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                    && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else if (this.Context is IPatientSearchToolContext)
            {
                ((IPatientSearchToolContext)this.ContextBase).SelectedProfileChanged += delegate
                {
                    IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                    this.Enabled = (context.SelectedProfile != null && context.SelectedProfile.ProfileRef != null);
                };
            }
            else if (this.Context is IPatientBiographyToolContext)
            {
                this.Enabled = true;
            }
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

        public void NewOrder()
        {
            if (this.Context is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement(context.SelectedItems);
                string title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
                NewOrder(item.PatientRef, title, context.DesktopWindow);
            }
            else if (this.Context is IPatientSearchToolContext)
            {
                IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                string title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(context.SelectedProfile.Name), MrnFormat.Format(context.SelectedProfile.Mrn));
                NewOrder(context.SelectedProfile.PatientRef, title, context.DesktopWindow);
            }
            else if (this.Context is IPatientBiographyToolContext)
            {
                IPatientBiographyToolContext context = (IPatientBiographyToolContext)this.ContextBase;
                string title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(context.PatientProfile.Name), MrnFormat.Format(context.PatientProfile.Mrn));
                NewOrder(context.PatientRef, title, context.DesktopWindow);
            }
        }

        private void NewOrder(EntityRef patientRef, string title, IDesktopWindow desktopWindow)
        {
            try
            {
                ApplicationComponent.LaunchAsWorkspace(
                    desktopWindow,
                    new OrderEntryComponent(patientRef),
                    title);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
            }
        }
    }
}
