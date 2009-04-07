using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Alert.View)]
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = App_GlobalResources.Titles.AlertsPageTitle;
        }

        public void DeleteAlert(ServerEntityKey itemKey)
        {
            if (itemKey != null)
            {
                DeleteAlertDialog.AlertItemKey = itemKey;
                DeleteAlertDialog.Show(false);
            }
        }

        public void DeleteAllAlerts()
        {
            DeleteAllAlertsDialog.Show(true);
        }

        public void UpdateUI()
        {
            AlertsPanel.UpdateUI();
        }
    }
}
