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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

//
//  This is the controller for Device Filter panel
//
public partial class Admin_Configuration_DeviceFilterControl : System.Web.UI.UserControl
{
    static string ALL_PARTITIONS_TEXT = "--------- ALL ----------";

    #region private members
    protected IList<ServerPartition> _ServerPartitions;
    #endregion

    #region public properties
    public IList<ServerPartition> ServerPartitions
    {
        get
        {
            return _ServerPartitions ;
        }
        set
        {
            // Populate the server partition dropdown list 

            _ServerPartitions = value;
            int selectedIndex = ServerPartitionDropDownList.SelectedIndex;
            ServerPartitionDropDownList.Items.Clear();
            ServerPartitionDropDownList.Items.Add(new ListItem(ALL_PARTITIONS_TEXT, null));
            foreach (ServerPartition p in _ServerPartitions)
            {
                ServerPartitionDropDownList.Items.Add(new ListItem(p.Description, p.GetKey().Key.ToString()));
            }

            if (selectedIndex < 0)
                selectedIndex = 0; // Select "-------ALL--------" when first loaded

            ServerPartitionDropDownList.SelectedIndex = selectedIndex;
        }
    }

    public ServerEntityKey SelectedServerPartitionKey
    {
        get
        {
            if (ServerPartitionDropDownList.SelectedIndex < 0 || ServerPartitionDropDownList.SelectedValue == ALL_PARTITIONS_TEXT)
                return null;

            return new ServerEntityKey("ServerPartition", ServerPartitionDropDownList.SelectedValue);
        }
    }

    public bool ActiveDevicesOnly
    {
        get
        {
            return ActiveOnlyCheckBox.Checked;
        }
    }
    #endregion

    #region Events
    public delegate void FilterSelectionChange(object sender, EventArgs e);
    public event FilterSelectionChange OnFilterSelectionChange;
    #endregion

    #region initialization methods
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion

    #region event handlers
    protected void ServerPartitionDropDownList_SelectedIndexChanged1(object sender, EventArgs e)
    {
        if (OnFilterSelectionChange!=null)
            OnFilterSelectionChange(sender, e);
    }
    protected void ActiveOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (OnFilterSelectionChange!=null)
            OnFilterSelectionChange(sender, e);
    }
    #endregion
}
