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
            if (profile.PrimaryLanguage != null && profile.PrimaryLanguage.Code != "en")
            {
                SpokenLanguageEnum language = context.GetBroker<IEnumBroker>().Lookup<SpokenLanguageEnum>(profile.PrimaryLanguage.ToString());
                alertNotification.Reasons.Add(language.Value);
                return alertNotification;
            }

            return null;
        }
    }
}
