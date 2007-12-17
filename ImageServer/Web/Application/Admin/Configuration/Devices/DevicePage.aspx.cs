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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices
{
    /// <summary>
    /// Device Configuration Web Page.
    /// </summary>
    public partial class DevicePage : System.Web.UI.Page
    {
        #region Private members
        // Map between the server partition and the panel
        private IDictionary<ServerEntityKey, DevicePanel> _mapDevicePanel = new Dictionary<ServerEntityKey, DevicePanel>();

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
                                                           DevicePanel panel = _mapDevicePanel[dev.ServerPartition.GetKey()];
                                                           panel.UpdateUI();
                                                       }
                                                   }
                                                   else
                                                   {
                                                       // Create new device in the database
                                                       if (_controller.AddDevice(dev))
                                                       {
                                                           DevicePanel panel = _mapDevicePanel[dev.ServerPartition.GetKey()];
                                                           panel.UpdateUI();
                                                       } 
                                                   }
                                                   
                                                   
                                               };
            

            ConfirmDialog1.Confirmed += delegate(object data)
                                           {
                                               // delete the device and reload the affected partition.

                                               Device dev = data as Device;
                                               DevicePanel oldPanel = _mapDevicePanel[dev.ServerPartition.GetKey()];
                                               _controller.DeleteDevice(dev);
                                               if (oldPanel!=null)
                                                    oldPanel.UpdateUI();
                                           };
        }

        protected void SetupLoadPartitionTabs()
        {
            int n = 0; 
            
            TabContainer1.Tabs.Clear();
            IList<ServerPartition> partitions = GetPartitions();
            foreach (ServerPartition part in partitions)
            {
                n++;

                // create a tab
                TabPanel tabPannel = new TabPanel();
                tabPannel.HeaderText = part.AeTitle;
                tabPannel.ID = "Tab_" + n;

                // create a device panel
                DevicePanel devPanel = LoadControl("DevicePanel.ascx") as DevicePanel;
                devPanel.Partition = part;
                devPanel.ID = "DevicePanel_" + n;

                // put the panel into a lookup table to be used later
                _mapDevicePanel[part.GetKey()] = devPanel;

                // Setup delegates 
                devPanel.AddDeviceDelegate = delegate(DeviceConfigurationController controller, ServerPartition partition)
                                           {
                                               // Populate the add device dialog and display it
                                               AddEditDeviceControl1.EditMode = false;
                                               AddEditDeviceControl1.Device = null;
                                               IList<ServerPartition> list = new List<ServerPartition>();
                                               list.Add(partition);
                                               AddEditDeviceControl1.Partitions = list;
                                               AddEditDeviceControl1.Show();
                                           };

                devPanel.EditDeviceDelegate =
                    delegate(DeviceConfigurationController controller, ServerPartition partition, Device dev)
                    {
                        // Populate the edit device dialog and display it
                        AddEditDeviceControl1.EditMode = true;
                        AddEditDeviceControl1.Device = dev;
                        AddEditDeviceControl1.Partitions = controller.GetServerPartitions();
                        AddEditDeviceControl1.Show();
                    };

                devPanel.DeleteDeviceDelegate = delegate(DeviceConfigurationController controller, ServerPartition partition, Device dev)
                                              {
                                                  ConfirmDialog1.Message = string.Format("Are you sure to remove {0} from partition {1}?", dev.AeTitle, partition.AeTitle);
                                                  ConfirmDialog1.MessageType = ConfirmDialog.MessageTypeEnum.WARNING;
                                                  ConfirmDialog1.Data = dev;
                                                  ConfirmDialog1.Show();
                                              };


                // Add the device panel into the tab
                tabPannel.Controls.Add(devPanel);

                // Add the tab into the tabstrip
                TabContainer1.Tabs.Add(tabPannel);
            }

            if (partitions != null && partitions.Count > 0)
                TabContainer1.ActiveTabIndex = 0;
            else
            {
                TabContainer1.ActiveTabIndex = -1;
                Label1.Text = "Please add a server partition first";
                Label1.Visible = true;
            }
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

            SetupLoadPartitionTabs();
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        #endregion  Protected methods
        
    }
}
