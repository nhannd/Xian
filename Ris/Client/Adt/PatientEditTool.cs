using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("edit1", "global-menus/Patient/Edit Patient")]
    [ButtonAction("edit1", "global-toolbars/Patient/EditPatient")]
    [ClickHandler("edit1", "Apply")]
    [EnabledStateObserver("edit1", "Enabled", "EnabledChanged")]
    [Tooltip("edit1", "Edit Patient Information")]
    [IconSet("edit1", IconScheme.Colour, "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png")]
    [ActionPermission("edit1", AuthorityTokens.PatientProfileAdmin)]

    [ButtonAction("edit3", "folderexplorer-items-toolbar/Edit")]
    [ClickHandler("edit3", "Apply")]
    [EnabledStateObserver("edit3", "Enabled", "EnabledChanged")]
    [Tooltip("edit3", "Edit Patient Information")]
    [IconSet("edit3", IconScheme.Colour, "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png")]
    [ActionPermission("edit3", AuthorityTokens.PatientProfileAdmin)]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class PatientEditTool : ToolBase
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
                _enabled = true;
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
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                Edit(item.PatientProfileRef, context.DesktopWindow);
            }
            else
            {
                IPatientOverviewToolContext context = (IPatientOverviewToolContext)this.ContextBase;
                Edit(context.PatientProfile, context.DesktopWindow);
            }
        }

        private void Edit(EntityRef profileRef, IDesktopWindow desktopWindow)
        {
            ApplicationComponent.LaunchAsDialog(
                desktopWindow,
                new PatientProfileEditorComponent(profileRef),
                SR.TitleEditPatient);
        }
    }
}
