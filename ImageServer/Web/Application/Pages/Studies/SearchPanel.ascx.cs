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
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    public class SearchPanelDeleteButtonClickedEventArgs:EventArgs
    {
        private IEnumerable<StudySummary> _selectedStudies;
        public IEnumerable<StudySummary> SelectedStudies
        {
            set { _selectedStudies = value; }
            get { return _selectedStudies; }
        }
    }
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members
        private ServerPartition _serverPartition;
        private StudyController _controller = new StudyController();
        private EventHandler<SearchPanelDeleteButtonClickedEventArgs> _deleteButtonClickedHandler;
    	#endregion Private members

        #region Events
        public event EventHandler<SearchPanelDeleteButtonClickedEventArgs> DeleteButtonClicked
        {
            add { _deleteButtonClickedHandler += value; }
            remove { _deleteButtonClickedHandler -= value; }
        }
        #endregion

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteStudyButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenButtonClientID")]
        public string OpenButtonClientID
        {
            get { return ViewStudyDetailsButton.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("RestoreButtonClientID")]
		public string RestoreButtonClientID
		{
			get { return RestoreStudyButton.ClientID; }
		}

        [ExtenderControlProperty]
        [ClientPropertyName("SendButtonClientID")]
        public string SendButtonClientID
        {
            get { return MoveStudyButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("StudyListClientID")]
        public string StudyListClientID
        {
            get { return StudyListGridView.TheGrid.ClientID; }
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

        [ExtenderControlProperty]
        [ClientPropertyName("CanViewStudyDetails")]
        public bool CanViewStudyDetails
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Study.View); }
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
            ClearToStudyDateButton.Attributes["onclick"] = ScriptHelper.ClearDate(ToStudyDate.ClientID, ToStudyDateCalendarExtender.ClientID);
            ClearFromStudyDateButton.Attributes["onclick"] = ScriptHelper.ClearDate(FromStudyDate.ClientID, FromStudyDateCalendarExtender.ClientID);
            ToStudyDate.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromStudyDate.ClientID, ToStudyDate.ClientID, ToStudyDate.ClientID, ToStudyDateCalendarExtender.ClientID, "To Date must be greater than From Date");
            FromStudyDate.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromStudyDate.ClientID, ToStudyDate.ClientID, FromStudyDate.ClientID, FromStudyDateCalendarExtender.ClientID, "From Date must be less than To Date");
            
            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerStudySingleItem, App_GlobalResources.SR.GridPagerStudyMultipleItems, StudyListGridView.TheGrid, delegate { return StudyListGridView.ResultCount; }, ImageServerConstants.GridViewPagerPosition.Top);
            StudyListGridView.Pager = GridPagerTop;

            ConfirmStudySearchMessageBox.Confirmed += delegate(object data) { StudyListGridView.Refresh(); };

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
								else if (data is IList<StudySummary>)
								{
									IList<StudySummary> studies = data as IList<StudySummary>;
									foreach (StudySummary study in studies)
									{
										_controller.RestoreStudy(study.TheStudy);
									}
								}
                                else if (data is Study)
                                {
                                    Study study = data as Study;
                                    _controller.RestoreStudy(study);
                                }

                                DataBind();
                                SearchUpdatePanel.Update(); // force refresh
                            };

            StudyListGridView.DataSourceCreated += delegate(StudyDataSource source)
                                        {
                                            source.Partition = ServerPartition;
                                            source.DateFormats = ToStudyDateCalendarExtender.Format;

                                            if (!String.IsNullOrEmpty(PatientId.Text))
                                                source.PatientId = PatientId.Text;
                                            if (!String.IsNullOrEmpty(PatientName.Text))
                                                source.PatientName = PatientName.Text;
                                            if (!String.IsNullOrEmpty(AccessionNumber.Text))
                                                source.AccessionNumber = AccessionNumber.Text;
                                            if (!String.IsNullOrEmpty(ToStudyDate.Text))
                                                source.ToStudyDate = ToStudyDate.Text;
                                            if (!String.IsNullOrEmpty(FromStudyDate.Text))
                                                source.FromStudyDate = FromStudyDate.Text;
                                            if (!String.IsNullOrEmpty(StudyDescription.Text))
                                                source.StudyDescription = StudyDescription.Text;

                                            if (ModalityListBox.SelectedIndex > -1)
                                            {
                                                List<string> modalities = new List<string>();
                                                foreach (ListItem item in ModalityListBox.Items)
                                                {
                                                    if (item.Selected)
                                                    {
                                                        modalities.Add(item.Value);
                                                    }
                                                }
                                                source.Modalities = modalities.ToArray();
                                            }

                                            if (StatusListBox.SelectedIndex > -1)
                                            {
                                                List<string> statuses = new List<string>();
                                                foreach (ListItem status in StatusListBox.Items)
                                                {
                                                    if (status.Selected)
                                                    {
                                                        statuses.Add(status.Value);
                                                    }
                                                }
                                                source.Statuses = statuses.ToArray();
                                            }
                                        };

            //Set Roles
            ViewStudyDetailsButton.Roles = AuthorityTokens.Study.View;
            MoveStudyButton.Roles = AuthorityTokens.Study.Move;
            DeleteStudyButton.Roles = AuthorityTokens.Study.Delete;
            RestoreStudyButton.Roles = AuthorityTokens.Study.Restore;
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
            ToStudyDate.Text = string.Empty;
            FromStudyDate.Text = string.Empty;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetupChildControls();           
        }

        public void Refresh()
        {
            if(!StudyListGridView.IsDataSourceSet()) StudyListGridView.SetDataSource();
            StudyListGridView.RefreshCurrentPage();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {   
            if(String.IsNullOrEmpty(PatientId.Text) && 
               String.IsNullOrEmpty(PatientName.Text) &&
               String.IsNullOrEmpty(AccessionNumber.Text) &&
               String.IsNullOrEmpty(ToStudyDate.Text) &&
               String.IsNullOrEmpty(FromStudyDate.Text) &&
               String.IsNullOrEmpty(StudyDescription.Text) &&
               ModalityListBox.SelectedIndex < 0 &&
               StatusListBox.SelectedIndex < 0) {
                ConfirmStudySearchMessageBox.Message = "With no filters specified, this search may return a large number of studies.<br/>Are you sure you want to continue?";
                   ConfirmStudySearchMessageBox.MessageStyle = "font-weight: bold; color: #205F87;";
                ConfirmStudySearchMessageBox.Show();
            } else
            {
                StudyListGridView.Refresh();    
            }            
        }
        
		protected void RestoreStudyButton_Click(object sender, ImageClickEventArgs e)
		{
			IList<StudySummary> studies = StudyListGridView.SelectedStudies;

			if (studies != null && studies.Count > 0)
			{
			    string message = studies.Count > 1 ? string.Format(App_GlobalResources.SR.MultipleStudyRestore):
				                                    string.Format(App_GlobalResources.SR.SingleStudyRestore);

			    RestoreMessageBox.Message = DialogHelper.createConfirmationMessage(message);
                RestoreMessageBox.Message += DialogHelper.createStudyTable(studies);
				
			    RestoreMessageBox.Title = App_GlobalResources.Titles.RestoreStudyConfirmation;
                RestoreMessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
				IList<Study> studyList = new List<Study>();
				foreach (StudySummary summary in studies)
					studyList.Add(summary.TheStudy);
				RestoreMessageBox.Data = studyList;
				RestoreMessageBox.Show();
			}
		}

        #endregion Protected Methods

        protected void DeleteStudyButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyListGridView.RefreshCurrentPage();
            SearchPanelDeleteButtonClickedEventArgs args = new SearchPanelDeleteButtonClickedEventArgs();
            args.SelectedStudies = StudyListGridView.SelectedStudies;
            EventsHelper.Fire(_deleteButtonClickedHandler, this, args);
        }
    }
}