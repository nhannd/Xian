#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    public partial class WorkQueueAlertContextDataView : System.Web.UI.UserControl, IAlertPopupView
    {
        protected AlertSummary Alert { get; set; }

        public void SetAlert(AlertSummary alert)
        {
            Alert = alert;
        }
    }
}