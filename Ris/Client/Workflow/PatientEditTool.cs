#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
    [ButtonAction("edit", "folderexplorer-items-toolbar/Edit Patient", "Apply")]
    [MenuAction("edit", "folderexplorer-items-contextmenu/Edit Patient", "Apply")]
    [ButtonAction("edit", "patientsearch-items-toolbar/Edit Patient", "Apply")]
    [MenuAction("edit", "patientsearch-items-contextmenu/Edit Patient", "Apply")]
    [EnabledStateObserver("edit", "Enabled", "EnabledChanged")]
    [Tooltip("edit", "Edit Patient Information")]
    [IconSet("edit", IconScheme.Colour, "Icons.EditPatientToolSmall.png", "Icons.EditPatientToolMedium.png", "Icons.EditPatientToolLarge.png")]
    [ActionPermission("edit", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Update)]
	[ActionPermission("edit", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientProfile.Update)]

    [ExtensionOf(typeof(PatientBiographyToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
    public class PatientEditTool : ToolBase
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            _enabled = false;   // disable by default

            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                    && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else if (this.ContextBase is IPatientSearchToolContext)
            {
                ((IPatientSearchToolContext)this.ContextBase).SelectedProfileChanged += delegate
                {
                    IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                    this.Enabled = (context.SelectedProfile != null && context.SelectedProfile.PatientProfileRef != null);
                };
            }
            else if (this.ContextBase is IPatientBiographyToolContext)
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
        
        public void Apply()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItemSummary item = CollectionUtils.FirstElement(context.SelectedItems);
                if (Edit(item.PatientProfileRef, context.DesktopWindow))
                {
                    context.InvalidateSelectedFolder();
                }
            }
            else if (this.ContextBase is IPatientSearchToolContext)
            {
                IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                Edit(context.SelectedProfile.PatientProfileRef, context.DesktopWindow);
            }
            else if (this.ContextBase is IPatientBiographyToolContext)
            {
                IPatientBiographyToolContext context = (IPatientBiographyToolContext)this.ContextBase;
                Edit(context.PatientProfileRef, context.DesktopWindow);
            }
        }

        private bool Edit(EntityRef profileRef, IDesktopWindow desktopWindow)
        {
            try
            {
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    desktopWindow,
                    new PatientProfileEditorComponent(profileRef),
                    SR.TitleEditPatient);

                return exitCode == ApplicationComponentExitCode.Accepted;

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }
}
