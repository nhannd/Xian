using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/Patient/Reconcile")]
    [ButtonAction("apply", "global-toolbars/Patient/Reconcile")]
    [MenuAction("apply", "folderexplorer-items-toolbar/Reconcile")]
    [Tooltip("apply", "Reconcile patient profiles")]
    [IconSet("apply", IconScheme.Colour, "Icons.PatientReconciliationToolSmall.png", "Icons.PatientReconciliationToolMedium.png", "Icons.PatientReconciliationToolLarge.png")]
    [ClickHandler("apply", "Reconcile")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ActionPermission("apply", AuthorityTokens.ReconcilePatients)]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class PatientReconcileTool : ToolBase
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

        public void Reconcile()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                ShowReconciliationDialog(item.PatientProfileRef, context.DesktopWindow);
            }
            else
            {
                IPatientOverviewToolContext context = (IPatientOverviewToolContext)this.ContextBase;
                ShowReconciliationDialog(context.PatientProfile, context.DesktopWindow);
            }
        }

        private void ShowReconciliationDialog(EntityRef patientProfile, IDesktopWindow desktopWindow)
        {
            PatientReconciliation.ShowReconciliationDialog(patientProfile, desktopWindow);
        }
    }
}
