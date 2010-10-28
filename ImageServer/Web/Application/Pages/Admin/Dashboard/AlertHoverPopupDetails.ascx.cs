#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Services.WorkQueue;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    public partial class AlertHoverPopupDetails : System.Web.UI.UserControl
    {
        #region Private Members
        private AlertSummary _alert; 
        #endregion
        
        #region Public Properties
        public AlertSummary Alert
        {
            get { return _alert; }
            set { _alert = value; }
        } 
        #endregion

        public override void DataBind()
        {
            if (Alert!=null && Alert.ContextData!=null)
            {
                if (Alert.ContextData is WorkQueueAlertContextData)
                {
                    WorkQueueAlertContextDataView view = Page.LoadControl("WorkQueueAlertContextDataView.ascx") as WorkQueueAlertContextDataView;
                    view.Alert = this.Alert;
                    DetailsPlaceHolder.Controls.Add(view);
                }
            }
            base.DataBind();
        }
    }
}