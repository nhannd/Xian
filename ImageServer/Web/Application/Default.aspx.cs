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
using ClearCanvas.ImageServer.Web.Common;

namespace ClearCanvas.ImageServer.Web.Application
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string defaultHomePageUrl = UserProfile.GetDefaultUrl();
            if (defaultHomePageUrl == null)
            {
				// The user isn't logged in
            	Response.End();
            }
            else
            {
                Response.Redirect(defaultHomePageUrl);
            }
        }
    }
}
