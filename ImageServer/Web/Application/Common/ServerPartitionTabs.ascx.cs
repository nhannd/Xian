using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Common
{
    public partial class ServerPartitionTabs : UserControl
    {
        #region Delegates

        public delegate UserControl GetTabPanel(ServerPartition partition);

        #endregion

        #region Private members

        // Map between the server partition and the panel
        private readonly IDictionary<ServerEntityKey, UserControl> _mapPanel =
            new Dictionary<ServerEntityKey, UserControl>();

        private readonly ServerPartitionConfigController _controller = new ServerPartitionConfigController();
        private IList<ServerPartition> _partitionList;

        #endregion

        #region Public Properties

        public IList<ServerPartition> ServerPartitionList
        {
            get
            {
                if (_partitionList == null)
                    _partitionList = _controller.GetAllPartitions();

                return _partitionList;
            }
            set { _partitionList = value; }
        }

        #endregion

        public UserControl GetUserControlForPartition(ServerEntityKey key)
        {
            if (_mapPanel.ContainsKey(key))
                return _mapPanel[key];

            return null;
        }

        protected override void OnInit(EventArgs e)
        {
 
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void SetupLoadPartitionTabs(GetTabPanel tabDelegate)
        {
            int n = 0;

            this.PartitionTabContainer.Tabs.Clear();
            foreach (ServerPartition part in ServerPartitionList)
            {
                n++;

                // create a tab
                TabPanel tabPanel = new TabPanel();
                tabPanel.HeaderText = part.AeTitle;
                tabPanel.ID = "Tab_" + n;

                if (tabDelegate != null)
                {
                    // create a device panel
                    UserControl panel = tabDelegate(part);

                    // put the panel into a lookup table to be used later
                    _mapPanel[part.GetKey()] = panel;


                    // Add the device panel into the tab
                    tabPanel.Controls.Add(panel);
                }
                // Add the tab into the tabstrip
                PartitionTabContainer.Tabs.Add(tabPanel);
            }

            if (ServerPartitionList != null && ServerPartitionList.Count > 0)
                PartitionTabContainer.ActiveTabIndex = 0;
            else
            {
                PartitionTabContainer.ActiveTabIndex = -1;
            }
        }
    }
}