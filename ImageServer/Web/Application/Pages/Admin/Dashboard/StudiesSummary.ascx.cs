#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    public partial class StudiesSummary : System.Web.UI.UserControl
    {
        private IList<ServerPartition> _partitions;

        protected void Page_Load(object sender, EventArgs e)
        {
            ServerPartitionConfigController partitionController = new ServerPartitionConfigController();
            IList<ServerPartition> partitions = partitionController.GetAllPartitions();

            long totalStudies = 0;
            foreach(ServerPartition partition in partitions)
            {
                totalStudies += partition.StudyCount;
            }
            TotalStudiesLabel.Text = totalStudies.ToString("N0");

            _partitions = partitions;
            StudiesDataList.DataSource = _partitions;
            DataBind();
        }

        protected void Item_DataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ServerPartition partition = e.Item.DataItem as ServerPartition;
                LinkButton button = e.Item.FindControl("PartitionLink") as LinkButton;
                button.PostBackUrl = ImageServerConstants.PageURLs.StudiesPage + "?AETitle=" + partition.AeTitle;
            }
        }
    }
}