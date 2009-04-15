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
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string defaultHomePageUrl = UserProfile.GetDefaultUrl();
            if (defaultHomePageUrl != null)
            {
                Response.Redirect(ImageServerConstants.PageURLs.SearchPage);
            }
            else
            {
                Response.Redirect(defaultHomePageUrl);
            }
        }
    }
}
