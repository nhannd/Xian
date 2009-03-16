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
using ClearCanvas.ImageServer.Web.Application.Pages.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.Admin.ApplicationLog.Search)]
	public partial class Default : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            Page.Title = App_GlobalResources.Titles.ApplicationLogPageTitle;
		}
	}
}
