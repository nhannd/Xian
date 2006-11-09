using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("view1", "global-menus/Patient/View Details...")]
    [ButtonAction("view1", "global-toolbars/Patient/ViewPatient")]
    [ClickHandler("view1", "View")]
    [EnabledStateObserver("view1", "Enabled", "EnabledChanged")]
    [Tooltip("view1", "Open patient details")]
    [IconSet("view1", IconScheme.Colour, "PatientOpenToolSmall.png", "PatientOpenToolMedium.png", "PatientOpenToolLarge.png")]

    [MenuAction("view2", "worklist-contextmenu/View Details")]
    [ButtonAction("view2", "worklist-toolbar/Details")]
    [ClickHandler("view2", "View")]
    [EnabledStateObserver("view2", "Enabled", "EnabledChanged")]
    [Tooltip("view2", "Open patient details")]
    [IconSet("view2", IconScheme.Colour, "OpenItemSmall.png", "OpenItemMedium.png", "OpenItemLarge.png")]

    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class PatientOverviewTool : Tool<IWorklistToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        
        public override void Initialize()
        {
            base.Initialize();
            this.Context.DefaultAction = View;
            this.Context.SelectedPatientProfileChanged += delegate(object sender, EventArgs args)
            {
                this.Enabled = (this.Context.SelectedPatientProfile != null);
            };
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

        public void View()
        {
            OpenPatient(this.Context.SelectedPatientProfile, this.Context.DesktopWindow);
        }

        protected void OpenPatient(EntityRef<PatientProfile> profile, IDesktopWindow window)
        {
            Document doc = DocumentManager.Get(profile.ToString());
            if (doc == null)
            {
                doc = new PatientOverviewDocument(profile, window);
                doc.Open();
            }
            else
            {
                doc.Activate();
            }
        }
    }
}
