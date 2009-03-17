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
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    //
    //  Used to display the list of Restore Queue Items.
    //
    public partial class StudyIntegrityQueueItemList : System.Web.UI.UserControl
    {
		#region Delegates
		public delegate void StudyIntegrityQueueDataSourceCreated(StudyIntegrityQueueDataSource theSource);
		public event StudyIntegrityQueueDataSourceCreated DataSourceCreated;
		#endregion

        #region Private members
        // list of studies to display
        private IList<StudyIntegrityQueueSummary> _queueItems;
        private Unit _height;
    	private StudyIntegrityQueueDataSource _dataSource;
        #endregion Private members

        #region Public properties

		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new StudyIntegrityQueueDataSource();

                    _dataSource.StudyIntegrityQueueFoundSet += delegate(IList<StudyIntegrityQueueSummary> newlist)
											{
												Items = newlist;
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
        public Web.Common.WebControls.UI.GridView StudyIntegrityQueueGrid
        {
            get { return StudyIntegrityQueueGridView; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<StudyIntegrityQueueSummary> SelectedItems
        {
            get
            {
                if(!StudyIntegrityQueueGridView.IsDataBound) StudyIntegrityQueueGridView.DataBind();

                if (Items==null || Items.Count == 0)
                    return null;

                int[] rows = StudyIntegrityQueueGridView.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<StudyIntegrityQueueSummary> queueItems = new List<StudyIntegrityQueueSummary>();
                for(int i=0; i<rows.Length; i++)
                {
                    if (rows[i] < Items.Count)
                    {
                        queueItems.Add(Items[rows[i]]);
                    }
                }

                return queueItems;
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<StudyIntegrityQueueSummary> Items
        {
            get
            {
                return _queueItems;
            }
            set
            {
                _queueItems = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get {
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
        /// Defines the handler for <seealso cref="OnQueueItemSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItems"></param>
        public delegate void QueueItemSelectedEventHandler(object sender, IList<StudyIntegrityQueueSummary> selectedItems);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event QueueItemSelectedEventHandler OnQueueItemSelectionChanged;

        #endregion // Events
        
        #region protected methods
       
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            StudyIntegrityQueueGridView.SelectedIndexChanged += StudyIntegrityQueueGridView_SelectedIndexChanged;

            StudyIntegrityQueueGridView.DataSource = StudyIntegrityQueueDataSourceObject;
        }

        protected void StudyIntegrityQueueGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<StudyIntegrityQueueSummary> queueItems = SelectedItems;
            if (queueItems != null)
                if (OnQueueItemSelectionChanged != null)
                    OnQueueItemSelectionChanged(this, queueItems);            
        }

        protected void StudyIntegrityQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void StudyIntegrityQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StudyIntegrityQueueGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

		protected void DisposeStudyIntegrityQueueDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
		{
			e.Cancel = true;
		}

		protected void GetStudyIntegrityQueueDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
                _dataSource = new StudyIntegrityQueueDataSource();

                _dataSource.StudyIntegrityQueueFoundSet += delegate(IList<StudyIntegrityQueueSummary> newlist)
										{
											Items = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);

		}

        protected void StudyIntegrityQueueItemList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (StudyIntegrityQueueGridView.EditIndex != e.Row.RowIndex)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeRowAttribute(e.Row);
                    CustomizeColumns(e.Row);
                }
            }
        }

        private void CustomizeRowAttribute(GridViewRow row)
        {
            StudyIntegrityQueueSummary item = row.DataItem as StudyIntegrityQueueSummary;
            row.Attributes["canreconcile"] = item.CanReconcile ? "true" : "false";
        }

        private void CustomizeColumns(GridViewRow row)
        {
            StudyIntegrityQueueSummary summary = row.DataItem as StudyIntegrityQueueSummary;

            if (summary != null)
            {
                Label existingPatientName = row.FindControl("ExistingPatientName") as Label;
                Label conflictingPatientName = row.FindControl("ConflictingPatientName") as Label;
                Label existingPatientId = row.FindControl("ExistingPatientId") as Label;
                Label conflictingPatientId = row.FindControl("ConflictingPatientId") as Label;
                Label existingAccessionNumber = row.FindControl("ExistingAccessionNumber") as Label;
                Label conflictingAccessionNumber = row.FindControl("ConflictingAccessionNumber") as Label;
                Label existingModalities = row.FindControl("ExistingModalities") as Label;
                Label conflictingModalities = row.FindControl("ConflictingModalities") as Label;
                
                if(existingPatientId != null)
                    existingPatientId.Text = summary.ExistingPatientId;
                if(existingPatientName != null)
                    existingPatientName.Text = summary.ExistingPatientName;
                if(existingAccessionNumber != null)
                    existingAccessionNumber.Text = summary.ExistingAccessionNumber;
                if(conflictingPatientId != null)
                    conflictingPatientId.Text = summary.ConflictingPatientId;
                if (conflictingPatientId != null)
                {
                    conflictingPatientId.Text = summary.ConflictingPatientId;
//                    if (!summary.ExistingPatientId.Equals(summary.ConflictingPatientId))
  //                      conflictingPatientId.ForeColor = Color.Red;
                }
                if (conflictingPatientName != null)
                {
                    conflictingPatientName.Text = summary.ConflictingPatientName;
                    if (!summary.ExistingPatientName.Equals(summary.ConflictingPatientName))
                        conflictingPatientName.ForeColor = Color.Red;
                }
                if (conflictingAccessionNumber != null) {
                    conflictingAccessionNumber.Text = summary.ConflictingAccessionNumber;
                    if (!summary.ExistingAccessionNumber.Equals(summary.ConflictingAccessionNumber))
                        conflictingAccessionNumber.ForeColor = Color.Red;
                }

                if (conflictingModalities!=null)
                {
                    conflictingModalities.Text = StringUtilities.Combine(summary.ConflictingModalities, ",");
                }
                if (existingModalities!=null)
                {
                    existingModalities.Text = summary.StudySummary.ModalitiesInStudy;
                }
                

                DateTimeLabel timeReceived = row.FindControl("TimeReceived") as DateTimeLabel;
                if (timeReceived != null)
                    timeReceived.Value = summary.ReceivedTime;
            }
        }
        #endregion

        #region public methods

        public void Refresh()
        {
            StudyIntegrityQueueGrid.ClearSelections();
            StudyIntegrityQueueGrid.PageIndex = 0;
            StudyIntegrityQueueGrid.DataBind();
        }

        public void RefreshCurrentPage()
        {
            StudyIntegrityQueueGrid.DataBind();
        }



        #endregion // public methods

    }

}
