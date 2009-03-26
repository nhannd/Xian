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
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Error
{
    public partial class AuthorizationErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Context.Items[ImageServerConstants.ContextKeys.ErrorMessage] != null) {
                ErrorMessageLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorMessage].ToString();
            } 
            if (Context.Items[ImageServerConstants.ContextKeys.StackTrace] != null)
            {
                StackTraceTextBox.Text = Context.Items[ImageServerConstants.ContextKeys.StackTrace].ToString();
                StackTraceTextBox.Visible = true;
                StackTraceMessage.Visible = true;
            }
            if (Context.Items[ImageServerConstants.ContextKeys.ErrorDescription] != null)
            {
                DescriptionLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorDescription].ToString();
            }

            Page.Title = App_GlobalResources.Titles.AuthorizationErrorPageTitle;
        }

        protected void Logout_Click(Object sender, EventArgs e)
        {
            SessionManager.TerminateSession();
            Response.Redirect("~/Pages/Login/Default.aspx");
        }
    }
}
