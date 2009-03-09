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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class AddEditDeviceDialog : UserControl
    {
        #region private variables

        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.
        private IList<ServerPartition> _partitions = new List<ServerPartition>();

        private bool _editMode;
        // device being editted/added
        private Device _device;

        #endregion

        #region public members

        /// <summary>
        /// Sets the list of partitions users allowed to pick.
        /// </summary>
        public IList<ServerPartition> Partitions
        {
            set { _partitions = value; }

            get { return _partitions; }
        }

        /// <summary>
        /// Sets or gets the value which indicates whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                ViewState[ "EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public Device Device
        {
            set
            {
                _device = value;
                // put into viewstate to retrieve later
                ViewState[ "EditedDevice"] = _device;
            }
            get { return _device; }
        }

        #endregion // public members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="device">The device being added.</param>
        public delegate void OnOKClickedEventHandler(Device device);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TabContainer1.ActiveTabIndex = 0;

            DHCPCheckBox.InputAttributes.Add("onClick", "EnableDisableIp();");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID,
                                                        @"<script language='javascript'>
                            function EnableDisableIp()
                            {
                                var checkBox = document.getElementById('" +
                                                        DHCPCheckBox.ClientID +
                                                        @"');
                                var ipBox = document.getElementById('" +
                                                        IPAddressTextBox.ClientID +
                                                        @"');
                                ipBox.disabled=checkBox.checked;         
                                ipBox.value = '';
                            }
                        </script>");

            EditDeviceValidationSummary.HeaderText = App_GlobalResources.ErrorMessages.EditStudyValidationError;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
            }
            else
            {
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool) ViewState[ "EditMode"];

                if (ViewState[ "EditedDevice"] != null)
                    _device = ViewState[ "EditedDevice"] as Device;
            }
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveData();

                if (OKClicked != null)
                    OKClicked(Device);

                Close();
            }
            else
            {
                Show(false);
            }
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        
        private void SaveData()
        {
            if (Device == null)
            {
                Device = new Device();
            }

            Device.Enabled = ActiveCheckBox.Checked;
            Device.AeTitle = AETitleTextBox.Text;
            Device.Description = DescriptionTextBox.Text;
            Device.Dhcp = DHCPCheckBox.Checked;
            Device.IpAddress = IPAddressTextBox.Text;
            int port;
            if (Int32.TryParse(PortTextBox.Text, out port))
                Device.Port = port;
            Device.ServerPartitionKey = new ServerEntityKey("Device", ServerPartitionDropDownList.SelectedItem.Value);
            Device.AllowStorage = AllowStorageCheckBox.Checked;
            Device.AllowQuery = AllowQueryCheckBox.Checked;
            Device.AllowRetrieve = AllowRetrieveCheckBox.Checked;
            Device.AllowAutoRoute = AllowAutoRouteCheckBox.Checked;
            Device.ThrottleMaxConnections = ThrottleSettingsTab.MaxConnections;

        }

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            // update the dropdown list
            ServerPartitionDropDownList.Items.Clear();
            foreach (ServerPartition partition in _partitions)
            {
                ServerPartitionDropDownList.Items.Add(
                    new ListItem(partition.AeTitle, partition.GetKey().Key.ToString())
                    );
            }

            // Update the title and OK button text. Changing the image is the only way to do this, since the 
            // SkinID cannot be set dynamically after Page_PreInit.
            if (EditMode)
            {
                ModalDialog1.Title  = App_GlobalResources.SR.DialogEditDeviceTitle;
                OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.UpdateButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.UpdateButtonHover;
            }
            else
            {
                ModalDialog1.Title = App_GlobalResources.SR.DialogAddDeviceTitle;
                OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.AddButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.AddButtonHover;
            }

            // Update the rest of the fields
            if (Device == null)
            {
                AETitleTextBox.Text = App_GlobalResources.SR.DeviceAE;
                IPAddressTextBox.Text = string.Empty;
                ActiveCheckBox.Checked = true;
                DHCPCheckBox.Checked = false;
                DescriptionTextBox.Text = string.Empty;
                PortTextBox.Text = App_GlobalResources.SR.DeviceDefaultPort;
                AllowStorageCheckBox.Checked = true;
                AllowQueryCheckBox.Checked = true;
                AllowRetrieveCheckBox.Checked = true;
                AllowAutoRouteCheckBox.Checked = true;
                ThrottleSettingsTab.MaxConnections = UICommonSettings.Admin.Device.MaxConnections;
                ServerPartitionDropDownList.SelectedIndex = 0;
            }
            else if (Page.IsValid)
            {
                AETitleTextBox.Text = Device.AeTitle;
                IPAddressTextBox.Text = Device.IpAddress;
                ActiveCheckBox.Checked = Device.Enabled;
                DHCPCheckBox.Checked = Device.Dhcp;
                DescriptionTextBox.Text = Device.Description;
                PortTextBox.Text = Device.Port.ToString();

                AllowStorageCheckBox.Checked = Device.AllowStorage;
                AllowQueryCheckBox.Checked = Device.AllowQuery;
                AllowRetrieveCheckBox.Checked = Device.AllowRetrieve;
                AllowAutoRouteCheckBox.Checked = Device.AllowAutoRoute;
                ThrottleSettingsTab.MaxConnections = Device.ThrottleMaxConnections;
                ServerPartitionDropDownList.SelectedValue = Device.ServerPartitionKey.Key.ToString();
            }
        }

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();


            IPAddressTextBox.Enabled = !DHCPCheckBox.Checked;

            TabContainer1.ActiveTabIndex = 0;

            ModalDialog1.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog1.Hide();
        }

        #endregion Public methods
    }
}
