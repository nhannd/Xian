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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Test
{
    public partial class TestSeriesDelete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(ServerAE.Text);

            using(IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                List<string> series = new List<string>(SeriesUID.Text.Split(','));
                StudyEditorHelper.DeleteSeries(ctx, partition, StudyUID.Text, series, "Testing");
                ctx.Commit();
            }
        }
    }
}
