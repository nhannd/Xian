using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/Patient/New Patient...")]
    [ButtonAction("apply", "global-toolbars/Patient/PatientAddTool")]
    [Tooltip("apply", "New Patient")]
    [IconSet("apply", IconScheme.Colour, "Icons.PatientAddToolSmall.png", "Icons.PatientAddToolMedium.png", "Icons.PatientAddToolLarge.png")]
    [ClickHandler("apply", "Apply")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientAddTool : Tool<IDesktopToolContext>
    {
        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public PatientAddTool()
        {
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            PatientProfileEditorComponent editor = new PatientProfileEditorComponent();
            ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(
                this.Context.DesktopWindow,
                editor,
                "New Patient");

            if (result == ApplicationComponentExitCode.Normal)
            {
                // open the patient overview for the newly created patient
                Document doc = new PatientOverviewDocument(editor.PatientProfile, this.Context.DesktopWindow);
                doc.Open();
            }
        }
    }
}
