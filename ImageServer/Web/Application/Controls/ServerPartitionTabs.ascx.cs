#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class ServerPartitionTabs : UserControl
    {
        #region Delegates

        public delegate UserControl GetTabPanel(ServerPartition partition);

        #endregion

        #region Private members

        // Map between the server partition and the panel
        private readonly IDictionary<ServerEntityKey, Control> _mapPanel =
            new Dictionary<ServerEntityKey, Control>();

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

        #region Public Methods

        public void UpdateCurrentPartition()
        {
            Update(true);
        }

        public ServerEntityKey GetCurrentPartitionKey()
        {
            ServerPartition partition = GetCurrentPartition();

            return partition != null ? partition.Key : null;
        }

        public ServerPartition GetCurrentPartition()
        {
            if (_partitionList == null || _partitionList.Count == 0)
                return null;

            ServerPartition partition = _partitionList[PartitionTabContainer.ActiveTabIndex];

            return partition;
        }

        public Control GetUserControlForCurrentPartition()
        {
            return GetUserControlForPartition(GetCurrentPartitionKey());
        }

        public Control GetUserControlForPartition(ServerEntityKey key)
        {
            if (_mapPanel.ContainsKey(key))
                return _mapPanel[key];

            return null;
        }

        public void SetupLoadPartitionTabs(GetTabPanel tabDelegate)
        {
            if(ServerPartitionList == null || ServerPartitionList.Count == 0)
            {
                return;
            }
            
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
                    // create a panel for the control 
                    UserControl panel = tabDelegate(part);

                    // wrap an updatepanel around the tab
                    UpdatePanel updatePanel = new UpdatePanel();
                    updatePanel.ContentTemplateContainer.Controls.Add(panel);

                    // put the panel into a lookup table to be used later
                    _mapPanel[part.GetKey()] = updatePanel;


                    // Add the device panel into the tab
                    tabPanel.Controls.Add(updatePanel);
                }

                // Add the tab into the tabstrip
                PartitionTabContainer.Tabs.Add(tabPanel);
            }

            if (ServerPartitionList != null && ServerPartitionList.Count > 0)
            {
                PartitionTabContainer.ActiveTabIndex = 0;
            }
            else
            {
                PartitionTabContainer.ActiveTabIndex = -1;
            }
        }

        /// <summary>
        /// Update the specified partition tab
        /// </summary>
        /// <param name="key">The server partition key</param>
        /// <remarks>
        /// 
        /// </remarks>
        public void Update(ServerEntityKey key)
        {
            Control ctrl = GetUserControlForPartition(key);
            if (ctrl != null)
            {
                ctrl.DataBind();

                if (ctrl is UpdatePanel)
                {
                    UpdatePanel panel = ctrl as UpdatePanel;
                    if (panel.UpdateMode == UpdatePanelUpdateMode.Conditional)
                    {
                        panel.Update();
                    }
                }
            }

        }

        /// <summary>
        /// Update the specified partition tab
        /// </summary>
        /// <param name="tabIndex">The server partition tab index</param>
        /// <remarks>
        /// 
        /// </remarks>
        public void Update(int tabIndex)
        {
            if (_partitionList == null || _partitionList.Count == 0)
                return;

            ServerPartition partition = _partitionList[tabIndex];

            Update(partition.GetKey());
        }

        public void Update(bool current)
        {
            if (_partitionList == null || _partitionList.Count == 0)
                return;

            if(current)
            {
                Update(PartitionTabContainer.ActiveTabIndex);
            } else
            {
                for (int i = 0; i < PartitionTabContainer.Tabs.Count; i++)
                {
                    Update(i);
                }
            }
        }

        #endregion Public Methods
    }
}