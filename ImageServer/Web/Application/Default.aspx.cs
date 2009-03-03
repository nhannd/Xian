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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //TODO: Use a mapping file similar to SiteMap to specify the default home page based on the authority tokens that user has.
            if (SessionManager.Current == null)
            {
                FormsAuthentication.RedirectToLoginPage();  // once user has logged in, he/she will be redirected to somewhere else based on the roles  
            }
                
            if (SessionManager.Current.User.IsInRole(AuthorityTokens.Admin.System.EnterpriseConfiguration))
            {
                Response.Redirect(ImageServerConstants.PageURLs.AdminUserPage);
            }
            else
            {
                Response.Redirect(ImageServerConstants.PageURLs.SearchPage);
            }
        }
    }
}
