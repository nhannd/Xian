using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.PatientReconciliation;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    public class ReconciliationAlert : PatientProfileAlertBase
    {
        private class ReconciliationAlertNotification : AlertNotification
        {
            public ReconciliationAlertNotification()
                : base ("Patient has unreconciled records", "High", "Reconciliation Alert")
            {
            }
        }

        public override IAlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
            ReconciliationAlertNotification alertNotification = new ReconciliationAlertNotification();
            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();

            IList<PatientProfileMatch> matches = strategy.FindReconciliationMatches(profile, context);
            if (matches.Count > 0)
            {
                return alertNotification;
            }

            return null;
        }
    }
}
