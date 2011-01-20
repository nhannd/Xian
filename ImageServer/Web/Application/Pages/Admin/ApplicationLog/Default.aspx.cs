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
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.ApplicationLog.Search)]
	public partial class Default : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            SetPageTitle(App_GlobalResources.Titles.ApplicationLogPageTitle);
		}
	}
}
