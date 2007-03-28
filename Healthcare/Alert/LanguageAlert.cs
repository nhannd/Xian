using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionOf(typeof(AlertExtensionPoint))]
    class LanguageAlert : Alert
    {
        private class LanguageAlertNotification : AlertNotification
        {
            public LanguageAlertNotification()
                : base("Patient may not speak English", "High", "Language Alert")
            {
            }
        }

        public LanguageAlert()
            : base(typeof(Patient), new LanguageAlertNotification())
        {
        }
    }
}
