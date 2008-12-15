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
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    /// <summary>
    /// Server parition panel  used in <seealso cref="Default"/> web page.
    /// </summary>
    public partial class PartitionArchivePanel : UserControl
    {
        #region Private Members

        // list of partitions displayed in the list
        private IList<Model.PartitionArchive> _partitionArchives = new List<Model.PartitionArchive>();
        // used for database interaction
        private PartitionArchiveConfigController _theController;
		private ServerPartition _serverPartition;

        #endregion Private Members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeletePartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("AddButtonClientID")]
        public string AddButtonClientID
        {
            get { return AddPartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string RestoreButtonClientID
        {
            get { return EditPartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("PartitionArchiveListClientID")]
        public string PartitionArchiveListClientID
        {
            get { return PartitionArchiveGridPanel.TheGrid.ClientID; }
        }

        // Sets/Gets the list of partitions displayed in the panel
        public IList<Model.PartitionArchive> PartitionArchives
        {
            get { return _partitionArchives; }
            set
            {
                _partitionArchives = value;
                PartitionArchiveGridPanel.Partitions = _partitionArchives;
            }
        }

        // Sets/Gets the controller used to retrieve load partitions.
        public PartitionArchiveConfigController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

		/// <summary>
		/// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
		/// </summary>
		public ServerPartition ServerPartition
		{
			get { return _serverPartition; }
			set { _serverPartition = value; }
		}
        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            int archiveSelectedIndex = ArchiveTypeFilter.SelectedIndex;

            ArchiveTypeFilter.Items.Clear();
            ArchiveTypeFilter.Items.Add(new ListItem(App_GlobalResources.SR.All));
            foreach (ArchiveTypeEnum archiveTypeEnum in ArchiveTypeEnum.GetAll())
            {
                ArchiveTypeFilter.Items.Add(
                    new ListItem(archiveTypeEnum.Description, archiveTypeEnum.Lookup));
            }
            ArchiveTypeFilter.SelectedIndex = archiveSelectedIndex;

            int statusSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.All, App_GlobalResources.SR.All));
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.Enabled, App_GlobalResources.SR.Enabled));
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.Disabled, App_GlobalResources.SR.Disabled));
            StatusFilter.SelectedIndex = statusSelectedIndex;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            UpdateUI();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new PartitionArchiveConfigController();

            GridPagerTop.Target = PartitionArchiveGridPanel.TheGrid;
            GridPagerTop.ItemName = App_GlobalResources.SR.GridPagerPartitionSingleItem;
            GridPagerTop.PuralItemName = App_GlobalResources.SR.GridPagerPartitionMultipleItems;

            GridPagerBottom.Target = PartitionArchiveGridPanel.TheGrid;
            GridPagerBottom.ItemName = App_GlobalResources.SR.GridPagerPartitionSingleItem;
            GridPagerBottom.PuralItemName = App_GlobalResources.SR.GridPagerPartitionMultipleItems;

            GridPagerBottom.Target = PartitionArchiveGridPanel.TheGrid;

            SetupEventHandlers();
        }

        protected void SetupEventHandlers()
        {
            GridPagerTop.GetRecordCountMethod = delegate { return PartitionArchives.Count; };
            GridPagerBottom.GetRecordCountMethod = delegate { return PartitionArchives.Count; };
        }

        public override void DataBind()
        {
            LoadData();
            base.DataBind();
        }

        protected void LoadData()
        {
            PartitionArchiveSelectCriteria criteria = new PartitionArchiveSelectCriteria();

            if (String.IsNullOrEmpty(DescriptionFilter.Text) == false)
            {
                string key = DescriptionFilter.Text.Replace("*", "%");
                criteria.Description.Like(key + "%");
            }

            if (StatusFilter.SelectedIndex > 0)
            {
                if (StatusFilter.SelectedIndex == 1)
                    criteria.Enabled.EqualTo(true);
                else
                    criteria.Enabled.EqualTo(false);
            }

        	criteria.ServerPartitionKey.EqualTo(ServerPartition.Key);

            PartitionArchives =
                _theController.GetPartitions(criteria);
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            DataBind();
        }

        protected void AddPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
           ((Default)Page).AddPartition(ServerPartition);
        }

        protected void EditPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.PartitionArchive selectedPartition =
                PartitionArchiveGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                ((Default)Page).EditPartition(selectedPartition);
            }
        }

        protected void DeletePartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.PartitionArchive selectedPartition =
                PartitionArchiveGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                ((Default)Page).DeletePartition(selectedPartition);
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public void UpdateUI()
        {
            LoadData();
            PartitionArchiveGridPanel.UpdateUI();

            Model.PartitionArchive partition = PartitionArchiveGridPanel.SelectedPartition;

            EditPartitionButton.Enabled = partition != null;
            DeletePartitionButton.Enabled = (partition != null && _theController.CanDelete(partition));

            UpdatePanel1.Update();
        }

        #endregion Public methods
       
    }
}