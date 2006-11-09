using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("edit1", "global-menus/Patient/Edit Patient")]
    [ButtonAction("edit1", "global-toolbars/Patient/EditPatient")]
    [ClickHandler("edit1", "Apply")]
    [EnabledStateObserver("edit1", "Enabled", "EnabledChanged")]
    [Tooltip("edit1", "Edit Patient Information")]
    [IconSet("edit1", IconScheme.Colour, "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png")]

    [MenuAction("edit2", "worklist-contextmenu/Edit Patient")]
    [ButtonAction("edit2", "worklist-toolbar/Edit")]
    [ClickHandler("edit2", "Apply")]
    [EnabledStateObserver("edit2", "Enabled", "EnabledChanged")]
    [Tooltip("edit2", "Edit Patient Information")]
    [IconSet("edit2", IconScheme.Colour, "Icons.Edit.png", "Icons.Edit.png", "Icons.Edit.png")]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class PatientEditTool : ToolBase
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
            if (this.ContextBase is IWorklistToolContext)
            {
                IWorklistToolContext context = (IWorklistToolContext)this.ContextBase;
                Edit(context.SelectedPatientProfile, context.DesktopWindow);
            }
            else
            {
                IPatientOverviewToolContext context = (IPatientOverviewToolContext)this.ContextBase;
                Edit(context.PatientProfile, context.DesktopWindow);
            }
        }

        private void Edit(EntityRef<PatientProfile> profileRef, IDesktopWindow desktopWindow)
        {
            ApplicationComponent.LaunchAsDialog(
                desktopWindow,
                new PatientProfileEditorComponent(profileRef),
                "Edit Patient");
        }
    }
}
