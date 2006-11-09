using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/Patient/Reconcile")]
    [ButtonAction("apply", "global-toolbars/Patient/Reconcile")]
    [MenuAction("apply", "worklist-contextmenu/Reconcile")]
    [ButtonAction("apply", "worklist-toolbar/Reconcile")]
    [Tooltip("apply", "Reconcile patient profiles")]
    [IconSet("apply", IconScheme.Colour, "Icons.PatientReconciliationToolSmall.png", "Icons.PatientReconciliationToolMedium.png", "Icons.PatientReconciliationToolLarge.png")]
    [ClickHandler("apply", "Reconcile")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class PatientReconcileTool : ToolBase
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            if (this.ContextBase is IWorklistToolContext)
            {
                _enabled = false;   // disable by default

                ((IWorklistToolContext)this.ContextBase).SelectedPatientProfileChanged += delegate(object sender, EventArgs args)
                {
                    this.Enabled = ((IWorklistToolContext)this.ContextBase).SelectedPatientProfile != null;
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

        public void Reconcile()
        {
            if (this.ContextBase is IWorklistToolContext)
            {
                IWorklistToolContext context = (IWorklistToolContext)this.ContextBase;
                ShowReconciliationDialog(context.SelectedPatientProfile, context.DesktopWindow);
            }
            else
            {
                IPatientOverviewToolContext context = (IPatientOverviewToolContext)this.ContextBase;
                ShowReconciliationDialog(context.PatientProfile, context.DesktopWindow);
            }
        }

        private void ShowReconciliationDialog(EntityRef<PatientProfile> profile, IDesktopWindow desktopWindow)
        {
            PatientReconciliation.ShowReconciliationDialog(profile, desktopWindow);
        }
    }
}
