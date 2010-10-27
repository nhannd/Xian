#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
        private ServerPartitionConfigController _controller;

        #endregion

        #region Protected Methods

        protected void Initialize()
        {
            _controller = new ServerPartitionConfigController();

            ServerPartitionPanel.Controller = _controller;

            SetPageTitle(App_GlobalResources.Titles.ServerPartitionsPageTitle);

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
            deleteConfirmBox.MessageStyle = "color: red; font-weight: bold;";
            deleteConfirmBox.Show();
        }

        #endregion
    }
}