using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;

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
    public class PatientOpenFromWorklistTool : PatientWorklistTool
    {
        public override void Initialize()
        {
            base.Initialize();

            this.Context.DefaultAction = View;
        }

        public void View()
        {
            OpenPatient(this.Context.SelectedPatientProfile, this.Context.DesktopWindow);
        }

        protected void OpenPatient(PatientProfile profile, IDesktopWindow window)
        {
            Document doc = DocumentManager.Get(profile.OID);
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
