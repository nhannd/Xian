using System;
using System.Collections.Generic;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SearchPage : System.Web.UI.Page
    {
        #region Private members

        // Map between the server partition and the panel
        private readonly IDictionary<ServerEntityKey, SearchPanel> _mapSearchPanel =
            new Dictionary<ServerEntityKey, SearchPanel>();

        private readonly ServerPartitionConfigController _partitionController = new ServerPartitionConfigController();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void SetupLoadPartitionTabs()
        {
            int n = 0;

            SearchPartitionTabContainer.Tabs.Clear();
            IList<ServerPartition> partitions = _partitionController.GetAllPartitions();
            foreach (ServerPartition part in partitions)
            {
                n++;

                // create a tab
                TabPanel tabPannel = new TabPanel();
                tabPannel.HeaderText = part.AeTitle;
                tabPannel.ID = "Tab_" + n;

                // create a device panel
                SearchPanel searchPanel = LoadControl("SearchPanel.ascx") as SearchPanel;
                searchPanel.Partition = part;
                searchPanel.ID = "SearchPanel_" + n;

                // put the panel into a lookup table to be used later
                _mapSearchPanel[part.GetKey()] = searchPanel;

                // Add the device panel into the tab
                tabPannel.Controls.Add(searchPanel);

                // Add the tab into the tabstrip
                SearchPartitionTabContainer.Tabs.Add(tabPannel);
            }

            if (partitions.Count > 0)
                SearchPartitionTabContainer.ActiveTabIndex = 0;
            else
            {
                SearchPartitionTabContainer.ActiveTabIndex = -1;
                Label1.Text = "Please add a server partition first";
                Label1.Visible = true;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.SetupLoadPartitionTabs();
        }
    }
}
