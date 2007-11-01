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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Web.Common;


public partial class Admin_Configuration_Devices : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (Page.IsPostBack)
        {
            // Actually we shouldn't reload, but the control viewstate will be lost if we don't,
            // and this will lead to other codes not working properly (eg, edit device function takes the 
            // the data from the currently selected device. This object will not be available in the grid 
            // until its viewstate (in particular its data) has been restored.
            ReloadData(); 
        }
        DeviceFilterControl1.OnFilterSelectionChange += new Admin_Configuration_DeviceFilterControl.FilterSelectionChange(OnFilterSelectionChange);

        ServerPartitionDataAdapter serverPartitionAdapter = new ServerPartitionDataAdapter();
        DeviceFilterControl1.ServerPartitions = DeviceGridViewControl1.Partitions = serverPartitionAdapter.GetServerPartitions();

        
        
    }

    protected override void Render(HtmlTextWriter writer)
    {
        // Must reload data before rending
        // Events are processed after Page_Load() which makes the data in the process pipeline Some events
        ReloadData();
        base.Render(writer);
    }

    protected void ReloadData()
    {
        ServerEntityKey SearchServerPartitionKey = DeviceFilterControl1.SelectedServerPartitionKey;
        if (SearchServerPartitionKey==null)
        {
            DeviceDataAdapter deviceAdapter = new DeviceDataAdapter();
            DeviceGridViewControl1.Devices = deviceAdapter.GetDevices();
        }
        else
        {
            DeviceDataAdapter deviceAdapter = new DeviceDataAdapter();
            DeviceGridViewControl1.Devices = deviceAdapter.GetDevices("*", SearchServerPartitionKey);
        }

        DeviceGridViewControl1.DataBind();
        
    }

    protected void OnAddDeviceButtonClick(object sender, ImageClickEventArgs ev)
    {
        AddDeviceControl1.Show();
    }

    protected void OnDeleteDeviceButtonClick(object sender, ImageClickEventArgs ev)
    {
        //        ModalPopupExtender1.Show();  
        
     }

    protected void OnEditDeviceButtonClick(object sender, ImageClickEventArgs ev)
    {
        //        ModalPopupExtender1.Show();  

        if (DeviceGridViewControl1.SelectedDevice != null)
        {
            EditDeviceControl1.Device = DeviceGridViewControl1.SelectedDevice;
            EditDeviceControl1.Show();
        }
    }

    
    protected void OnRefreshButtonClick(object sender, EventArgs e)
    {
        ReloadData();
    }

    protected void AddDeviceControl1_Load(object sender, EventArgs e)
    {

    }

    protected void OnFilterSelectionChange(object sender, EventArgs e)
    {
        
    }

    
    // Attach event handlers for different buttons
    protected void DeviceToolBarControl1_Load(object sender, EventArgs e)
    {

        DeviceToolBarControl1.OnAddDeviceButtonClick += new Admin_Configuration_DeviceToolBarControl.AddDeviceButtonClick(OnAddDeviceButtonClick);
        DeviceToolBarControl1.OnDeleteDeviceButtonClick += new Admin_Configuration_DeviceToolBarControl.DeleteDeviceButtonClick(OnDeleteDeviceButtonClick);
        DeviceToolBarControl1.OnEditDeviceButtonClick += new Admin_Configuration_DeviceToolBarControl.EditDeviceButtonClick(OnEditDeviceButtonClick);

        DeviceToolBarControl1.OnRefreshButtonClick += new Admin_Configuration_DeviceToolBarControl.RefreshButtonClick(OnRefreshButtonClick);

    }


}
