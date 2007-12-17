#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
