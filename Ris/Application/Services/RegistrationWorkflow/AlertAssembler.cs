using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alert;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.Alert;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class AlertAssembler
    {
        public AlertNotificationDetail CreateAlertNotification(IAlertNotification alertNotification)
        {
            return new AlertNotificationDetail(
                alertNotification.Representation,
                alertNotification.Severity,
                alertNotification.Type);
        }
    }
}
