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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    /// <summary>
    /// Device Configuration Web Page.
    /// </summary>
    public partial class Default : BasePage
    {
        #region Private members

        // Map between the server partition and the panel
        private IDictionary<ServerEntityKey, DevicePanel> _mapDevicePanel =
            new Dictionary<ServerEntityKey, DevicePanel>();

        // the controller used for database interaction
        private DeviceConfigurationController _controller = new DeviceConfigurationController();

        #endregion Private members

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
                                                           if (newDev!=null)
                                                           {
                                                               DevicePanel panel =
                                                                   _mapDevicePanel[newDev.ServerPartition.GetKey()];
                                                               panel.UpdateUI();
                                                           }
                                                       }
                                                   };


            DeleteConfirmation.Confirmed += delegate(object data)
                                            {
                                                // delete the device and reload the affected partition.

                                                Device dev = data as Device;
                                                DevicePanel oldPanel = _mapDevicePanel[dev.ServerPartition.GetKey()];
                                                _controller.DeleteDevice(dev);
                                                if (oldPanel != null)
                                                    oldPanel.UpdateUI();
                                            };
        }


        /// <summary>
        /// Retrieves the partitions to be rendered in the page.
        /// </summary>
        /// <returns></returns>
        private IList<ServerPartition> GetPartitions()
        {
            // TODO We may want to add context or user preference here to specify which partitions to load

            IList<ServerPartition> list = _controller.GetServerPartitions();
            return list;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddEditDeviceControl1.Partitions = GetPartitions();

            SetupEventHandlers();

            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
                                                           {
                                                               DevicePanel panel =
                                                                   LoadControl("DevicePanel.ascx") as DevicePanel;
                                                               panel.ServerPartition = partition;
                                                               panel.ID = "DevicePanel_" + partition.AeTitle;

                                                               panel.EnclosingPage = this;
                                                               _mapDevicePanel[partition.GetKey()] = panel; // this map is used to reload the list when the devices are updated.
                                                               return panel;
                                                           });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
            DeleteConfirmation.Message = string.Format("Are you sure to remove {0} from partition {1}?", dev.AeTitle, serverPartition.AeTitle);
            DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.YESNO;
            DeleteConfirmation.Data = dev;
            DeleteConfirmation.Show();
        }

        #endregion
    }
}
