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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    /// <summary>
    /// Panel to display list of devices for a particular server partition.
    /// </summary>
    public partial class DevicePanel : UserControl
    {
        #region Private members

        // the controller used for interaction with the database.
        private DeviceConfigurationController _theController;
        // the partition whose information will be displayed in this panel
        private ServerPartition _partition;
        private Default _enclosingPage;

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the partition whose information is displayed in this panel.
        /// </summary>
        public ServerPartition ServerPartition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Default EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Set up event handlers for the child controls.
        /// </summary>
        protected void SetUpEventHandlers()
        {
            GridPagerTop.GetRecordCountMethod = delegate { return DeviceGridViewControl1.Devices.Count; };
            GridPagerBottom.GetRecordCountMethod = delegate { return DeviceGridViewControl1.Devices.Count; };
        }

        protected void Clear()
        {
            AETitleFilter.Text = string.Empty;
            IPAddressFilter.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
            DHCPFilter.SelectedIndex = 0;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new DeviceConfigurationController();


            // setup child controls
            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerDeviceSingleItem, App_GlobalResources.SR.GridPagerDeviceMultipleItems, DeviceGridViewControl1.TheGrid, ImageServerConstants.GridViewPagerPosition.top);
            GridPagerBottom.InitializeGridPager(App_GlobalResources.SR.GridPagerDeviceSingleItem, App_GlobalResources.SR.GridPagerDeviceMultipleItems, DeviceGridViewControl1.TheGrid, ImageServerConstants.GridViewPagerPosition.bottom);

            //GridPagerTop.AssociatedUpdatePanelID = SearchUpdatePanel.ClientID;

            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.All));
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.Enabled));
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.Disabled));

            DHCPFilter.Items.Add(new ListItem(App_GlobalResources.SR.All));
            DHCPFilter.Items.Add(new ListItem(App_GlobalResources.SR.DHCP));
            DHCPFilter.Items.Add(new ListItem(App_GlobalResources.SR.NoDHCP));

            // setup event handler for child controls
            SetUpEventHandlers();
        }

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            if (AETitleFilter.Text.Length > 0 || IPAddressFilter.Text.Length > 0 || StatusFilter.SelectedIndex > 0 ||
                DHCPFilter.SelectedIndex > 0)
                return true;
            else
                return false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateUI();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // This make sure we have the list to work with. 
            // the list may be out-dated if the add/update event is fired later
            // In those cases, the list must be refreshed again.
            LoadDevices();
        }

        #endregion Protected methods

        /// <summary>
        /// Load the devices for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="UpdateUI()"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="UpdateUI()"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadDevices()
        {
            DeviceSelectCriteria criteria = new DeviceSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(ServerPartition.GetKey());

            if (!String.IsNullOrEmpty(AETitleFilter.Text))
            {
                string key = AETitleFilter.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.AeTitle.Like(key);
            }

            if (!String.IsNullOrEmpty(IPAddressFilter.Text))
            {
                string key = IPAddressFilter.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.IpAddress.Like(key);
            }

            if (StatusFilter.SelectedIndex != 0)
            {
                if (StatusFilter.SelectedIndex == 1)
                    criteria.Enabled.EqualTo(true);
                else
                    criteria.Enabled.EqualTo(false);
            }

            if (DHCPFilter.SelectedIndex != 0)
            {
                if (DHCPFilter.SelectedIndex == 1)
                    criteria.Dhcp.EqualTo(true);
                else
                    criteria.Dhcp.EqualTo(false);
            }

            DeviceGridViewControl1.Devices = _theController.GetDevices(criteria);
            DeviceGridViewControl1.DataBind();
        }

        /// <summary>
        /// Updates the device list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadDevices()"/> instead.
        /// </remarks>
        public void UpdateUI()
        {
            LoadDevices();
            Device dev = DeviceGridViewControl1.SelectedDevice;

            if (dev == null)
            {
                // no device being selected
                EditDeviceButton.Enabled = false;
                DeleteDeviceButton.Enabled = false;
            }
            else
            {
                EditDeviceButton.Enabled = true;
                DeleteDeviceButton.Enabled = true;
            }

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            SearchUpdatePanel.Update();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            LoadDevices();

        }

        protected void AddDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.OnAddDevice(_theController, ServerPartition);
        }

        protected void EditDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            Device dev = DeviceGridViewControl1.SelectedDevice;
            if (dev != null)
            {
                EnclosingPage.OnEditDevice(_theController, ServerPartition, dev);
            }
        }

        protected void DeleteDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            Device dev = DeviceGridViewControl1.SelectedDevice;
            if (dev != null)
            {
                EnclosingPage.OnDeleteDevice(_theController, ServerPartition, dev);
            }
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            // Clear all filters and reload the data
            Clear();
            LoadDevices();
        }
    }
}