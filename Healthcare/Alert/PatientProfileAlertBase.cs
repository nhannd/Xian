using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alert
{
    public abstract class PatientProfileAlertBase : IPatientProfileAlert
    {
        protected PatientProfileAlertBase()
        {
        }

        #region IPatientProfileAlert Members

        public string Name
        {
            get { return "PatientProfileAlert"; }
        }

        public virtual IAlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
            return null;
        }

        #endregion
    }

    public class PatientProfileAlertHelper
    {
        private static PatientProfileAlertHelper _instance;
        private IList<IPatientProfileAlert> _patientProfileAlertTests;

        public static PatientProfileAlertHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PatientProfileAlertHelper();

                return _instance;
            }
        }

        private PatientProfileAlertHelper()
        {
            PatientProfileAlertExtensionPoint xp = new PatientProfileAlertExtensionPoint();
            object[] tests = xp.CreateExtensions();

            _patientProfileAlertTests = new List<IPatientProfileAlert>();
            foreach (object o in tests)
            {
                _patientProfileAlertTests.Add((IPatientProfileAlert)o);
            }
        }

        public IList<IPatientProfileAlert> GetAlertTests()
        {
            return _patientProfileAlertTests;
        }
    }
}
