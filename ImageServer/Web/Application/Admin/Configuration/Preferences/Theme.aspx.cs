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
using ClearCanvas.ImageServer.Web.Application.Common;

namespace ClearCanvas.ImageServer.Web.Application
{
    public partial class Theme : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
                return;
            if (Page.Theme != null && Themes.Items.FindByValue(Page.Theme)!=null)
            {
                Themes.Items.FindByValue(Page.Theme).Selected = true;
            }
                
        }

        protected void ThemeSelectedIndexChanged(
            object sender, EventArgs e)
        {
            // Create a cookie to hold the selected theme.
            HttpCookie SelTheme;
            SelTheme = new HttpCookie("Theme");

            // Get the selected theme.
            SelTheme.Value = Themes.SelectedValue;
            SelTheme.Expires = DateTime.Now.AddMonths(1);

            // Save the cookie.
            if (Response.Cookies["Theme"] == null)
                Response.Cookies.Add(SelTheme);
            else
            {
                Response.Cookies.Remove("Theme");
                Response.Cookies.Add(SelTheme);
            }

            Response.Redirect(Request.Url.AbsolutePath);
        }
    }
}
