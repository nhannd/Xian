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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.App_GlobalResources;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.StudyIntegrityQueue.Search)]
    public partial class Default : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string patientID = string.Empty;
            string patientName = string.Empty;
            string partitionKey;
            string reason = string.Empty;
            string databind = string.Empty;
            ServerPartition activePartition = null;

            if (!IsPostBack && !Page.IsAsync)
            {
                patientID = Request["PatientID"];
                patientName = Request["PatientName"];
                partitionKey = Request["PartitionKey"];
                reason = Request["Reason"];
                databind = Request["Databind"];

                if (!string.IsNullOrEmpty(patientID) && !string.IsNullOrEmpty(patientName) &&
                    !string.IsNullOrEmpty(partitionKey))
                {
                    if (!string.IsNullOrEmpty(partitionKey))
                    {
                        var controller = new ServerPartitionConfigController();
                        activePartition = controller.GetPartition(new ServerEntityKey("ServerPartition", partitionKey));
                    }
                }
                if (string.IsNullOrEmpty(reason))
                {
                    if (!string.IsNullOrEmpty(partitionKey))
                    {
                        var controller = new ServerPartitionConfigController();
                        activePartition = controller.GetPartition(new ServerEntityKey("ServerPartition", partitionKey));
                    }
                }
            }

            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
                                                           {
                                                               var panel =
                                                                   LoadControl("SearchPanel.ascx") as SearchPanel;
                                                               if (panel != null)
                                                               {
                                                                   panel.ServerPartition = partition;
                                                                   panel.ID = "SearchPanel_" + partition.AeTitle;

                                                                   if (!string.IsNullOrEmpty(patientName) ||
                                                                       !string.IsNullOrEmpty(patientID) ||
                                                                       !string.IsNullOrEmpty(reason))
                                                                   {
                                                                       panel.PatientNameFromUrl = patientName;
                                                                       panel.PatientIdFromUrl = patientID;
                                                                       panel.ReasonFromUrl = reason;
                                                                   }

                                                                   if (!string.IsNullOrEmpty(databind))
                                                                   {
                                                                       panel.DataBindFromUrl = true;
                                                                   }
                                                               }
                                                               return panel;
                                                           });

            if (activePartition != null)
            {
                ServerPartitionTabs.SetActivePartition(activePartition.AeTitle);
            }

            SetPageTitle(Titles.StudyIntegrityQueuePageTitle);
        }

        public void OnReconcileItem(ReconcileDetails details)
        {
            if (details.StudyIntegrityQueueItem.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.Duplicate)
            {
                DuplicateSopReconcileDialog.StudyIntegrityQueueItem = details.StudyIntegrityQueueItem;
                DuplicateSopReconcileDialog.DuplicateEntryDetails = details as DuplicateEntryDetails;

                DuplicateSopReconcileDialog.DataBind();
                DuplicateSopReconcileDialog.Show();
            }
            else
            {
                ReconcileDialog.ReconcileDetails = details;
                ReconcileDialog.StudyIntegrityQueueItem = details.StudyIntegrityQueueItem;
                ReconcileDialog.Show();
            }
        }

        public void UpdateUI()
        {
            foreach (ServerPartition partition in ServerPartitionTabs.ServerPartitionList)
            {
                var panel =
                    ServerPartitionTabs.GetUserControlForPartition(partition.GetKey()).FindControl("SearchPanel_" +
                                                                                                   partition.AeTitle) as
                    SearchPanel;
                if (panel != null) panel.UpdateUI();
            }
        }
    }
}