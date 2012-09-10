#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Permissions;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using Resources;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Dashboard.View)]
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(Titles.DashboardTitle);

            switch(ServerPlatform.ServerOperatingMode)
            {
                case ServerOperatingMode.TemporaryCache:
                    OperatingModeLabel.Text = SR.ServerModeTemporaryCache;
                    break;

                case ServerOperatingMode.Archive:
                    OperatingModeLabel.Text = SR.ServerModeArchive;
                    break;

                case ServerOperatingMode.MixedMode:
                    OperatingModeLabel.Text = SR.ServerModeMixedMode
                    break;
            }
            
        }
    }
}