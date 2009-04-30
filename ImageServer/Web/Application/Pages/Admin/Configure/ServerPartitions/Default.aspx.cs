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
using System.Security.Permissions;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.ServerPartitions)]
    public partial class Default : BasePage
    {
        #region Private Members

        // used for database interaction
        private ServerPartitionConfigController _controller = null;

        #endregion

        #region Protected Methods

        protected void Initialize()
        {
            _controller = new ServerPartitionConfigController();

            ServerPartitionPanel.Controller = _controller;

            Page.Title = App_GlobalResources.Titles.ServerPartitionsPageTitle;

            SetupEventHandlers();
        }

        protected void SetupEventHandlers()
        {
            AddEditPartitionDialog.OKClicked += AddEditPartitionDialog_OKClicked;
            deleteConfirmBox.Confirmed += DeleteConfirmDialog_Confirmed;
        }


        protected void UpdateUI()
        {
            ServerPartitionPanel.UpdateUI();
            UpdatePanel.Update();
        }

        protected override void OnInit(EventArgs e)
        {
            ServerPartitionPanel.EnclosingPage = this;

            base.OnInit(e);

            Initialize();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddEditPartitionDialog_OKClicked(ServerPartition partition)
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
            ServerPartition partition = data as ServerPartition;
            if (partition != null)
            {
                if (!_controller.Delete(partition))
                {
                    UpdateUI();

                    MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Message =
                        "Unable to delete this server partition. This could mean there are studies on this partition.<BR>Please check the log file or contact the server administrator";
                    MessageBox.Show();
                }
                else
                {
                    UpdateUI();
                    if (ServerPartitionPanel.Partitions != null && ServerPartitionPanel.Partitions.Count == 0)
                    {
                        MessageBox.MessageType = MessageBox.MessageTypeEnum.INFORMATION;
                        MessageBox.Message =
                            String.Format(
                                "Server partition {0} has been removed.<P>At least one server partition is required. Please add a new server partition.",
                                partition.AeTitle);
                        MessageBox.Show();
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddPartition()
        {
            // display the add dialog
            AddEditPartitionDialog.Partition = null;
            AddEditPartitionDialog.EditMode = false;
            AddEditPartitionDialog.Show(true);
        }

        public void EditPartition(ServerPartition selectedPartition)
        {
            AddEditPartitionDialog.Partition = selectedPartition;
            AddEditPartitionDialog.EditMode = true;
            AddEditPartitionDialog.Show(true);
        }

        public void DeletePartition(ServerPartition selectedPartition)
        {
            deleteConfirmBox.Data = selectedPartition;
            deleteConfirmBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
            deleteConfirmBox.Message =
                String.Format(
                    "It's recommended that you disable the partition instead of deleting it.<P>Are you sure you still want to delete partition {0} and all related settings permanently?",
                    selectedPartition.AeTitle);
            deleteConfirmBox.Show();
        }

        #endregion
    }
}