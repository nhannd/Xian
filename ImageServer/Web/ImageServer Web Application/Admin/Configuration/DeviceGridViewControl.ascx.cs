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
using System.Collections.Generic;

using System.ComponentModel;
using ClearCanvas.ImageServer.Model;

//
//  This is the controller for DeviceGridView control
//
public partial class DeviceGridViewControl : System.Web.UI.UserControl
{
    #region public properties



    public Device SelectedDevice
    {
        get
        {
            if (Devices.Count == 0 || GridView1.SelectedIndex < 0)
                return null;

            int index = GridView1.PageIndex * GridView1.PageSize + GridView1.SelectedIndex;
            return Devices[index];
        }
    }
    #endregion

    #region private memebers, properties
    // lookup table for server partitions based on server key
    protected Dictionary<string,ServerPartition> _DictionaryPartitions = new Dictionary<string,ServerPartition>();

    protected Dictionary<string, ServerPartition> DictionaryPartitions
    {
        get { return _DictionaryPartitions; }
        set { _DictionaryPartitions = value; }
    }


    protected IList<Device> _devices;
    public IList<Device> Devices
    {
        get
        {
            return _devices;
        }
        set
        {
            _devices = value;
            GridView1.DataSource = _devices; // must manually call DataBind()
        }
    }

    protected IList<ServerPartition> _partitions;
    public IList<ServerPartition> Partitions
    {
        get
        {
            return _partitions;
        }
        set
        {
            _partitions = value;
            
            DictionaryPartitions.Clear();
            foreach (ServerPartition p in _partitions)
            {
                DictionaryPartitions.Add(p.GetKey().Key.ToString(), p);
            }

        }
    }
    #endregion



    #region Render methods

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
        if (GridView1.EditIndex != e.Row.RowIndex)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Add OnClick attribute to each row to make javascript call "Select$###" (where ### is the selected row)
                // This method when posted back will be handled by the grid
                e.Row.Attributes["OnClick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Select$" + e.Row.RowIndex);
                e.Row.Style["cursor"] = "hand";

                // For some reason, double-click won't work if single-click is used
                // e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Edit$" + e.Row.RowIndex);

                CustomizeActiveColumn(e);
                CustomizeDHCPColumn(e);

                CustomizeServerPartitionColumn(e);

            }

        }

    }

    private  void CustomizeDHCPColumn(GridViewRowEventArgs e)
    {
        Image img = ((Image)e.Row.FindControl("DHCPImage"));
        if (img != null)
        {
            bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "DHCP"));
            if (active)
            {
                img.ImageUrl = "~/images/checked_small.gif";
            }
            else
            {
                img.ImageUrl = "~/images/unchecked_small.gif";
            }
        }
    }

    private  void CustomizeActiveColumn(GridViewRowEventArgs e)
    {
        Image img = ((Image)e.Row.FindControl("ActiveImage"));
        
        if (img != null)
        {
            bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Active"));
            if (active)
                img.ImageUrl = "~/images/checked_small.gif";
            else
            {
                img.ImageUrl = "~/images/unchecked_small.gif";
            }
        }
    }

    // Display the Partition Description in Server Partition column
    private  void CustomizeServerPartitionColumn(GridViewRowEventArgs e)
    {
        
        Device dev = e.Row.DataItem as Device;
        Label lbl = e.Row.FindControl("ServerParitionLabel") as Label; // The label is added in the template
        lbl.Text = dev.ServerPartition.Description;
    }

    protected void GridView1_UpdateToolBar()
    {

    }

    protected void UpdatePager()
    {

        if (GridView1.BottomPagerRow != null)
        {
            Label lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PageLabel") as Label;
            if (lbl != null)
                lbl.Text = "(Page " + (GridView1.PageIndex + 1) + " of " + GridView1.PageCount + ")";

            LinkButton btn = GridView1.BottomPagerRow.Cells[0].FindControl("PrevButton") as LinkButton;
            if (btn != null)
                btn.Enabled = GridView1.PageIndex > 0;

            btn = GridView1.BottomPagerRow.Cells[0].FindControl("NextButton") as LinkButton;
            if (btn != null)
                btn.Enabled = GridView1.PageIndex < GridView1.PageCount - 1;

            Label NumDevLabel = GridView1.BottomPagerRow.Cells[0].FindControl("NumDeviceLabel") as Label;
            if (NumDevLabel != null)
                NumDevLabel.Text = string.Format("{0} Device(s)", Devices.Count);

        }

    }

    protected void GridView1_PreRender(object sender, EventArgs e)
    {

    }
    #endregion


    #region Event handlers

    protected void DropDownList1_DataBound(object sender, EventArgs e)
    {


    }

   

    public override void DataBind()
    {
        GridView1.DataBind();
    }

    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView1_UpdateToolBar();
    }

    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView1_UpdateToolBar();
    }
    protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        GridView1_UpdateToolBar();
    }
    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        GridView1_UpdateToolBar();
        UpdatePager();
        
    }

    
    protected void GridView1_PageIndexChanged(object sender, EventArgs e)
    {
        DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GridView1.DataBind();
    }
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
    }

    #endregion

}
