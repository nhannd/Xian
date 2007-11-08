#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ImageServerWebApplication.Admin.Configuration
{
    //
    // Dialog for adding new device.
    //
    public partial class AddDeviceDialog : UserControl
    {
        #region private variables
        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.
        private IList<ServerPartition> _partitions = new List<ServerPartition>();

        
        #endregion

        #region public members
        /// <summary>
        /// Sets the list of partitions users allowed to pick for the new device.
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


            // Register a javascript that can be called to popup this dialog on the client
            // 
            Page.RegisterClientScriptBlock("popupThisWindow",
                      @"<script language='javascript'>
                        function ShowAddDeviceDialog()
                        {  
                            var ctrl = $find('" + ModalPopupExtender1.UniqueID + @"'); 
                            ctrl.show();
                        }
                    </script>");

        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.IsPostBack == false)
            {


            }

        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            Device device = new Device();
            device.Active = ActiveCheckBox.Checked;
            device.AeTitle = AETitleTextBox.Text;
            device.Description = DescriptionTextBox.Text;
            device.Dhcp = DHCPCheckBox.Checked;
            device.IpAddress = IPAddressTextBox.Text;
            device.Port = Int32.Parse(PortTextBox.Text);
            device.ServerPartitionKey = new ServerEntityKey("Device", ServerPartitionDropDownList.SelectedItem.Value);

            // TODO: Add input validation here


            if (OKClicked != null)
                OKClicked(device);

            Close();

        }

        #endregion Protected methods


        #region Public methods
        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show()
        {
            // STRANGE AJAX BUG?: 
            //      This block of code will cause 
            //      WebForms.PageRequestManagerServerErrorException: Status code 500 when the dialog box is dismissed.
            //
            //
            // Pre-fill the data
            //
            //  AETitleTextBox.Text = "<Enter AE Title>";
            //  ActiveCheckBox.Checked = false;
            //  DHCPCheckBox.Checked = false;
            //  DescriptionTextBox.Text = "<Enter Description>";
            //  PortTextBox.Text = "<Port #>";


            // update the dropdown list
            ServerPartitionDropDownList.Items.Clear();
            foreach (ServerPartition partition in _partitions)
            {
                ServerPartitionDropDownList.Items.Add(new ListItem(partition.Description, partition.GetKey().Key.ToString()));
            }

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
