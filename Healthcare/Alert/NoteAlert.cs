using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionOf(typeof(PatientAlertExtensionPoint))]
    public class NoteAlert : PatientAlertBase
    {
        private class NoteAlertNotification : AlertNotification
        {
            public NoteAlertNotification()
                : base ("Patient contains high severity notes", "High", "Note Alert")
            {
            }
        }

        public override IAlertNotification Test(Patient patient, IPersistenceContext context)
        {
            NoteAlertNotification alertNotification = new NoteAlertNotification();

            foreach (Note note in patient.Notes)
            {
                if (note.Category.Severity == NoteSeverity.H)
                    alertNotification.Reasons.Add(note.Category.Name);
            }

            if (alertNotification.Reasons.Count > 0)
                return alertNotification;

            return null;
        }
    }
}
