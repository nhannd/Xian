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
    public partial class ErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Context.Items["ERROR_MESSAGE"] != null) {
                ErrorMessageLabel.Text = Context.Items["ERROR_MESSAGE"].ToString();
            } 
            if (Context.Items["STACK_TRACE"] != null)
            {
                StackTraceTextBox.Text = Context.Items["STACK_TRACE"].ToString();
                StackTraceTextBox.Visible = true;
                StackTraceMessage.Visible = true;
            }
            if (Context.Items["ERROR_DESCRIPTION"] != null)
            {
                DescriptionLabel.Text = Context.Items["ERROR_DESCRIPTION"].ToString();
            }
        }
    }
}
