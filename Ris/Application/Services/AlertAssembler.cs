#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alerts;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Application.Services
{
    public class AlertAssembler
    {
        public AlertNotificationDetail CreateAlertNotification(AlertNotification alertNotification)
        {
            return new AlertNotificationDetail(
                alertNotification.AlertId,
                new List<string>(alertNotification.Reasons));
        }
    }
}
