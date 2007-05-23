using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionPoint()]
    public class PatientProfileAlertExtensionPoint : ExtensionPoint<IPatientProfileAlert>
    {
    }

    public interface IPatientProfileAlert : IAlert<PatientProfile>
    {
    }
}
