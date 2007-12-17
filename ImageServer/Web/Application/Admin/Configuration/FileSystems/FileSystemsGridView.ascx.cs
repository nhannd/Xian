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


namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems
{
    //
    //  Used to display the list of devices.
    //
    public partial class FileSystemsGridView : System.Web.UI.UserControl
    {
        #region private members
        private IList<Filesystem> _fileSystems;
        #endregion Private members

        #region protected properties

        #endregion protected properties

        #region public properties

        /// <summary>
        /// Retrieve reference to the grid control being used to display the filesystems.
        /// </summary>
        public GridView TheGrid
        {
            get { return GridView1; }
        }

        /// <summary>
        /// Gets/Sets the current selected FileSystem.
        /// </summary>
        public Filesystem SelectedFileSystem
        {
            get
            {
                if (FileSystems.Count == 0 || GridView1.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView1.PageIndex * GridView1.PageSize + GridView1.SelectedIndex;

                if (index < 0 || index > FileSystems.Count - 1)
                    return null;

                return FileSystems[index];
            }
            set
            {

                GridView1.SelectedIndex = FileSystems.IndexOf(value);
                if (FileSystemSelectionChanged != null)
                    FileSystemSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of file systems rendered on the screen.
        /// </summary>
        public IList<Filesystem> FileSystems
        {
            get
            {
                return _fileSystems;
            }
            set
            {
                _fileSystems = value;
                GridView1.DataSource = _fileSystems; // must manually call DataBind() later
            }
        }

       
        #endregion

        #region Events
        /// <summary>
        /// Defines the handler for <seealso cref="FileSystemSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedFileSystem"></param>
        public delegate void FileSystemSelectedEventHandler(object sender, Filesystem selectedFileSystem);

        /// <summary>
        /// Occurs when the selected filesystem in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected filesystem can change programmatically or by users selecting the filesystem in the list.
        /// </remarks>
        public event FileSystemSelectedEventHandler FileSystemSelectionChanged;

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

            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            GridView1.PagerSettings.Visible = false;
            GridView1.SelectedIndexChanged +=new EventHandler(GridView1_SelectedIndexChanged);

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
                Label lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PagerFileSystemCountLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("{0} device(s)", this.FileSystems.Count);

                // Show current page and the number of pages for the list
                lbl = GridView1.BottomPagerRow.Cells[0].FindControl("PagerPagingLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("Page {0} of {1}", GridView1.PageIndex + 1, GridView1.PageCount);

                // Enable/Disable the "Prev" page button
                ImageButton btn = GridView1.BottomPagerRow.Cells[0].FindControl("PagerPrevImageButton") as ImageButton;
                if (btn != null)
                {
                    if (this.FileSystems.Count == 0 || GridView1.PageIndex == 0)
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
                    if (this.FileSystems.Count == 0 || GridView1.PageIndex == GridView1.PageCount - 1)
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
                    e.Row.Attributes["OnClick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Select$" + e.Row.RowIndex);
                    e.Row.Style["cursor"] = "hand";

                    // For some reason, double-click won't work if single-click is used
                    // e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Edit$" + e.Row.RowIndex);

                    CustomizePathColumn(e.Row);
                    CustomizeEnabledColumn(e);
                    CustomizeReadColumn(e);
                    CustomizeWriteColumn(e);
                    CustomizeFilesystemTierColumn(e.Row);
                    CustomizeUsageColumn(e.Row);
                }

            }

        }

        private void CustomizeUsageColumn(GridViewRow row)
        {
            Filesystem fs = row.DataItem as Filesystem;
            Image img = row.FindControl("UsageImage") as Image;

            if (img != null)
            {
                img.ImageUrl = string.Format("~/Common/BarChart.aspx?pct={0}&high={1}&low={2}",
                                        fs.PercentFull,
                                        fs.HighWatermark,
                                        fs.LowWatermark);
                img.AlternateText = string.Format("Current Usage   : {0}%\nHigh Watermark : {1}%\nLow Watermark  : {2}%",
                                        fs.PercentFull,
                                        fs.HighWatermark,
                                        fs.LowWatermark);

            }
        }
        private void CustomizePathColumn(GridViewRow row)
        {
            Filesystem fs = row.DataItem as Filesystem;
            Label lbl = row.FindControl("PathLabel") as Label; // The label is added in the template
            
            if (fs.FilesystemPath!=null)
            {
                // truncate it
                if (fs.FilesystemPath.Length>50)
                {
                    lbl.Text = fs.FilesystemPath.Substring(0, 45) + "...";
                    lbl.ToolTip = string.Format("{0}: {1}", fs.Description, fs.FilesystemPath);
                }
                else
                {
                    lbl.Text = fs.FilesystemPath;
                }
            }
        }

        private void CustomizeFilesystemTierColumn(GridViewRow row)
        {
            Filesystem fs = row.DataItem as Filesystem;
            Label lbl = row.FindControl("FilesystemTierDescription") as Label; // The label is added in the template
            lbl.Text = fs.FilesystemTierEnum.Description;
        }

        

        private void CustomizeBooleanColumn(GridViewRow row, string controlName, string fieldName)
        {
            Image img = ((Image)row.FindControl(controlName));
            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(row.DataItem, fieldName));
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

        protected  void CustomizeReadColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image)e.Row.FindControl("ReadImage"));
            if (img != null)
            {
                bool enabled = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                bool readOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ReadOnly"));
                bool writeOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "WriteOnly"));

                bool canRead = enabled && (readOnly || ( /*not readonly and */ !writeOnly));

                if (canRead)
                {
                    img.ImageUrl = "~/images/checked_small.gif";
                }
                else
                {
                    img.ImageUrl = "~/images/unchecked_small.gif";
                }
            }
        }

        protected void CustomizeEnabledColumn(GridViewRowEventArgs e)
        {
            CustomizeBooleanColumn(e.Row, "EnabledImage", "Enabled");
        }

        protected void CustomizeWriteColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image)e.Row.FindControl("WriteImage"));
            if (img != null)
            {
                bool enabled = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                bool readOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ReadOnly"));
                bool writeOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "WriteOnly"));

                bool canWrite = enabled && (writeOnly || ( /*not write only and also */ !readOnly));

                if (canWrite)
                {
                    img.ImageUrl = "~/images/checked_small.gif";
                }
                else
                {
                    img.ImageUrl = "~/images/unchecked_small.gif";
                }
            }
        }
        
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filesystem filesystem = SelectedFileSystem;
            if (filesystem != null)
                if (FileSystemSelectionChanged != null)
                    FileSystemSelectionChanged(this, filesystem);
            
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
           // DataBind();
        }
        
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
          //  GridView1.PageIndex = e.NewPageIndex;
          //  DataBind();
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
        /// This method must be called after setting <seeaslo cref="FileSystems"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            GridView1.DataBind();

            GridView1.PagerSettings.Visible = false;

        }

        #endregion // public methods

    }

}
