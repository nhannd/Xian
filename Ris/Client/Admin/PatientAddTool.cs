using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("add", "global-menus/Admin/Patient/New...")]
    [ButtonAction("add", "global-toolbars/PatientAdminToolbar/AddPatient")]
    [Tooltip("add", "Add new Patient")]
    [IconSet("add", IconScheme.Colour, "Icons.AddPatientMedium.png", "Icons.AddPatientMedium.png", "Icons.AddPatientMedium.png")]
    [ClickHandler("add", "AddPatient")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientAddTool : PatientAddEditToolBase
    {
        public void AddPatient()
        {
            OpenPatient("New Patient", PatientProfile.New());
        }

        protected override IDesktopWindow DesktopWindow
        {
            get
            {
                return ((IDesktopToolContext)this.ContextBase).DesktopWindow;
            }
        }

        protected override void EditorClosed(PatientProfile patient, ApplicationComponentExitCode exitCode)
        {
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                IPatientAdminService service = ApplicationContext.GetService<IPatientAdminService>();
                service.AddNewPatient(patient);
            }
        }
    }
}
