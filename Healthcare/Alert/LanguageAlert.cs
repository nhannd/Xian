using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionOf(typeof(PatientAlertExtensionPoint))]
    class LanguageAlert : PatientAlert
    {
        private class LanguageAlertNotification : AlertNotification
        {
            public LanguageAlertNotification()
                : base("Patient may not speak English", "High", "Language Alert")
            {
            }
        }

        public override IAlertNotification Test(Patient patient, IPersistenceContext context)
        {
            LanguageAlertNotification alertNotification = new LanguageAlertNotification();
            SpokenLanguageEnumTable languageEnumTable = context.GetBroker<ISpokenLanguageEnumBroker>().Load();

            List<string> languagesSpoken = new List<string>();
            foreach (PatientProfile profile in patient.Profiles)
            {
                if (profile.PrimaryLanguage != SpokenLanguage.en)
                    alertNotification.Reasons.Add(languageEnumTable[profile.PrimaryLanguage].Value);
            }

            if (alertNotification.Reasons.Count > 0)
                return alertNotification;

            return null;
        }
    }
}
