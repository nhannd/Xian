#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Error
{
    public partial class JavascriptRequired : BasePage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(App_GlobalResources.Titles.JavascriptRequiredPageTitle);
        }
    }
}
