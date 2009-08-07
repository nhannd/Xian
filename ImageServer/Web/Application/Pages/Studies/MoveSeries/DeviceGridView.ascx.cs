#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.ImageServer.Web.Application.Controls;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    public partial class DeviceGridView : GridViewPanel
    {
        #region Private members
        // list of devices to display
        private IList<Device> _devices;
        private Unit _height;
        #endregion Private members

        #region Public properties

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
                int index = GridView1.PageIndex * GridView1.PageSize + GridView1.SelectedIndex;

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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            TheGrid = GridView1;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView1.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
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
                img.ImageUrl = ImageServerConstants.ImageURLs.RetrieveFeature;
                img.AlternateText = "Retrieve";
            }
            else
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.Blank;
                img.AlternateText = string.Empty;
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
                img.ImageUrl = ImageServerConstants.ImageURLs.QueryFeature;
                img.AlternateText = "Query";
            }
            else
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.Blank;
                img.AlternateText = string.Empty;
            }
            placeHolder.Controls.Add(img);
        }

        private void AddAllowStorageImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            Image img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowStorage")))
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.StoreFeature;
                img.AlternateText = "Store";
            }
            else
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.Blank;
                img.AlternateText = string.Empty;
            }
            placeHolder.Controls.Add(img);
        }

        private void AddAllowAutoRouteImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            Image img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowAutoRoute")))
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.AutoRouteFeature;
                img.AlternateText = "Auto Route";
            }
            else
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.Blank;
                img.AlternateText = string.Empty;
            }
            placeHolder.Controls.Add(img);
        }

        protected void CustomizeDHCPColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image)e.Row.FindControl("DHCPImage"));
            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "DHCP"));
                if (active)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        protected void CustomizeActiveColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image)e.Row.FindControl("ActiveImage"));

            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                if (active)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        // Display the Partition Description in Server Partition column
        protected void CustomizeServerPartitionColumn(GridViewRowEventArgs e)
        {
            Device dev = e.Row.DataItem as Device;
            if (dev == null) return;
            Label lbl = e.Row.FindControl("ServerParitionLabel") as Label; // The label is added in the template
            if (lbl == null) return;
            lbl.Text = dev.ServerPartition.AeTitle;
        }

        // Display the Partition Description in Server Partition column
        protected void CustomizeIpAddressColumn(GridViewRowEventArgs e)
        {
            Device dev = e.Row.DataItem as Device;
            if (dev==null) return;
            Label lbl = e.Row.FindControl("IpAddressLabel") as Label; // The label is added in the template
            if (lbl == null) return;
            if (dev.Dhcp)
                lbl.Text = string.Empty;
            else
                lbl.Text = dev.IpAddress;
        }

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
        }

        #endregion // public methods
    }
}