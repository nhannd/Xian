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
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members
        private ServerPartition _serverPartition;
        private StudyController _controller = new StudyController();

    	#endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return this.DeleteStudyButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenButtonClientID")]
        public string OpenButtonClientID
        {
            get { return this.ViewStudyDetailsButton.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("RestoreButtonClientID")]
		public string RestoreButtonClientID
		{
			get { return this.RestoreStudyButton.ClientID; }
		}

        [ExtenderControlProperty]
        [ClientPropertyName("SendButtonClientID")]
        public string SendButtonClientID
        {
            get { return this.MoveStudyButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("StudyListClientID")]
        public string StudyListClientID
        {
            get { return StudyListGridView.StudyListGrid.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenStudyPageUrl")]
        public string OpenStudyPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SendStudyPageUrl")]
        public string SendStudyPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.MoveStudyPage); }
        }

        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        #endregion Public Properties  

        #region Private Methods

        private void SetupChildControls()
        {
            StudyDateCalendarExtender.Format = DateTimeFormatter.DefaultDateFormat;

            ClearStudyDateButton.OnClientClick = "document.getElementById('" + StudyDate.ClientID + "').value=''; return false;";
            
            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerStudySingleItem, App_GlobalResources.SR.GridPagerStudyMultipleItems, StudyListGridView.StudyListGrid);
            GridPagerBottom.InitializeGridPager(App_GlobalResources.SR.GridPagerStudySingleItem, App_GlobalResources.SR.GridPagerStudyMultipleItems, StudyListGridView.StudyListGrid);
            GridPagerTop.GetRecordCountMethod = delegate
                              {
                                  return StudyListGridView.ResultCount;
                              };
            GridPagerBottom.GetRecordCountMethod = delegate
                                          {
                                              return StudyListGridView.ResultCount;
                                          };

            DeleteMessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Study>)
                                {
                                    IList<Study> studies = data as IList<Study>;
                                    foreach (Study study in studies)
                                    {
                                        _controller.DeleteStudy(study);
                                    }
                                }
                                else if (data is Study)
                                {
                                    Study study = data as Study;
                                    _controller.DeleteStudy(study);
                                }

                                DataBind();
                                UpdatePanel.Update(); // force refresh
                            };

            RestoreMessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Study>)
                                {
                                    IList<Study> studies = data as IList<Study>;
                                    foreach (Study study in studies)
                                    {
                                        _controller.RestoreStudy(study);
                                    }
                                }
                                else if (data is Study)
                                {
                                    Study study = data as Study;
                                    _controller.RestoreStudy(study);
                                }

                                DataBind();
                                UpdatePanel.Update(); // force refresh
                            };

            StudyListGridView.DataSourceCreated += delegate(StudyDataSource source)
                                        {
                                            source.Partition = ServerPartition;
                                            source.DateFormats = StudyDateCalendarExtender.Format;

                                            if (!String.IsNullOrEmpty(PatientId.Text))
                                                source.PatientId = PatientId.Text;
                                            if (!String.IsNullOrEmpty(PatientName.Text))
                                                source.PatientName = PatientName.Text;
                                            if (!String.IsNullOrEmpty(AccessionNumber.Text))
                                                source.AccessionNumber = AccessionNumber.Text;
                                            if (!String.IsNullOrEmpty(StudyDate.Text))
                                                source.StudyDate = StudyDate.Text;
                                            if (!String.IsNullOrEmpty(StudyDescription.Text))
                                                source.StudyDescription = StudyDescription.Text;
                                        };
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            AccessionNumber.Text = string.Empty;
            StudyDescription.Text = string.Empty;
            StudyDate.Text = string.Empty;
        }

        public override void DataBind()
        {
            StudyListGridView.Partition = ServerPartition;
            base.DataBind();
            StudyListGridView.DataBind();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetupChildControls();           
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            StudyDate.Text = Request[StudyDate.UniqueID];
            if (!String.IsNullOrEmpty(StudyDate.Text))
                StudyDateCalendarExtender.SelectedDate = DateTime.ParseExact(StudyDate.Text, StudyDateCalendarExtender.Format, null);
            else
                StudyDateCalendarExtender.SelectedDate = null;

			if (StudyListGridView.IsPostBack)
			{
				DataBind();
			} 
        }

        protected override void OnPreRender(EventArgs e)
        {

			UpdateUI();
			base.OnPreRender(e);
        }

        protected void UpdateUI()
        {
            UpdateToolbarButtonState();
            
        }
        
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyListGridView.StudyListGrid.ClearSelections();
        	StudyListGridView.StudyListGrid.PageIndex = 0;
			DataBind();
        }

        protected void DeleteStudyButton_Click(object sender, EventArgs e)
        {
            IList<Study> studies = StudyListGridView.SelectedStudies;

            if (studies != null && studies.Count>0)
            {
                string message = studies.Count > 1 ? string.Format(App_GlobalResources.SR.MultipleStudyDelete) :
                                                     string.Format(App_GlobalResources.SR.SingleStudyDelete);

                DeleteMessageBox.Message = DialogHelper.createConfirmationMessage(message);
                DeleteMessageBox.Message += DialogHelper.createStudyTable(studies);

                DeleteMessageBox.Title = App_GlobalResources.Titles.DeleteStudyConfirmation;
                DeleteMessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
                DeleteMessageBox.Data = studies;
                DeleteMessageBox.Show();
            }
        }

		protected void RestoreStudyButton_Click(object sender, ImageClickEventArgs e)
		{
			IList<Study> studies = StudyListGridView.SelectedStudies;

			if (studies != null && studies.Count > 0)
			{
			    string message = studies.Count > 1 ? string.Format(App_GlobalResources.SR.MultipleStudyRestore):
				                                    string.Format(App_GlobalResources.SR.SingleStudyRestore);

			    RestoreMessageBox.Message = DialogHelper.createConfirmationMessage(message);
                RestoreMessageBox.Message += DialogHelper.createStudyTable(studies);
				
			    RestoreMessageBox.Title = App_GlobalResources.Titles.RestoreStudyConfirmation;
                RestoreMessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
				RestoreMessageBox.Data = studies;
				RestoreMessageBox.Show();
			}
		}

        protected void UpdateToolbarButtonState()
        {
            IList<Study> studies = StudyListGridView.SelectedStudies;
            if (studies != null)
            {
				RestoreStudyButton.Enabled = true;
				foreach (Study study in studies)
				{
					if (!study.StudyStatusEnum.Equals(StudyStatusEnum.Nearline))
					{
						RestoreStudyButton.Enabled = false;
						break;
					}
				}


            	ViewStudyDetailsButton.Enabled = true;
				DeleteStudyButton.Enabled = true;
                MoveStudyButton.Enabled = true;
                foreach (Study study in studies)
                {
					if (study.StudyStatusEnum.Equals(StudyStatusEnum.Nearline) || 
                        study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ReconcileRequired))
					{
						DeleteStudyButton.Enabled = false;
						MoveStudyButton.Enabled = false;
					}
                    else if (_controller.IsScheduledForDelete(study))
                    {
                        DeleteStudyButton.Enabled = false;
                        break;
                    }
                }
            }
            else
            {
                ViewStudyDetailsButton.Enabled = false;
                MoveStudyButton.Enabled = false;
                DeleteStudyButton.Enabled = false;
            	RestoreStudyButton.Enabled = false;
            }
        }

        #endregion Protected Methods
    }
}