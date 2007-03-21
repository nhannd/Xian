using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    [ExtensionPoint]
    public class PatientReconciliationStrategyExtensionPoint : ExtensionPoint<IPatientReconciliationStrategy>
    {
    }
}
