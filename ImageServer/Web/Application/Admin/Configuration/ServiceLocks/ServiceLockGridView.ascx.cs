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


namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServiceLocks
{
    //
    //  Used to display the list of services.
    //
    public partial class ServiceLockGridView : UserControl
    {
        #region private members
        private ServiceLockCollection _services;
        private Unit _height;
        #endregion Private members

        #region protected properties

        #endregion protected properties

        #region public properties

        /// <summary>
        /// Retrieve reference to the grid control being used to display the services.
        /// </summary>
        public GridView TheGrid
        {
            get { return GridView1; }
        }

        /// <summary>
        /// Gets/Sets the current selected service.
        /// </summary>
        public ServiceLock SelectedServiceLock
        {
            get
            {
                if (ServiceLocks==null || ServiceLocks.Count == 0 || GridView1.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView1.PageIndex*GridView1.PageSize + GridView1.SelectedIndex;

                if (index < 0 || index > ServiceLocks.Count - 1)
                    return null;

                return ServiceLocks[index];
            }
            set
            {
                GridView1.SelectedIndex = ServiceLocks.IndexOf(value);
                if (OnServiceLockSelectionChanged != null)
                    OnServiceLockSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of services rendered on the screen.
        /// </summary>
        public ServiceLockCollection ServiceLocks
        {
            get { return _services; }
            set
            {
                _services = value;
                GridView1.DataSource = _services; // must manually call DataBind() later
            }
        }


        /// <summary>
        /// Gets/Sets the height of service list panel.
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
        /// Defines the handler for <seealso cref="OnServiceLockSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedServiceLock"></param>
        public delegate void ServiceLockSelectedEventHandler(object sender, ServiceLock selectedServiceLock);

        /// <summary>
        /// Occurs when the selected service in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected service can change programmatically or by users selecting the service in the list.
        /// </remarks>
        public event ServiceLockSelectedEventHandler OnServiceLockSelectionChanged;

        #endregion // Events

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
           
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
        }

        /// <summary>
        /// Updates the grid pager based on the current list.
        /// </summary>
        protected void UpdatePager()
        {
            #region update pager of the gridview if it is used

            if (GridView1.BottomPagerRow != null)
            {
                // Show Number of services in the list
                Label lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PagerServiceLockCountLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("{0} service(s)", ServiceLocks.Count);

                // Show current page and the number of pages for the list
                lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PagerPagingLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("Page {0} of {1}", GridView1.PageIndex + 1, GridView1.PageCount);

                // Enable/Disable the "Prev" page button
                ImageButton btn = GridView1.BottomPagerRow.Cells[0].FindControl("PagerPrevImageButton") as ImageButton;
                if (btn != null)
                {
                    if (ServiceLocks.Count == 0 || GridView1.PageIndex == 0)
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
                    if (ServiceLocks.Count == 0 || GridView1.PageIndex == GridView1.PageCount - 1)
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

                    CustomizeEnabledColumn(e.Row);
                    CustomizeLockColumn(e.Row);
                }
            }
        }

        protected void CustomizeEnabledColumn(GridViewRow row)
        {
            Image img = row.FindControl("EnabledImage") as Image;

            ServiceLock item = row.DataItem as ServiceLock;
            if (img!=null && item != null)
            {
                img.ImageUrl = item.Enabled ? "~/images/checked_small.gif" : "~/images/unchecked_small.gif";
            }
        }
        protected void CustomizeLockColumn(GridViewRow row)
        {
            Image img = row.FindControl("LockedImage") as Image;

            ServiceLock item = row.DataItem as ServiceLock;
            if (img != null && item != null)
            {
                img.ImageUrl = item.Lock ? "~/images/checked_small.gif" : "~/images/unchecked_small.gif";
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBind();
            ServiceLock dev = SelectedServiceLock;
            if (dev != null)
                if (OnServiceLockSelectionChanged != null)
                    OnServiceLockSelectionChanged(this, dev);
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
            if (SelectedServiceLock != null)
            {
                GridView1.SelectedIndex = ServiceLocks.RowIndexOf(SelectedServiceLock, GridView1);
            }
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
        /// This method must be called after setting <seeaslo cref="ServiceLocks"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            GridView1.DataSource = ServiceLocks.Values;


            GridView1.PagerSettings.Visible = false;
            base.DataBind();
        }


        public void Refresh()
        {
            UpdatePanel1.Update();
        }

        #endregion // public methods
    }
}
