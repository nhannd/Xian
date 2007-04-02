using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionPoint()]
    public class PatientAlertExtensionPoint : ExtensionPoint<IPatientAlert>
    {
    }

    public interface IPatientAlert : IAlert<Patient>
    {
    }
}
