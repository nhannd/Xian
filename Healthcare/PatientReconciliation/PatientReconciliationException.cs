using System;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    public class PatientReconciliationException : HealthcareWorkflowException
    {
        public PatientReconciliationException(String message) : base("Cannot reconcile patients: " + message)
        {
        }
    }
}
