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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive
{
    /// <summary>
    /// Server Partition configuration page.
    /// </summary>
    public partial class Default : BasePage
    {
        #region Private Members

        private IDictionary<ServerEntityKey, PartitionArchivePanel> _mapDevicePanel =
            new Dictionary<ServerEntityKey, PartitionArchivePanel>();

        private ServerPartition _serverPartition;

        // used for database interaction
        private PartitionArchiveConfigController _controller = new PartitionArchiveConfigController();

        #endregion

        #region Public Properties

        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
        }

        #endregion


        #region Protected Methods

        protected void SetupEventHandlers()
        {
            AddEditPartitionDialog.OKClicked += AddEditPartitionDialog_OKClicked;
            deleteConfirmBox.Confirmed += DeleteConfirmDialog_Confirmed;
        }


        protected void UpdateUI()
        {
            ServerPartitionTabs.Update(0);
            UpdatePanel.Update();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetupEventHandlers();

            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
                                                           {
                                                               _serverPartition = partition;
                                                               
                                                               PartitionArchivePanel panel =
                                                                   LoadControl("PartitionArchivePanel.ascx") as PartitionArchivePanel;
                                                               
                                                               panel.ID = "PartitionArchivePanel_" + partition.AeTitle;
                                                               _mapDevicePanel[partition.GetKey()] = panel; // this map is used to reload the list when the devices are updated.
                                                               return panel;
                                                           });

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddEditPartitionDialog_OKClicked(Model.PartitionArchive partition)
        {
            if (AddEditPartitionDialog.EditMode)
            {
                // Add partition into db and refresh the list
                if (_controller.UpdatePartition(partition))
                {
                    UpdateUI();
                }
            }
            else
            {
                // Add partition into db and refresh the list
                if (_controller.AddPartition(partition))
                {
                    UpdateUI();
                }
            }
        }

        private void DeleteConfirmDialog_Confirmed(object data)
        {
            Model.PartitionArchive partition = data as Model.PartitionArchive;
            if (partition != null)
            {
                if (!_controller.Delete(partition))
                {
                    UpdateUI();

                    MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Message =
                        "Unable to delete this partition archive. This could mean there are studies on this partition.<BR>Please check the log file or contact the server administrator";
                    MessageBox.Show();
                }
                else
                {
                    UpdateUI();
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddPartition()
        {
            // display the add dialog
            AddEditPartitionDialog.PartitionArchive = null;
            AddEditPartitionDialog.EditMode = false;
            AddEditPartitionDialog.Show(true);
        }

        public void EditPartition(Model.PartitionArchive partitionArchive)
        {
            AddEditPartitionDialog.PartitionArchive = partitionArchive;
            AddEditPartitionDialog.EditMode = true;
            AddEditPartitionDialog.Show(true);
        }

        public void DeletePartition(Model.PartitionArchive selectedPartition)
        {
            deleteConfirmBox.Data = selectedPartition;
            deleteConfirmBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
            deleteConfirmBox.Message =
                String.Format(
                    "Are you sure you want to delete this partition archive and all related settings permanently?");
            deleteConfirmBox.Show();
        }

        #endregion
 
    }
}