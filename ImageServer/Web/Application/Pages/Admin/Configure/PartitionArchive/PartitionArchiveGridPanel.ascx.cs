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
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.ImageServer.Web.Common.Data;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    /// <summary>
    /// Partition list view panel.
    /// </summary>
    public partial class PartitionArchiveGridPanel : UserControl
    {
        #region Private Members

        /// <summary>
        /// list of partitions rendered on the screen.
        /// </summary>
        private IList<Model.PartitionArchive> _partitions;
        private Unit _height;
        #endregion private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the list of partitions rendered on the screen.
        /// </summary>
        public IList<Model.PartitionArchive> Partitions
        {
            get { return _partitions; }
            set
            {
                _partitions = value;
                PartitionGridView.DataSource = _partitions;
            }
        }

        /// <summary>
        /// Retrieves a reference to the embedded grid.
        /// </summary>
        public GridView TheGrid
        {
            get { return PartitionGridView; }
        }

        /// <summary>
        /// Retrieve the current selected partition.
        /// </summary>
        public Model.PartitionArchive SelectedPartition
        {
            get
            {
                if (Partitions.Count == 0 || PartitionGridView.SelectedIndex < 0)
                    return null;
                
                int index = TheGrid.PageIndex*TheGrid.PageSize + TheGrid.SelectedIndex;

                if (index < 0 || index >= Partitions.Count)
                    return null;

                return Partitions[index];
            }
        }

        /// <summary>
        /// Gets/Sets the height of server partition list panel.
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

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Height != Unit.Empty)
                ContainerTable.Height = _height;
        }

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            DataBind();
            
            SearchUpdatePanel.Update(); // force refresh
            ((Default) Page).UpdateUI();
        }

        protected void PartitionGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (PartitionGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Model.PartitionArchive pa = e.Row.DataItem as Model.PartitionArchive;
                    Label archiveTypeLabel = e.Row.FindControl("ArchiveType") as Label;
                    archiveTypeLabel.Text = pa.ArchiveTypeEnum.Description;

                    Label configXml = e.Row.FindControl("ConfigurationXML") as Label;
                    configXml.Text = XmlUtils.GetXmlDocumentAsString(pa.ConfigurationXml, true);

                    Image img = ((Image) e.Row.FindControl("EnabledImage"));
                    if (img != null)
                    {
                        img.ImageUrl = pa.Enabled
                                           ? ImageServerConstants.ImageURLs.Checked
                                           : ImageServerConstants.ImageURLs.Unchecked;
                    }

                    img = ((Image) e.Row.FindControl("ReadOnlyImage"));
                    if (img != null)
                    {
                        img.ImageUrl = pa.ReadOnly
                                           ? ImageServerConstants.ImageURLs.Checked
                                           : ImageServerConstants.ImageURLs.Unchecked;
                    }
                }
            }
        }

        #endregion Public methods
    }
}