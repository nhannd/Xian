using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.Common
{
    public partial class BasePage : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            // Retrieve the theme name if present.
            if (Request.Cookies["Theme"] != null)
                Page.Theme = Request.Cookies["Theme"].Value;

            
        }

        protected override void  OnInit(EventArgs e)
        {
 	         base.OnInit(e);
            
        }
    }
}
