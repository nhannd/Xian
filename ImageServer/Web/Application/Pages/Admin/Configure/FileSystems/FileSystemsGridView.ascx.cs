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
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems
{
    //
    //  Used to display the list of devices.
    //
    public partial class FileSystemsGridView : UserControl
    {
        #region private members

        private IList<Filesystem> _fileSystems;
        private Unit _height;
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
        /// Gets/Sets the height of the filesystem list panel.
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
                int index = GridView1.PageIndex*GridView1.PageSize + GridView1.SelectedIndex;

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
            get { return _fileSystems; }
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


        #region private methods

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView1.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeUsageColumn(e.Row);
                    CustomizePathColumn(e.Row);
                    CustomizeEnabledColumn(e);
                    CustomizeReadColumn(e);
                    CustomizeWriteColumn(e);
                    CustomizeFilesystemTierColumn(e.Row);
                }
            }
        }


        private float GetFilesystemUsedPercentage(Filesystem fs)
        {
            if (!isServiceAvailable())
                return float.NaN;

            FilesystemServiceProxy.FilesystemServiceClient client = new FilesystemServiceProxy.FilesystemServiceClient();

            try
            {
                FilesystemServiceProxy.FilesystemInfo fsInfo = client.GetFilesystemInfo(fs.FilesystemPath);

                _serviceIsOffline = false;
                _lastServiceAvailableTime = Platform.Time;
                return 100.0f - ((float)fsInfo.FreeSizeInKB) / fsInfo.SizeInKB * 100.0F;
            }
            catch (Exception)
            {
                _serviceIsOffline = true;
                _lastServiceAvailableTime = Platform.Time;
            }
            finally
            {
                if (client.State == CommunicationState.Opened)
                    client.Close();
            }

            return float.NaN;
        }

        private void CustomizeUsageColumn(GridViewRow row)
        {
            Filesystem fs = row.DataItem as Filesystem;
            Image img = row.FindControl("UsageImage") as Image;

            float usage = GetFilesystemUsedPercentage(fs);
            if (img != null)
            {
                img.ImageUrl = string.Format(ImageServerConstants.PageURLs.BarChartPage,
                                             usage,
                                             fs.HighWatermark,
                                             fs.LowWatermark);
                img.AlternateText =
                    string.Format("Current Usage   : {0}\nHigh Watermark : {1}%\nLow Watermark  : {2}%",
                                  float.IsNaN(usage) ? "Unknown" : usage.ToString() + "%",
                                  fs.HighWatermark,
                                  fs.LowWatermark);
            }
        }

        private void CustomizePathColumn(GridViewRow row)
        {
            Filesystem fs = row.DataItem as Filesystem;
            Label lbl = row.FindControl("PathLabel") as Label; // The label is added in the template

            if (fs.FilesystemPath != null)
            {
                // truncate it
                if (fs.FilesystemPath.Length > 50)
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
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        #endregion

        #region protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

        }

        #endregion   

        protected void CustomizeReadColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("ReadImage"));
            if (img != null)
            {
                bool enabled = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                bool readOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ReadOnly"));
                bool writeOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "WriteOnly"));

                bool canRead = enabled && (readOnly || ( /*not readonly and */ !writeOnly));

                if (canRead)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        protected void CustomizeEnabledColumn(GridViewRowEventArgs e)
        {
            CustomizeBooleanColumn(e.Row, "EnabledImage", "Enabled");
        }

        protected void CustomizeWriteColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("WriteImage"));
            if (img != null)
            {
                bool enabled = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                bool readOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ReadOnly"));
                bool writeOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "WriteOnly"));

                bool canWrite = enabled && (writeOnly || ( /*not write only and also */ !readOnly));

                if (canWrite)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            TheGrid.DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            TheGrid.DataBind();
        }

        #region Private Static members
        static private bool _serviceIsOffline = false;
        static private DateTime _lastServiceAvailableTime = Platform.Time;

        /// <summary>
        /// Return a value indicating whether the last web service call was successful.
        /// </summary>
        /// <returns></returns>
        static private bool isServiceAvailable()
        {
            TimeSpan elapsed = Platform.Time - _lastServiceAvailableTime;
            return (!_serviceIsOffline || /*service was offline but */ elapsed.Seconds > 15);
        }

        #endregion Private Static members

        #region public methods

        /// <summary>
        /// Binds the list to the control.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting <seeaslo cref="FileSystems"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            TheGrid.DataBind();
        }

        #endregion // public methods
    }
}
