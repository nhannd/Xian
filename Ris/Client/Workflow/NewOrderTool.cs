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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
    [MenuAction("neworder", "folderexplorer-items-contextmenu/New Order", "NewOrder")]
    [ButtonAction("neworder", "folderexplorer-items-toolbar/New Order", "NewOrder")]
    [ButtonAction("neworder", "patientsearch-items-toolbar/New Order", "NewOrder")]
    [MenuAction("neworder", "patientsearch-items-contextmenu/New Order", "NewOrder")]
    [IconSet("neworder", IconScheme.Colour, "NewOrderSmall.png", "NewOrderMedium.png", "NewOrderLarge.png")]
    [EnabledStateObserver("neworder", "Enabled", "EnabledChanged")]
    [ActionPermission("neworder", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Order.Create)]

    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
    public class NewOrderTool : Tool<IToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

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
                    this.Enabled = (context.SelectedProfile != null && context.SelectedProfile.PatientProfileRef != null);
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
                RegistrationWorklistItemSummary item = CollectionUtils.FirstElement(context.SelectedItems);
                string title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
                NewOrder(item.PatientRef, item.PatientProfileRef, title, context.DesktopWindow);
            }
            else if (this.Context is IPatientSearchToolContext)
            {
                IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                string title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(context.SelectedProfile.Name), MrnFormat.Format(context.SelectedProfile.Mrn));
                NewOrder(context.SelectedProfile.PatientRef, context.SelectedProfile.PatientProfileRef, title, context.DesktopWindow);
            }
            else if (this.Context is IPatientBiographyToolContext)
            {
                IPatientBiographyToolContext context = (IPatientBiographyToolContext)this.ContextBase;
                string title = string.Format(SR.TitleNewOrder, PersonNameFormat.Format(context.PatientProfile.Name), MrnFormat.Format(context.PatientProfile.Mrn));
                NewOrder(context.PatientRef, context.PatientProfileRef, title, context.DesktopWindow);
            }
        }

        private void NewOrder(EntityRef patientRef, EntityRef profileRef, string title, IDesktopWindow desktopWindow)
        {
            try
            {
				OrderEditorComponent component = new OrderEditorComponent(new PatientProfileSummary { PatientRef = patientRef, PatientProfileRef = profileRef });
                IWorkspace workspace = ApplicationComponent.LaunchAsWorkspace(
                    desktopWindow,
                    component,
                    title);

                workspace.Closed += delegate
                    {
                        if (component.ExitCode == ApplicationComponentExitCode.Accepted)
                        {
                            DocumentManager.InvalidateFolder(typeof(Folders.Registration.ScheduledFolder));
                        }
                    };
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
            }
        }
    }
}
