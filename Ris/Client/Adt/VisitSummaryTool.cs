using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/VisitSummaryTool")]
    [ButtonAction("apply", "global-toolbars/ToolbarMyTools/VisitSummaryTool")]
    [Tooltip("apply", "Place tooltip text here")]
    [IconSet("apply", IconScheme.Colour, "Icons.VisitSummaryToolSmall.png", "Icons.VisitSummaryToolMedium.png", "Icons.VisitSummaryToolLarge.png")]
    [ClickHandler("apply", "ShowVisits")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class VisitSummaryTool : ToolBase
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


        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void ShowVisits()
        {
            if (this.ContextBase is IWorklistToolContext)
            {
                IWorklistToolContext context = (IWorklistToolContext)this.ContextBase;
                ShowVisitSummaryDialog(context.SelectedPatientProfile, context.DesktopWindow);
            }
            else
            {
                IPatientOverviewToolContext context = (IPatientOverviewToolContext)this.ContextBase;
                ShowVisitSummaryDialog(context.PatientProfile, context.DesktopWindow);
            }
        }

        private void ShowVisitSummaryDialog(EntityRef<PatientProfile> patientProfileRef, IDesktopWindow iDesktopWindow)
        {
            VisitSummaryComponent component = new VisitSummaryComponent(patientProfileRef);
            ApplicationComponent.LaunchAsWorkspace(
                iDesktopWindow,
                component,
                SR.TitlePatientVisits,
                null);
        }
    }
}
