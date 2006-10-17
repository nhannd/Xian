using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("edit1", "global-menus/Patient/Edit Patient...")]
    [ButtonAction("edit1", "global-toolbars/Patient/EditPatient")]
    [ClickHandler("edit1", "Edit")]
    [EnabledStateObserver("edit1", "Enabled", "EnabledChanged")]
    [Tooltip("edit1", "Edit Patient Information")]
    [IconSet("edit1", IconScheme.Colour, "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png")]

    [MenuAction("edit2", "worklist-contextmenu/Edit Patient")]
    [ButtonAction("edit2", "worklist-toolbar/Edit")]
    [ClickHandler("edit2", "Edit")]
    [EnabledStateObserver("edit2", "Enabled", "EnabledChanged")]
    [Tooltip("edit2", "Edit Patient Information")]
    [IconSet("edit2", IconScheme.Colour, "Icons.Edit.png", "Icons.Edit.png", "Icons.Edit.png")]


    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class PatientEditFromWorklistTool : PatientWorklistTool
    {
        public void Edit()
        {
            PatientProfile profile = this.Context.SelectedPatientProfile;
            ApplicationComponent.LaunchAsDialog(
                this.Context.DesktopWindow,
                new PatientProfileEditorComponent(profile),
                string.Format(SR.PatientComponentTitle, profile.Name.Format(), profile.MRN.Format()));
        }
    }
}
