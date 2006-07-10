using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("patientAdmin", "Admin/Patients")]
    [ClickHandler("patientAdmin", "LaunchPatientAdmin")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientAdminLaunchTool : Tool
    {
        public void LaunchPatientAdmin()
        {
            Workspace workspace = new ApplicationComponentHostWorkspace(
                "Patient Admin",
                new PatientAdminComponent(),
                new PatientAdminComponentViewExtensionPoint());

            DesktopApplication.WorkspaceManager.Workspaces.Add(workspace);
        }
    }
}
