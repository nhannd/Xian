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
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    class LanguageAlert : PatientProfileAlertBase
    {
        private class LanguageAlertNotification : AlertNotification
        {
            public LanguageAlertNotification()
                : base("Patient may not speak English", "High", "Language Alert")
            {
            }
        }

        public override IAlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
            LanguageAlertNotification alertNotification = new LanguageAlertNotification();
            SpokenLanguageEnumTable languageEnumTable = context.GetBroker<ISpokenLanguageEnumBroker>().Load();

            if (profile.PrimaryLanguage != SpokenLanguage.en)
            {
                alertNotification.Reasons.Add(languageEnumTable[profile.PrimaryLanguage].Value);
                return alertNotification;
            }

            return null;
        }
    }
}
