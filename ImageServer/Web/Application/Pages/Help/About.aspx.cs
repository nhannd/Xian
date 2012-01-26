#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Help
{
    public partial class About : BasePage
    {
        public string LicenseKey { get; set; }

        public About()
        {
            try
            {
                if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Configuration.ServerPartitions))
                {
                    LicenseInformation.Reset();
                    LicenseKey = LicenseInformation.LicenseKey;
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(Titles.AboutPageTitle);
        }
    }
}
