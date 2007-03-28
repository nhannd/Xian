using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionOf(typeof(AlertExtensionPoint))]
    public class NoteAlert : Alert
    {
        private class NoteAlertNotification : AlertNotification
        {
            public NoteAlertNotification()
                : base ("Note alert representation", "High", "Note Alert")
            {
            }
        }

        public NoteAlert()
            : base(typeof(Patient), new NoteAlertNotification())
        {
        }
    }
}
