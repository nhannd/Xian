#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Reconcile Patients", "Reconcile")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Reconcile Patients", "Reconcile")]
    [Tooltip("apply", "Reconcile patient profiles")]
	[IconSet("apply", IconScheme.Colour, "Icons.ReconcilePatientToolSmall.png", "Icons.ReconcilePatientToolMedium.png", "Icons.ReconcilePatientToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Reconcile)]

    [ExtensionOf(typeof(PatientBiographyToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
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
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
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
                RegistrationWorklistItemSummary item = CollectionUtils.FirstElement<RegistrationWorklistItemSummary>(context.SelectedItems);
                if (ShowReconciliationDialog(item.PatientProfileRef, context.DesktopWindow))
                {
                    context.InvalidateSelectedFolder();
                }
            }
            else
            {
                IPatientBiographyToolContext context = (IPatientBiographyToolContext)this.ContextBase;
                ShowReconciliationDialog(context.PatientProfileRef, context.DesktopWindow);
            }
        }

        private bool ShowReconciliationDialog(EntityRef patientProfile, IDesktopWindow desktopWindow)
        {
            try
            {
                return PatientReconciliation.ShowReconciliationDialog(patientProfile, desktopWindow);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }
}
