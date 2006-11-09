using System;

namespace ClearCanvas.Healthcare
{
    public class PatientReconciliationException : Exception
    {
        public PatientReconciliationException(String message) : base("Cannot reconcile patients: " + message)
        {
        }
    }
}
