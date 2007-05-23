using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alert
{
    public abstract class PatientAlertBase : IPatientAlert
    {
        protected PatientAlertBase()
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

    public class PatientAlertHelper
    {
        private static PatientAlertHelper _instance;
        private IList<IPatientAlert> _patientAlertTests;

        public static PatientAlertHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PatientAlertHelper();

                return _instance;
            }
        }

        private PatientAlertHelper()
        {
            PatientAlertExtensionPoint xp = new PatientAlertExtensionPoint();
            object[] tests = xp.CreateExtensions();

            _patientAlertTests = new List<IPatientAlert>();
            foreach (object o in tests)
            {
                _patientAlertTests.Add((IPatientAlert)o);
            }
        }

        public IList<IPatientAlert> GetAlertTests()
        {
            return _patientAlertTests;
        }
    }
}
