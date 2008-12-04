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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Common
{
    public partial class BaseAdminPage : BasePage
    {
        protected override void OnPreInit(EventArgs e)
        {
            // TODO: Check for admin access rights
            
            base.OnPreInit(e);
        }
    }
}
