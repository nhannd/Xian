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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void DeleteAlert(ServerEntityKey itemKey)
        {
            if (itemKey != null)
            {
                DeleteAlertDialog.AlertItemKey = itemKey;
                DeleteAlertDialog.Show();
            }
        }

        public void DeleteAllAlerts()
        {
            DeleteAllAlertsDialog.Show();
        }
    }
}
