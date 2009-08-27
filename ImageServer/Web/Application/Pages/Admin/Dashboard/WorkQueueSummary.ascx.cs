using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    public partial class WorkQueueSummary : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WorkQueueController controller = new WorkQueueController();
            WorkQueueDataList.DataSource = controller.GetWorkQueueOverview();
            DataBind();
        }

        protected void Item_DataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                WorkQueueInfo info = e.Item.DataItem as WorkQueueInfo;
                LinkButton button = e.Item.FindControl("WorkQueueLink") as LinkButton;
                button.PostBackUrl = ImageServerConstants.PageURLs.WorkQueuePage + "?ProcessorID=" + info.Server;
            }
        }
    }
}