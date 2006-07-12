using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Admin
{
//    [MenuAction("patientAdmin", "Admin/Patients")]
//    [ClickHandler("patientAdmin", "LaunchPatientAdmin")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientAdminLaunchTool : Tool
    {
        public void LaunchPatientAdmin()
        {
            ApplicationComponent.LaunchAsWorkspace(new PatientAdminComponent(), "Patient Admin", null);
        }
    }
}
