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
    [MenuAction("add", "Admin/Patient/New...")]
    [ButtonAction("add", "PatientAdminToolbar/AddPatient")]
    [Tooltip("add", "Add new Patient")]
    [ClickHandler("add", "AddPatient")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientAddTool : PatientAddEditToolBase
    {
        public void AddPatient()
        {
            OpenPatient("New Patient", Patient.New());
        }

        protected override void SaveChanges(Patient patient)
        {
            IPatientAdminService service = ApplicationContext.GetService<IPatientAdminService>();
            service.AddNewPatient(patient);
        }
    }
}
