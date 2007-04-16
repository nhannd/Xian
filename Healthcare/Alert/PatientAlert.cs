using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alert
{
    public abstract class PatientAlert : IPatientAlert
    {
        protected PatientAlert()
        {
        }

        #region IPatientAlert Members

        public string Name
        {
            get { return "PatientAlert"; }
        }

        public virtual IAlertNotification Test(Patient patient, IPersistenceContext context)
        {
            return null;
        }

        #endregion
    }
}
