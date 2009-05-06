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
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    //
    //  Used to display the list of Archive Queue Items.
    //
    public partial class AlertsGridPanel : System.Web.UI.UserControl
    {
        #region Delegates
        public delegate void AlertDataSourceCreated(AlertDataSource theSource);
        public event AlertDataSourceCreated DataSourceCreated;
        #endregion

        #region Private members
        // list of studies to display
        private AlertItemCollection _alertCollection;
        private Unit _height;
        private AlertDataSource _dataSource;
        #endregion Private members

        #region Public properties

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new AlertDataSource();

                    _dataSource.AlertFoundSet += delegate(IList<AlertSummary> newlist)
                                            {
                                                AlertItems = new AlertItemCollection(newlist);
                                            };
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);
                    _dataSource.SelectCount();
                }
                if (_dataSource.ResultCount == 0)
                {
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);

                    _dataSource.SelectCount();
                }
                return _dataSource.ResultCount;
            }
        }

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView AlertGrid
        {
            get { return AlertGridView; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<Model.Alert> SelectedItems
        {
            get
            {
                if(!AlertGridView.IsDataBound) AlertGridView.DataBind();
                
                if (AlertItems == null || AlertItems.Count == 0)
                    return null;

                int[] rows = AlertGridView.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<Alert> queueItems = new List<Model.Alert>();
                for (int i = 0; i < rows.Length; i++)
                {
                    if (rows[i] < AlertItems.Count)
                    {
                        queueItems.Add(AlertItems[rows[i]].TheAlertItem);
                    }
                }

                return queueItems;
            }
        }

        /// <summary>
        /// Gets/Sets the list of Alert Items
        /// </summary>
        public AlertItemCollection AlertItems
        {
            get
            {
                return _alertCollection;
            }
            set
            {
                _alertCollection = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
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
        /// Gets/Sets a key of the selected work queue item.
        /// </summary>
        public AlertSummary SelectedAlert
        {
            get
            {
                if (SelectedAlertKey != null && AlertItems.ContainsKey(SelectedAlertKey))
                {
                    return AlertItems[SelectedAlertKey];
                }
                else
                    return null;
            }
            set
            {
                SelectedAlertKey = value.Key;
                AlertGridView.SelectedIndex = AlertItems.RowIndexOf(SelectedAlertKey, AlertGridView);
            }
        }

        public void RefreshCurrentPage()
        {
            DataBind();
        }

        #endregion

        #region protected methods

        protected ServerEntityKey SelectedAlertKey
        {
            set
            {
                ViewState["SelectedAlertKey"] = value;
            }
            get
            {
                return ViewState["SelectedAlertKey"] as ServerEntityKey;
            }
        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            AlertGridView.DataSource = AlertDataSourceObject;
        }

        protected void AlertGridView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
            if (SelectedAlertKey != null)
            {
                AlertGridView.SelectedIndex = AlertItems.RowIndexOf(SelectedAlertKey, AlertGridView);
            }
        }

        protected void AlertGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void AlertGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            AlertGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void AlertGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (AlertGridView.EditIndex != e.Row.RowIndex)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {                   
                    AlertSummary alert = e.Row.DataItem as AlertSummary;
                    Label level = e.Row.FindControl("Level") as Label;
                   
                    if(level != null && alert != null)
                    {
                        if (alert.Level.Equals("Error") || alert.Level.Equals("Critical"))
                        {
                            level.ForeColor = Color.Red;
                        }
                        level.Text = alert.Level;
                    }
                }
            }
        }

        protected void DisposeAlertDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected void GetAlertDataSource(object sender, ObjectDataSourceEventArgs e)
        {
            if (_dataSource == null)
            {
                _dataSource = new AlertDataSource();

                _dataSource.AlertFoundSet += delegate(IList<AlertSummary> newlist)
                                        {
                                            AlertItems = new AlertItemCollection(newlist);
                                        };
            }

            e.ObjectInstance = _dataSource;

            if (DataSourceCreated != null)
                DataSourceCreated(_dataSource);

        }

        #endregion

    }

}
