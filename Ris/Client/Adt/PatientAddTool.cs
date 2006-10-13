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
    [MenuAction("apply", "global-menus/Adt/New Patient...")]
    [ButtonAction("apply", "global-toolbars/Adt/PatientAddTool")]
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
            PatientEditorShellComponent editor = new PatientEditorShellComponent(PatientProfile.New(), true);
            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow, editor, "New Patient", PatientEditorExited);
        }

        private void PatientEditorExited(IApplicationComponent component)
        {
            PatientEditorShellComponent editor = (PatientEditorShellComponent)component;
            if (editor.ExitCode == ApplicationComponentExitCode.Normal)
            {
                IAdtService service = ApplicationContext.GetService<IAdtService>();
                service.CreatePatientForProfile(editor.Subject);
            }
        }

    }
}
