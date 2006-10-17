using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("edit", "global-menus/Patient/Edit Patient...")]
    [ButtonAction("edit", "global-toolbars/Patient/EditPatient")]
    [Tooltip("edit", "Edit Patient")]
    [IconSet("edit", IconScheme.Colour, "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png", "Icons.PatientEditToolMedium.png")]
    [ClickHandler("edit", "Edit")]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    public class PatientEditFromOverviewTool : Tool<IPatientOverviewToolContext>
    {
        public void Edit()
        {
            PatientProfile profile = this.Context.SelectedProfile;
            ApplicationComponent.LaunchAsDialog(
                this.Context.DesktopWindow,
                new PatientProfileEditorComponent(profile),
                string.Format(SR.PatientComponentTitle, profile.Name.Format(), profile.MRN.Format()));
        }

    }
}
