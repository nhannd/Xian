using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    class IncompleteDemographicDataAlert : PatientProfileAlertBase
    {
        private class IncompleteDemographicDataAlertNotification : AlertNotification
        {
            public IncompleteDemographicDataAlertNotification()
                : base("Patient has incomplete demographic data", "High", "Incomplete demographic data alert")
            {
            }
        }

        private IDictionary<string, ISpecification> _specs;

        public IncompleteDemographicDataAlert()
        {
            LoadSpecifications();
        }

        public override IAlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
            IncompleteDemographicDataAlertNotification alertNotification = new IncompleteDemographicDataAlertNotification();
            foreach (KeyValuePair<string, ISpecification> kvp in _specs)
            {
                TestResult result = kvp.Value.Test(profile);
                if (result.Success == false)
                {
                    List<string> failureMessages = new List<string>();
                    ExtractFailureMessage(result.Reason, failureMessages);
                    alertNotification.Reasons.AddRange(failureMessages);
                }
            }

            if (alertNotification.Reasons.Count > 0)
                return alertNotification;

            return null;
        }

        #region Private Helpers

        private void LoadSpecifications()
        {
            ResourceResolver rr = new ResourceResolver(this.GetType().Assembly);
            string resourceName = string.Format("{0}.cfg.xml", this.GetType().Name);
            try
            {
                using (Stream xmlStream = rr.OpenResource(resourceName))
                {
                    SpecificationFactory specFactory = new SpecificationFactory(xmlStream);
                    _specs = specFactory.GetAllSpecifications();
                }
            }
            catch (Exception)
            {
                // no cfg file for this component
                _specs = new Dictionary<string, ISpecification>();
            }
        }

        private void ExtractFailureMessage(TestResultReason reason, List<string> failureMessages)
        {
            if (reason == null)
                return;

            if (reason.Message != null && reason.Message.Length > 0)
                failureMessages.Add(reason.Message);

            ExtractFailureMessage(reason.Reason, failureMessages);
        }

        #endregion

    }
}
