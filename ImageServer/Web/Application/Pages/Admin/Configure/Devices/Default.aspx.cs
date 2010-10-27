#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.App_GlobalResources;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.Devices)]
    public partial class Default : BasePage
    {
        #region Private members

        // Map between the server partition and the panel

        // the controller used for database interaction
        private readonly DeviceConfigurationController _controller = new DeviceConfigurationController();

        private readonly IDictionary<ServerEntityKey, DevicePanel> _mapDevicePanel =
            new Dictionary<ServerEntityKey, DevicePanel>();

        #endregion Private members

        public ServerEntityKey CurrentPartition
        {
            get
            {
                return ServerPartitionTabs.GetCurrentPartitionKey();
            }
        }


        #region Protected methods

        /// <summary>
        /// Set up the event handlers for child controls.
        /// </summary>
        protected void SetupEventHandlers()
        {
            AddEditDeviceControl1.OKClicked += delegate(Device dev)
                                                   {
                                                       if (AddEditDeviceControl1.EditMode)
                                                       {
                                                           // Commit the change into database
                                                           if (_controller.UpdateDevice(dev))
                                                           {
                                                               DevicePanel panel =
                                                                   _mapDevicePanel[dev.ServerPartition.GetKey()];
                                                               panel.UpdateUI();
                                                           }
                                                       }
                                                       else
                                                       {
                                                           // Create new device in the database
                                                           Device newDev = _controller.AddDevice(dev);
                                                           if (newDev != null)
                                                           {
                                                               DevicePanel panel =
                                                                   _mapDevicePanel[newDev.ServerPartition.GetKey()];
                                                               panel.UpdateUI();
                                                           }
                                                       }
													   UpdateUI();
                                                   };


            DeleteConfirmation.Confirmed += delegate(object data)
                                                {
                                                    // delete the device and reload the affected partition.
                                                    if (DeleteConfirmation.MessageType == MessageBox.MessageTypeEnum.YESNO)
                                                    {                                            
                                                        var dev = data as Device;
                                                        if (dev != null)
                                                        {
                                                            DevicePanel oldPanel =
                                                                _mapDevicePanel[dev.ServerPartition.GetKey()];

                                                            if (_controller.GetRelatedWorkQueueCount(dev) == 0)
                                                            {
                                                                _controller.DeleteDevice(dev);
                                                            }

                                                            if (oldPanel != null)
                                                                oldPanel.UpdateUI();
                                                        }
                                                        UpdateUI();
                                                    }
                                                };
            SetPageTitle(Titles.DevicesPageTitle);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddEditDeviceControl1.DevicePage = this;

            SetupEventHandlers();

            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
                                                           {
                                                               var panel =
                                                                   LoadControl("DevicePanel.ascx") as DevicePanel;
                                                               if (panel != null)
                                                               {
                                                                   panel.ServerPartition = partition;
                                                                   panel.ID = "DevicePanel_" + partition.AeTitle;

                                                                   panel.EnclosingPage = this;
                                                                   _mapDevicePanel[partition.GetKey()] = panel;
                                                                   // this map is used to reload the list when the devices are updated.
                                                               }
                                                               return panel;
                                                           });
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			UpdateUI();
		}

		protected void UpdateUI()
		{
			AddEditDeviceControl1.UpdateLabels();
			DialogUpdatePanel.Update();
		}

        #endregion  Protected methods

        #region Public Methods

        public void OnAddDevice(DeviceConfigurationController controller, ServerPartition serverPartition)
        {
            // Populate the add device dialog and display it
            AddEditDeviceControl1.EditMode = false;
            AddEditDeviceControl1.Device = null;
            IList<ServerPartition> list = new List<ServerPartition>();
            list.Add(serverPartition);
            AddEditDeviceControl1.Partitions = list;
            AddEditDeviceControl1.Show(true);
        }

        public void OnEditDevice(DeviceConfigurationController controller, ServerPartition serverPartition, Device dev)
        {
            // Populate the edit device dialog and display it
            AddEditDeviceControl1.EditMode = true;
            AddEditDeviceControl1.Device = dev;
            AddEditDeviceControl1.Partitions = controller.GetServerPartitions();
            AddEditDeviceControl1.Show(true);
        }

        public void OnDeleteDevice(DeviceConfigurationController controller, ServerPartition serverPartition, Device dev)
        {
            if (controller.GetRelatedWorkQueueCount(dev) > 0)
            {
                DeleteConfirmation.Message = string.Format("The device {0} has entries in the WorkQueue and cannot be deleted.",
                                                          dev.AeTitle);
                DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.INFORMATION;             
            }
            else
            {
                DeleteConfirmation.Message = string.Format("Are you sure you want to remove {0} from partition {1}?",
                                                           dev.AeTitle, serverPartition.AeTitle);
                DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.YESNO;
            }

            DeleteConfirmation.Data = dev;
            DeleteConfirmation.Show();
        }

        
        #endregion
    }
}