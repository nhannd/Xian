#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Permissions;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.ServiceScheduling)]
    public partial class Default : BasePage
    {
        #region Private members

        #endregion Private members

        #region Protected methods


        void ServiceLockPanel_ServiceLockUpdated(ServiceLock serviceLock)
        {
            DataBind();
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ServiceLockPanel.ServiceLockUpdated += ServiceLockPanel_ServiceLockUpdated;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();

            SetPageTitle(App_GlobalResources.Titles.ServiceSchedulingPageTitle);           
        }

        #endregion  Protected methods

        #region Public Methods


        #endregion
    }
}
