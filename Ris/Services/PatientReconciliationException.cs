using System;

namespace ClearCanvas.Ris.Services
{
    public class PatientReconciliationException : Exception
    {
        public PatientReconciliationException(String message) : base("Cannot reconcile patients: " + message)
        {
        }
    }
}
