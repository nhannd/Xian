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


namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices
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
            set
            {
                _partitions = value;
            }

            get
            {
                return _partitions;
            }
        }

        /// <summary>
        /// Sets or gets the value which indicates whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            get { return _editMode; }
            set { 
                _editMode = value;
                ViewState[ClientID + "_EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public Device Device
        {
            set
            {
                this._device = value;
                // put into viewstate to retrieve later
                ViewState[ClientID + "_EdittedDevice"] = _device;
            }
            get
            {
                return _device;
            }

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

            
            // Set up the popup extender
            // These settings could been done in the aspx page as well
            // but if we are to javascript to display, that won't work.
            ModalPopupExtender1.PopupControlID = DialogPanel.UniqueID;
            ModalPopupExtender1.TargetControlID = DummyPanel.UniqueID;
            ModalPopupExtender1.BehaviorID = ModalPopupExtender1.UniqueID;

            ModalPopupExtender1.DropShadow = true;
            ModalPopupExtender1.Drag = true;
            ModalPopupExtender1.PopupDragHandleControlID = TitleBarPanel.UniqueID;

            OKButton.OnClientClick = ClientID + "_clearFields()";

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ClientID,
                        @"<script language='javascript'>
                            function AddEditDialog_HideHelpImage(helpImgID)
                            {
                                img = document.getElementById(helpImgID);
                                img.style.visibility = 'hidden';
                            }
                            function AddEditDialog_ClearField(fieldID)
                            {
                                txtbox = document.getElementById(fieldID);
                                txtbox.style.backgroundColor = '';
                            }

                            function " + ClientID+ @"_clearFields()
                            {
                                
                                AddEditDialog_ClearField('" + AETitleTextBox.ClientID + @"');
                                AddEditDialog_HideHelpImage('" + AETitleHelpImage.ClientID + @"');
                                AddEditDialog_ClearField('" + IPAddressTextBox.ClientID + @"');
                                AddEditDialog_HideHelpImage('" + IPAddressHelpImage.ClientID + @"');
                                AddEditDialog_ClearField('" + PortTextBox.ClientID + @"');
                                AddEditDialog_HideHelpImage('" + PortHelpImage.ClientID + @"');
                                
                            }
                        </script>");
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.IsPostBack == false)
            {
               
            }
            else
            {
                if (ViewState[ClientID + "_EditMode"] != null)
                    _editMode = (bool)ViewState[ClientID + "_EditMode"];

                if (ViewState[ClientID + "_EdittedDevice"] != null)
                    _device = ViewState[ClientID + "_EdittedDevice"] as Device;
            }

        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Device==null)
            {
                Device = new Device();
            }

            Device.Enabled = ActiveCheckBox.Checked;
            Device.AeTitle = AETitleTextBox.Text;
            Device.Description = DescriptionTextBox.Text;
            Device.Dhcp = DHCPCheckBox.Checked;
            Device.IpAddress = IPAddressTextBox.Text;
            Device.Port = Int32.Parse(PortTextBox.Text);
            Device.ServerPartitionKey = new ServerEntityKey("Device", ServerPartitionDropDownList.SelectedItem.Value);
            Device.AllowStorage = AllowStorageCheckBox.Checked;
            Device.AllowQuery = AllowQueryCheckBox.Checked;
            Device.AllowRetrieve = AllowRetrieveCheckBox.Checked;


            // TODO: Add additional server-side validation here

            if (OKClicked != null)
                OKClicked(Device);

            Close();
           

        }

        #endregion Protected methods


        #region Public methods
        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            // update the dropdown list
            ServerPartitionDropDownList.Items.Clear();
            foreach (ServerPartition partition in _partitions)
            {
                ServerPartitionDropDownList.Items.Add(
                    new ListItem(partition.AeTitle, partition.GetKey().Key.ToString())
                    );
            }

            // Update the title and OK button text
            if (EditMode)
            {
                TitleLabel.Text = "Edit Device";
                OKButton.Text = "Update";
            }
            else
            {
                TitleLabel.Text = "Add Device";
                OKButton.Text = "Add";
            }

            // Update the rest of the fields
            if (Device==null)
            {
                AETitleTextBox.Text = "";
                IPAddressTextBox.Text = "";
                ActiveCheckBox.Checked = false;
                DHCPCheckBox.Checked = false;
                DescriptionTextBox.Text = "";
                PortTextBox.Text = "1";

                AllowStorageCheckBox.Checked = false;
                AllowQueryCheckBox.Checked = false;
                AllowRetrieveCheckBox.Checked = false;

                ServerPartitionDropDownList.SelectedIndex = 0;
            }
            else
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

                ServerPartitionDropDownList.SelectedValue = Device.ServerPartitionKey.Key.ToString();
            }

            TabContainer1.ActiveTabIndex = 0;

            UpdatePanel.Update();
            ModalPopupExtender1.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            // 
            // Clear all boxes
            //
            // STRANGE AJAX BUG?: 
            //      This block of code will cause 
            //      WebForms.PageRequestManagerServerErrorException: Status code 500 
            //      when other buttons are pressed AFTER the add device dialog box is dismissed.
            //
            //  Move the entire block into Show()
            //
            //  AETitleTextBox.Text = "<Enter AE Title>";
            //  ActiveCheckBox.Checked = false;
            //  DHCPCheckBox.Checked = false;
            //  DescriptionTextBox.Text = "<Enter Description>";
            //  PortTextBox.Text = "<Port #>";


            ModalPopupExtender1.Hide();
        }

        #endregion Public methods


        
    }
 
}
