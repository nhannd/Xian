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

//
// This is the controller for the Add Device Popup Dialog
//
public partial class Admin_Configuration_AddDeviceControl : UserControl
{
   
    //
    //  Populate the list of server partitions
    //  
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Page.IsPostBack == false)
        {
            
            ServerPartitionDataAdapter partitionAdapter = new ServerPartitionDataAdapter();
            IList<ServerPartition> partitions = partitionAdapter.GetServerPartitions();

            foreach (ServerPartition part in partitions)
            {
                ServerPartitionDropDownList.Items.Add(new ListItem(part.Description, part.GetKey().Key.ToString()));

            }

        }
        
        
    
        // These settings could been done in the aspx page as well
        ModalPopupExtender1.DropShadow = true;
        ModalPopupExtender1.Drag = true;
        ModalPopupExtender1.PopupDragHandleControlID = TitleBar.UniqueID;


        


    }

    public void Show()
    {
        // Popup the dialog
        /*AETitleTextBox.Text = "<Enter AE Title>";
        ActiveCheckBox.Checked = false;
        DHCPCheckBox.Checked = false;
        DescriptionTextBox.Text = "<Enter Description>";
        PortTextBox.Text = "<Port #>";
        */

        ModalPopupExtender1.Show();
    }


    // Return the title bar control 
    // Used by the modal popup extender to let user drag the window
    public Control TitleBar
    {
        get { return TitleLabel; }
    }

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


    // Create a Device instance 
    // and use the adapter to insert into database
    // TODO: exception handling and input validation
    protected void OKButton_Click(object sender, EventArgs e)
    {
        DeviceDataAdapter adapter = new DeviceDataAdapter();
        Device device = new Device();
        device.Active = ActiveCheckBox.Checked;
        device.AeTitle = AETitleTextBox.Text;
        device.Description = DescriptionTextBox.Text;
        device.Dhcp = DHCPCheckBox.Checked;
        device.IpAddress = IPAddressTextBox.Text;
        device.Port = Int32.Parse(PortTextBox.Text);
        device.ServerPartitionKey = new ServerEntityKey("Device", ServerPartitionDropDownList.SelectedItem.Value);
        adapter.AddDevice(device);
        Close();

    }
}
