using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    public partial class WorkQueueAlertContextDataView : System.Web.UI.UserControl
    {
        private AlertSummary _alert;

        public AlertSummary Alert
        {
            get { return _alert; }
            set { _alert = value; }
        }
    }
}