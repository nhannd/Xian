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


namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices
{
    //
    //  Used to display the list of devices.
    //
    public partial class DeviceGridView : UserControl
    {
        #region private members

        // server partitions lookup table based on server key
        private Dictionary<string, ServerPartition> _DictionaryPartitions = new Dictionary<string, ServerPartition>();
        // list of devices to display
        private IList<Device> _devices;
        private Unit _height;
        #endregion Private members

        #region protected properties

        protected Dictionary<string, ServerPartition> DictionaryPartitions
        {
            get { return _DictionaryPartitions; }
            set { _DictionaryPartitions = value; }
        }

        #endregion protected properties

        #region public properties

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView TheGrid
        {
            get { return GridView1; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public Device SelectedDevice
        {
            get
            {
                if (Devices.Count == 0 || GridView1.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView1.PageIndex*GridView1.PageSize + GridView1.SelectedIndex;

                if (index < 0 || index > Devices.Count - 1)
                    return null;

                return Devices[index];
            }
            set
            {
                GridView1.SelectedIndex = Devices.IndexOf(value);
                if (OnDeviceSelectionChanged != null)
                    OnDeviceSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<Device> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                GridView1.DataSource = _devices; // must manually call DataBind() later
            }
        }


        /// <summary>
        /// Gets/Sets the height of device list panel.
        /// </summary>
        public Unit Height
        {
            get
            {
                if (ContainerTable != null)
                    return ContainerTable.Height;
                else
                    return _height;
            }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Defines the handler for <seealso cref="OnDeviceSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedDevice"></param>
        public delegate void DeviceSelectedEventHandler(object sender, Device selectedDevice);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event DeviceSelectedEventHandler OnDeviceSelectionChanged;

        #endregion // Events

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;


            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            GridView1.PagerSettings.Visible = false;
            GridView1.SelectedIndexChanged += GridView1_SelectedIndexChanged;
        }

        /// <summary>
        /// Updates the grid pager based on the current list.
        /// </summary>
        protected void UpdatePager()
        {
            #region update pager of the gridview if it is used

            if (GridView1.BottomPagerRow != null)
            {
                // Show Number of devices in the list
                Label lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PagerDeviceCountLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("{0} device(s)", Devices.Count);

                // Show current page and the number of pages for the list
                lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PagerPagingLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("Page {0} of {1}", GridView1.PageIndex + 1, GridView1.PageCount);

                // Enable/Disable the "Prev" page button
                ImageButton btn = GridView1.BottomPagerRow.Cells[0].FindControl("PagerPrevImageButton") as ImageButton;
                if (btn != null)
                {
                    if (Devices.Count == 0 || GridView1.PageIndex == 0)
                    {
                        btn.ImageUrl = "~/images/prev_disabled.gif";
                        btn.Enabled = false;
                    }
                    else
                    {
                        btn.ImageUrl = "~/images/prev.gif";
                        btn.Enabled = true;
                    }

                    btn.Style.Add("cursor", "hand");
                }

                // Enable/Disable the "Next" page button
                btn = GridView1.BottomPagerRow.Cells[0].FindControl("PagerNextImageButton") as ImageButton;
                if (btn != null)
                {
                    if (Devices.Count == 0 || GridView1.PageIndex == GridView1.PageCount - 1)
                    {
                        btn.ImageUrl = "~/images/next_disabled.gif";
                        btn.Enabled = false;
                    }
                    else
                    {
                        btn.ImageUrl = "~/images/next.gif";
                        btn.Enabled = true;
                    }

                    btn.Style.Add("cursor", "hand");
                }
            }

            #endregion
        }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView1.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Add OnClick attribute to each row to make javascript call "Select$###" (where ### is the selected row)
                    // This method when posted back will be handled by the grid
                    e.Row.Attributes["OnClick"] =
                        Page.ClientScript.GetPostBackEventReference(GridView1, "Select$" + e.Row.RowIndex);
                    e.Row.Style["cursor"] = "hand";

                    // For some reason, double-click won't work if single-click is used
                    // e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Edit$" + e.Row.RowIndex);

                    CustomizeActiveColumn(e);
                    CustomizeIpAddressColumn(e);
                    CustomizeDHCPColumn(e);
                    CustomizeServerPartitionColumn(e);
                    CustomizeFeaturesColumn(e);
                }
            }
        }

        protected void CustomizeFeaturesColumn(GridViewRowEventArgs e)
        {
            PlaceHolder placeHolder = e.Row.FindControl("FeaturePlaceHolder") as PlaceHolder;

            if (placeHolder != null)
            {
                // add an image for each enabled feature
                AddAllowStorageImage(e, placeHolder);
                AddAllowAutoRouteImage(e, placeHolder);
                AddAllowRetrieveImage(e, placeHolder);
                AddAllowQueryImage(e, placeHolder);
            }
        }

        private void AddAllowRetrieveImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            Image img;
            img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowRetrieve")))
            {
                img = new Image();
                img.ImageUrl = "~/images/icons/RetrieveSmall.png";
                img.AlternateText = "Retrieve";
            }
            else
            {
                //img.Visible = false;
                img.ImageUrl = "~/images/blankfeature.gif";
                img.AlternateText = "";
            }
            placeHolder.Controls.Add(img);
        }

        private void AddAllowQueryImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            Image img;
            img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowQuery")))
            {
                img = new Image();
                img.ImageUrl = "~/images/icons/QuerySmall.png";
                img.AlternateText = "Query";
            }
            else
            {
                //img.Visible = false;
                img.ImageUrl = "~/images/blankfeature.gif";
                img.AlternateText = "";
            }
            placeHolder.Controls.Add(img);
        }

        private void AddAllowStorageImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            Image img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowStorage")))
            {
                img.ImageUrl = "~/images/icons/StoreSmall.png";
                img.AlternateText = "Store";
            }
            else
            {
                //img.Visible = false;
                img.ImageUrl = "~/images/blankfeature.gif";
                img.AlternateText = "";
            }
            placeHolder.Controls.Add(img);
        }

        private void AddAllowAutoRouteImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            Image img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowAutoRoute")))
            {
                img.ImageUrl = "~/images/icons/AutoRouteSmall.png";
                img.AlternateText = "Auto Route";
            }
            else
            {
                //img.Visible = false;
                img.ImageUrl = "~/images/blankfeature.gif";
                img.AlternateText = "";
            }
            placeHolder.Controls.Add(img);
        }

        protected void CustomizeDHCPColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("DHCPImage"));
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

        protected void CustomizeActiveColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("ActiveImage"));

            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                if (active)
                    img.ImageUrl = "~/images/checked_small.gif";
                else
                {
                    img.ImageUrl = "~/images/unchecked_small.gif";
                }
            }
        }

        // Display the Partition Description in Server Partition column
        protected void CustomizeServerPartitionColumn(GridViewRowEventArgs e)
        {
            Device dev = e.Row.DataItem as Device;
            Label lbl = e.Row.FindControl("ServerParitionLabel") as Label; // The label is added in the template
            lbl.Text = dev.ServerPartition.AeTitle;
        }

        // Display the Partition Description in Server Partition column
        protected void CustomizeIpAddressColumn(GridViewRowEventArgs e)
        {
            Device dev = e.Row.DataItem as Device;
            Label lbl = e.Row.FindControl("IpAddressLabel") as Label; // The label is added in the template
            if (dev.Dhcp)
                lbl.Text = "";
            else
                lbl.Text = dev.IpAddress;
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Device dev = SelectedDevice;
            if (dev != null)
                if (OnDeviceSelectionChanged != null)
                    OnDeviceSelectionChanged(this, dev);
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            UpdatePager();
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void ImageButton_Command(object sender, CommandEventArgs e)
        {
            // get the current page selected
            int intCurIndex = GridView1.PageIndex;

            switch (e.CommandArgument.ToString().ToLower())
            {
                case "first":
                    GridView1.PageIndex = 0;
                    break;
                case "prev":
                    GridView1.PageIndex = intCurIndex - 1;
                    break;
                case "next":
                    GridView1.PageIndex = intCurIndex + 1;
                    break;
                case "last":
                    GridView1.PageIndex = GridView1.PageCount;
                    break;
            }

            DataBind();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Binds the list to the control.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting <seeaslo cref="Devices"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            GridView1.DataBind();

            GridView1.PagerSettings.Visible = false;
        }

        #endregion // public methods
    }
}
