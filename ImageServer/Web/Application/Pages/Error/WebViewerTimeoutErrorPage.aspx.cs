#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Error
{
    public partial class WebViewerTimeoutErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(App_GlobalResources.Titles.TimeoutErrorPageTitle);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            SessionManager.SignOut();
        }
    }
}
