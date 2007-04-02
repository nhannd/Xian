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
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Application.Services
{
    public class AlertAssembler
    {
        public AlertNotificationDetail CreateAlertNotification(IAlertNotification alertNotification)
        {
            AlertNotificationDetail detail = new AlertNotificationDetail();
            detail.Representation = alertNotification.Representation;
            detail.Severity = alertNotification.Severity;
            detail.Type = alertNotification.Type;
            detail.Reasons = alertNotification.Reasons;

            return detail;
        }
    }
}
