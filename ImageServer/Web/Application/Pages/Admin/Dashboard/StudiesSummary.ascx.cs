using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    public partial class StudiesSummary : System.Web.UI.UserControl
    {
        private IList<ServerPartition> _partitions;

        protected void Page_Load(object sender, EventArgs e)
        {
            ServerPartitionConfigController partitionController = new ServerPartitionConfigController();
            IList<ServerPartition> partitions = partitionController.GetAllPartitions();

            double totalStudies = 0;
            foreach(ServerPartition partition in partitions)
            {
                totalStudies += partition.StudyCount;   
            }
            TotalStudiesLabel.Text = totalStudies.ToString("n").Replace(".00", "");

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