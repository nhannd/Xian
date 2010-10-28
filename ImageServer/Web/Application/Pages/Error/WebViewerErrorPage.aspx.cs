#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Error
{
    public partial class WebViewerErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Context.Items[ImageServerConstants.ContextKeys.ErrorMessage] != null) {
                ErrorMessageLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorMessage].ToString();
            } 
            if (Context.Items[ImageServerConstants.ContextKeys.StackTrace] != null)
            {
                StackTraceTextBox.Text = Server.HtmlEncode(Context.Items[ImageServerConstants.ContextKeys.StackTrace].ToString());
                StackTraceTextBox.Visible = true;
                StackTraceMessage.Visible = true;
            }
            if (Context.Items[ImageServerConstants.ContextKeys.ErrorDescription] != null)
            {
                DescriptionLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorDescription].ToString();
            }

            SetPageTitle(App_GlobalResources.Titles.ErrorPageTitle);
        }

        protected void Logout_Click(Object sender, EventArgs e)
        {
            SessionManager.SignOut();           
            Response.Redirect(ImageServerConstants.PageURLs.LoginPage);
        }

    }
}
