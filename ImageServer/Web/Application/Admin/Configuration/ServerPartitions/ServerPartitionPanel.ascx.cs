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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// Server parition panel  used in <seealso cref="ServerPartitionPage"/> web page.
    /// </summary>
    public partial class ServerPartitionPanel : System.Web.UI.UserControl
    {
        #region Private Members

        // list of partitions displayed in the list
        private IList<ServerPartition> _partitions = new List<ServerPartition>();
        // used for database interaction
        private IServerPartitionConfigurationController _theController;
        private ServerPartitionPage _enclosingPage;

        #endregion Private Members

        #region Public Properties

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
        public IServerPartitionConfigurationController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

        public ServerPartitionPage EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            if (AETitleFilter.Text.Length > 0 || DescriptionFilter.Text.Length > 0 || EnabledOnlyFilter.Checked)
                return true;
            else
                return false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            UpdateUI();

            if (Page.IsPostBack)
            {
                // Change the image of the "Apply Filter" button based on the filter settings
                if (HasFilters())
                    FilterButton.ImageUrl = "~/images/icons/QueryEnabled.png";
                else
                    FilterButton.ImageUrl = "~/images/icons/QueryEnabled.png";
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            GridPager.Grid = ServerPartitionGridPanel.TheGrid;
            GridPager.ItemName = "Partition";
            GridPager.PuralItemName = "Partitions";

            SetupEventHandlers();
        }

        protected void SetupEventHandlers()
        {
            GridPager.GetRecordCountMethod = delegate { return Partitions.Count; };
        }

        protected void Clear()
        {
            AETitleFilter.Text = "";
            DescriptionFilter.Text = "";
            EnabledOnlyFilter.Checked = false;
            FolderFilter.Text = "";
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

            if (EnabledOnlyFilter.Checked)
            {
                criteria.Enabled.EqualTo(true);
            }


            Partitions =
                _theController.GetPartitions(criteria);
        }

        #endregion Protected Methods

        #region Public Methods

        public void UpdateUI()
        {
            LoadData();
            ServerPartitionGridPanel.UpdateUI();

            ServerPartition parition = ServerPartitionGridPanel.SelectedPartition;
            if (parition == null)
            {
                // no Partition being selected

                EditButton.Enabled = false;
                EditButton.ImageUrl = "~/images/icons/EditDisabled.png";
            }
            else
            {
                EditButton.Enabled = true;
                EditButton.ImageUrl = "~/images/icons/EditEnabled.png";
            }

            UpdatePanel1.Update();
        }

        #endregion Public methods

        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            LoadData();
            UpdateUI();
        }

        protected void AddButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.OnAddPartition();
        }

        protected void EditButton_Click(object sender, ImageClickEventArgs e)
        {
            ServerPartition selectedPartition =
                ServerPartitionGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                EnclosingPage.OnEditPartition(selectedPartition);
            }
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            // refresh the list
            Clear();
            LoadData();
            UpdateUI();
        }
    }
}