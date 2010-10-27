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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.RestoreQueue.Search)]
    public partial class Default : BasePage
    {        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
                                                           {
                                                               SearchPanel panel =
                                                                   LoadControl("SearchPanel.ascx") as SearchPanel;
                                                               panel.ServerPartition = partition;
                                                               panel.ID = "SearchPanel_" + partition.AeTitle;
                                                               return panel;
                                                           });

			SetPageTitle(App_GlobalResources.Titles.RestoreQueuePageTitle);
        }
    }
}
