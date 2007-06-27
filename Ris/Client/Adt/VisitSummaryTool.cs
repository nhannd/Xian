using System;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/Patient/Visit Summary")]
    [ButtonAction("apply", "global-toolbars/Patient/Visit Summary")]
    [Tooltip("apply", "Visit Summary")]
    [IconSet("apply", IconScheme.Colour, "Icons.VisitSummaryToolSmall.png", "Icons.VisitSummaryToolMedium.png", "Icons.VisitSummaryToolLarge.png")]
    [ClickHandler("apply", "ShowVisits")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class VisitSummaryTool : ToolBase
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                _enabled = false;   // disable by default
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItemsChanged += delegate(object sender, EventArgs args)
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                        && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else
            {
                _enabled = true;    // always enabled
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


        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void ShowVisits()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                ShowVisitSummaryDialog(item.PatientProfileRef, context.DesktopWindow);
            }
            else
            {
                IPatientOverviewToolContext context = (IPatientOverviewToolContext)this.ContextBase;
                ShowVisitSummaryDialog(context.PatientProfile, context.DesktopWindow);
            }
        }

        private void ShowVisitSummaryDialog(EntityRef patientProfileRef, IDesktopWindow desktopWindow)
        {
            try
            {
                VisitSummaryComponent component = new VisitSummaryComponent(patientProfileRef);
                ApplicationComponent.LaunchAsWorkspace(
                    desktopWindow,
                    component,
                    SR.TitlePatientVisits,
                    null);

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
            }
        }
    }
}
