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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// Server parition panel  used in <seealso cref="ServerPartitionPage"/> web page.
    /// </summary>
    public partial class ServerPartitionPanel : System.Web.UI.UserControl
    {
        #region private Members
        // list of partitions displayed in the list
        private IList<ServerPartition> _partitions = new List<ServerPartition>();
        // used for database interaction
        private IServerPartitionConfigurationController _theController; 
        #endregion private Members

        #region Public Properties
        // Sets/Gets the list of partitions displayed in the panel
        public IList<ServerPartition> Partitions
        {
            get { return _partitions; }
            set { 
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

        #endregion Public Properties

        #region Public delegates

        /// <summary>
        /// Defines the delegate called by the panel to add new partition.
        /// </summary>
        public delegate void AddPartitionDelegate();

        /// <summary>
        /// Sets/Gets the delegate to add a new partition
        /// </summary>
        public AddPartitionDelegate AddPartitionMethod;

        /// <summary>
        /// Defines the delegate called by the panel to update an existing partition.
        /// </summary>
        /// <param name="partition"></param>
        public delegate void EditPartitionDelegate(ServerPartition partition);

        /// <summary>
        /// Sets/Gets the delegate to update an eisting partition.
        /// </summary>
        public EditPartitionDelegate EditPartitionMethod;

        #endregion Public delegates

        #region Protected Methods
        protected void Page_Load(object sender, EventArgs e)
        {

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
            GridPager.GetRecordCountMethod = delegate
                                                 {
                                                     return Partitions.Count;
                                                 };


            ServerPartitionToolBarPanel.OnAddPartitionButtonClick += delegate(object sender, ImageClickEventArgs ev)
                                                                         {
                                                                             if (AddPartitionMethod!=null)
                                                                                 AddPartitionMethod();
                                                                         };



            ServerPartitionToolBarPanel.OnEditPartitionButtonClick += delegate(object sender, ImageClickEventArgs ev)
                                                                          {
                                                                              ServerPartition selectedPartition =
                                                                                  ServerPartitionGridPanel.SelectedPartition;

                                                                              if (selectedPartition!=null)
                                                                              {
                                                                                  if (EditPartitionMethod != null)
                                                                                      EditPartitionMethod(selectedPartition);
                                                                              }
                                              
                                                                          };

            ServerPartitionToolBarPanel.OnRefreshButtonClick += delegate(object sender, ImageClickEventArgs ev)
                                                                    {
                                                                        // refresh the list
                                                                        ServerPartitionFilterPanel.Clear();
                                                                        LoadData();
                                                                        UpdateUI();
                                                                    };

            ServerPartitionToolBarPanel.GetSelectedPartition = delegate()
                                                                   {
                                                                       return ServerPartitionGridPanel.SelectedPartition;
                                                                   };

            ServerPartitionFilterPanel.ApplyFilterClicked += delegate(ServerPartitionFilterPanel.FilterSettings filters)
                                                                 {
                                                                     // refresh the list
                                                                     LoadData();
                                                                     UpdateUI();
                                                                 };
        }


        protected void LoadData()
        {
            ServerPartitionFilterPanel.FilterSettings filters = ServerPartitionFilterPanel.Filters;

            ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();

            if (String.IsNullOrEmpty(filters.AETitle) == false)
            {
                string key = filters.AETitle.Replace("*", "%");
                criteria.AETitle.Like(key + "%");
            }

            if (String.IsNullOrEmpty(filters.Description) == false)
            {
                string key = filters.Description.Replace("*", "%");
                criteria.Description.Like(key + "%");
            }

            if (String.IsNullOrEmpty(filters.Folder) == false)
            {
                string key = filters.Folder.Replace("*", "%");
                criteria.PartitionFolder.Like(key + "%");
            }

            if (filters.EnabledOnly)
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

            UpdatePanel1.Update();
        }

        #endregion Public methods

    }
}