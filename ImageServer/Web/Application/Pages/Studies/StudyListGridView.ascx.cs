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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    //
    //  Used to display the list of studies.
    //
    public partial class StudyListGridView : GridViewPanel
    {
		#region Delegates
		public delegate void StudyDataSourceCreated(StudyDataSource theSource);
		public event StudyDataSourceCreated DataSourceCreated;
		#endregion

        #region Private members
        // list of studies to display
        private IList<StudySummary> _studies;
        private ServerPartition _partition;
        private Unit _height;
    	private StudyDataSource _dataSource;
        private Dictionary<string, StudySummary> _studyDictionary = new Dictionary<string, StudySummary>();
        #endregion Private members

        #region Public properties

		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new StudyDataSource();

					_dataSource.StudyFoundSet += delegate(IList<StudySummary> newlist)
											{
												Studies = newlist;
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

        public ServerPartition Partition
        {
            set { _partition = value; }
            get { return _partition; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<StudySummary> SelectedStudies
        {
            get
            {
                if(!StudyListControl.IsDataBound) StudyListControl.DataBind();

                if (_studyDictionary.Count == 0)
                    return null;

                string[] rows = StudyListControl.SelectedDataKeys;
                if (rows == null || rows.Length == 0)
                    return null;

				IList<StudySummary> studies = new List<StudySummary>();
                for(int i=0; i<rows.Length; i++)
                {
                    studies.Add(_studyDictionary[rows[i]]);
                }

                return studies;
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<StudySummary> Studies
        {
            get
            {
                return _studies;
            }
            set
            {
                _studies = value;
                foreach(StudySummary study in _studies)
                {
                    _studyDictionary.Add(study.Key.ToString(), study);
                }
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
            	return _height;
            }
        	set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        public void SetDataSource()
        {
            StudyListControl.DataSource = StudyDataSourceObject;
        }

        public bool IsDataSourceSet()
        {
            return StudyListControl.DataSource != null;
        }

        #endregion
        
        #region protected methods
     
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = StudyListControl;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            if(IsPostBack || Page.IsAsync)
            {
                StudyListControl.DataSource = StudyDataSourceObject;    
            } 
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Studies == null)
            {
                return;
            } 

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    StudySummary study = Studies[row.RowIndex];

                    if (study != null)
                    {
                        row.Attributes.Add("instanceuid", study.TheStudy.StudyInstanceUid);
                        row.Attributes.Add("serverae", study.ThePartition.AeTitle);

                        string reason;
                        if (study.CanScheduleDelete(out reason))
                            row.Attributes.Add("candelete", "true");

                        if (study.CanScheduleMove(out reason))
                            row.Attributes.Add("canmove", "true");

                        if (study.CanScheduleRestore(out reason))
                            row.Attributes.Add("canrestore", "true");


                        LinkButton button = (LinkButton) row.FindControl("ReconcileLinkButton");
                        Label label = (Label)row.FindControl("SeparatorLabel");

                        if (study.IsReconcileRequired && Context.User.IsInRole(Enterprise.Authentication.AuthorityTokens.StudyIntegrityQueue.Search))
                        {
                            button.PostBackUrl = ImageServerConstants.PageURLs.StudyIntegrityQueuePage +
                                                 "?PatientID=" + study.PatientId + "&PatientName=" + study.PatientsName + "&PartitionKey=" + study.ThePartition.GetKey();
                            button.Visible = true;
                            label.Visible = true;
                        }
                        else
                        {
                            button.Visible = false;
                            label.Visible = false;
                        }

                        button = (LinkButton) row.FindControl("QueueLinkButton");
                        label = (Label) row.FindControl("QueueSeparatorLabel");

                        if(study.IsLocked)
                        {
							if (study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.RestoreScheduled))
							{
								if (Context.User.IsInRole(Enterprise.Authentication.AuthorityTokens.RestoreQueue.Search))
								{
									button.PostBackUrl = ImageServerConstants.PageURLs.RestoreQueuePage +
														 "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
									                     study.ThePartition.Key;
									button.Visible = true;
									button.Text = study.QueueStudyStateEnum.Description;
									label.Visible = true;
								}								
							}
							else if (study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ArchiveScheduled))
							{
								if (Context.User.IsInRole(Enterprise.Authentication.AuthorityTokens.ArchiveQueue.Search))
								{
									button.PostBackUrl = ImageServerConstants.PageURLs.ArchiveQueuePage +
														 "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
														 study.ThePartition.Key;
									button.Visible = true;
									button.Text = study.QueueStudyStateEnum.Description;
									label.Visible = true;									
								}
							}
							else
							{
								if (Context.User.IsInRole(Enterprise.Authentication.AuthorityTokens.WorkQueue.Search))
								{
									button.PostBackUrl = ImageServerConstants.PageURLs.WorkQueuePage +
									                     "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
									                     study.ThePartition.Key;
									button.Visible = true;
									button.Text = study.QueueStudyStateEnum.Description;
									label.Visible = true;
								}
							}
                        } else
                        {
                            button.Visible = false;
                            label.Visible = false;
                        }
                    }
                }
            }
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                EmptySearchResultsMessage message =
                                        (EmptySearchResultsMessage)e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    message.Message = TheGrid.DataSource == null ? "Please enter search criteria to find studies." : "No studies found matching the provided criteria.";
                }
            } 
        }

		protected void GetStudyDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
				_dataSource = new StudyDataSource();

				_dataSource.StudyFoundSet += delegate(IList<StudySummary> newlist)
										{
											Studies = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);

		}

        #endregion

    }

}
