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

        public override IAlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
            IncompleteDemographicDataAlertNotification alertNotification = new IncompleteDemographicDataAlertNotification();

            IDictionary<string, ISpecification> specs;

            try
            {
                IncompleteDemographicDataAlertSettings settings = new IncompleteDemographicDataAlertSettings();
                using (TextReader xml = new StringReader(settings.ValidationRules))
                {
                    SpecificationFactory specFactory = new SpecificationFactory(xml);
                    specs = specFactory.GetAllSpecifications();
                }
            }
            catch (Exception)
            {
                // no cfg file for this component
                specs = new Dictionary<string, ISpecification>();
            }
            
            foreach (KeyValuePair<string, ISpecification> kvp in specs)
            {
                TestResult result = kvp.Value.Test(profile);
                if (result.Success == false)
                {
                    List<string> failureMessages = new List<string>();
                    ExtractFailureMessage(result.Reasons, failureMessages);
                    alertNotification.Reasons.AddRange(failureMessages);
                }
            }

            if (alertNotification.Reasons.Count > 0)
                return alertNotification;

            return null;
        }

        #region Private Helpers

        private void ExtractFailureMessage(TestResultReason[] reasons, List<string> failureMessages)
        {
            foreach (TestResultReason reason in reasons)
            {
                if (!string.IsNullOrEmpty(reason.Message))
                    failureMessages.Add(reason.Message);

                ExtractFailureMessage(reason.Reasons, failureMessages);
            }
        }

        #endregion

    }
}
