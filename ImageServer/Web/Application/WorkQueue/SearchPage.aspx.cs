using System;
using System.Collections.Generic;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue
{
    /// <summary>
    /// WorkQueue Search Page
    /// </summary>
    public partial class SearchPage : System.Web.UI.Page
    {
        #region Private members

        // Map between the server partition and the panel
        private readonly IDictionary<ServerEntityKey, SearchPanel> _mapSearchPanel =
            new Dictionary<ServerEntityKey, SearchPanel>();

        private readonly ServerPartitionConfigController _partitionController = new ServerPartitionConfigController();

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
                                           {
                                               SearchPanel panel =
                                                   LoadControl("SearchPanel.ascx") as SearchPanel;
                                               panel.ServerPartition = partition;
                                               panel.ID = "SearchPanel_" + partition.AeTitle;

                                               panel.EnclosingPage = this;

                                               return panel;
                                           });
        }

        #endregion Protected Methods
    }
}
