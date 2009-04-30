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
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.js")]
    /// <summary>
    /// Server parition panel  used in <seealso cref="ServerPartitionPage"/> web page.
    /// </summary>
    public partial class ServerPartitionPanel : AJAXScriptControl
    {
        #region Private Members

        // list of partitions displayed in the list
        private IList<ServerPartition> _partitions = new List<ServerPartition>();
        // used for database interaction
        private ServerPartitionConfigController _theController;
        private Default _enclosingPage;

        #endregion Private Members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeletePartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditPartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ServerPartitionListClientID")]
        public string ServerPartitionListClientID
        {
            get { return ServerPartitionGridPanel.TheGrid.ClientID; }
        }

        // Sets/Gets the list of partitions displayed in the panel
        public IList<ServerPartition> Partitions
        {
            get { return _partitions; }
            set
            {
                _partitions = value;
                ServerPartitionGridPanel.Partitions = _partitions;
            }
        }

        // Sets/Gets the controller used to retrieve load partitions.
        public ServerPartitionConfigController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

        public Default EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            if (AETitleFilter.Text.Length > 0 || DescriptionFilter.Text.Length > 0 || StatusFilter.SelectedIndex > 0)
                return true;
            else
                return false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            UpdateUI();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerPartitionSingleItem, App_GlobalResources.SR.GridPagerPartitionMultipleItems, ServerPartitionGridPanel.TheGrid, delegate { return Partitions.Count; }, ImageServerConstants.GridViewPagerPosition.top);

            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.All));
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.Enabled));
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.Disabled));

        }

        public override void DataBind()
        {
            LoadData();
            base.DataBind();
        }


        protected void Clear()
        {
            AETitleFilter.Text = string.Empty;
            DescriptionFilter.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
            FolderFilter.Text = string.Empty;
        }

        protected void LoadData()
        {
            ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();

            if (String.IsNullOrEmpty(AETitleFilter.Text) == false)
            {
                string key = AETitleFilter.Text.Replace("*", "%");
                criteria.AeTitle.Like(key + "%");
            }

            if (String.IsNullOrEmpty(DescriptionFilter.Text) == false)
            {
                string key = DescriptionFilter.Text.Replace("*", "%");
                criteria.Description.Like(key + "%");
            }

            if (String.IsNullOrEmpty(FolderFilter.Text) == false)
            {
                string key = FolderFilter.Text.Replace("*", "%");
                criteria.PartitionFolder.Like(key + "%");
            }

            if (StatusFilter.SelectedIndex != 0)
            {
                if (StatusFilter.SelectedIndex == 1)
                    criteria.Enabled.EqualTo(true);
                else
                    criteria.Enabled.EqualTo(false);
            }

        	criteria.AeTitle.SortAsc(0);

            Partitions =
                _theController.GetPartitions(criteria);
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void AddPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.AddPartition();
        }

        protected void EditPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            ServerPartition selectedPartition =
                ServerPartitionGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                EnclosingPage.EditPartition(selectedPartition);
            }
        }

        protected void DeletePartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            ServerPartition selectedPartition =
                ServerPartitionGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                EnclosingPage.DeletePartition(selectedPartition);
            }
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            // refresh the list
            Clear();
            LoadData();
            UpdateUI();
        }

        #endregion Protected Methods

        #region Public Methods

        public void UpdateUI()
        {
            LoadData();
            ServerPartitionGridPanel.UpdateUI();

            ServerPartition partition = ServerPartitionGridPanel.SelectedPartition;
            EditPartitionButton.Enabled = partition != null;
            DeletePartitionButton.Enabled = (partition != null && _theController.CanDelete(partition));

            SearchUpdatePanel.Update();
        }

        #endregion Public methods
       
    }
}