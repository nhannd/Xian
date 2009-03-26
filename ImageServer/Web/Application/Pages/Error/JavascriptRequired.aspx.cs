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
using ClearCanvas.ImageServer.Web.Application.Pages.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Error
{
    public partial class JavascriptRequired : BasePage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = App_GlobalResources.Titles.JavascriptRequiredPageTitle;
        }
    }
}
