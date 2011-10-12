#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    public class SearchPanelButtonClickedEventArgs:EventArgs
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
        private EventHandler<SearchPanelButtonClickedEventArgs> _deleteButtonClickedHandler;
        private EventHandler<SearchPanelButtonClickedEventArgs> _assignAuthorityGroupsButtonClickedHandler;
    	#endregion Private members

        #region Events
        public event EventHandler<SearchPanelButtonClickedEventArgs> DeleteButtonClicked
        {
            add { _deleteButtonClickedHandler += value; }
            remove { _deleteButtonClickedHandler -= value; }
        }

        public event EventHandler<SearchPanelButtonClickedEventArgs> AssignAuthorityGroupsButtonClicked
        {
            add { _assignAuthorityGroupsButtonClickedHandler += value; }
            remove { _assignAuthorityGroupsButtonClickedHandler -= value; }
        }
        #endregion

        #region Private Properties
        
        private bool DisplaySearchWarning
        {
            get
            {
                return String.IsNullOrEmpty(PatientId.Text) &&
                       String.IsNullOrEmpty(PatientName.Text) &&
                       String.IsNullOrEmpty(AccessionNumber.Text) &&
                       String.IsNullOrEmpty(ToStudyDate.Text) &&
                       String.IsNullOrEmpty(FromStudyDate.Text) &&
                       String.IsNullOrEmpty(StudyDescription.Text) &&
                       String.IsNullOrEmpty(ReferringPhysiciansName.Text) &&
                       ModalityListBox.SelectedIndex < 0 &&
                       StatusListBox.SelectedIndex < 0 &&
                       ConfigurationManager.AppSettings["DisplaySearchWarning"].ToLower().Equals("true");
            }
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
        [ClientPropertyName("AssignAuthorityGroupsButtonClientID")]
        public string AssignAuthorityGroupsButtonClientID
        {
            get { return AssignAuthorityGroupsButton.ClientID; }
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
        [ClientPropertyName("ViewImagePageUrl")]
        public string ViewImagePageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.ViewImagesPage); }
        }
        
        [ExtenderControlProperty]
        [ClientPropertyName("CanViewImages")]
        public bool CanViewImages
        {
            get { return Thread.CurrentPrincipal.IsInRole(ImageServerConstants.WebViewerAuthorityToken); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewImageButtonClientID")]
        public string ViewImageButtonClientID
        {
            get { return ViewImagesButton.ClientID; }
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
            foreach(StudyStatusEnum s in  StudyStatusEnum.GetAll())
            {
                StatusListBox.Items.Add(new ListItem(){ Text = ServerEnumDescription.GetLocalizedDescription(s), Value = s.Lookup});
            }

            ClearToStudyDateButton.Attributes["onclick"] = ScriptHelper.ClearDate(ToStudyDate.ClientID, ToStudyDateCalendarExtender.ClientID);
            ClearFromStudyDateButton.Attributes["onclick"] = ScriptHelper.ClearDate(FromStudyDate.ClientID, FromStudyDateCalendarExtender.ClientID);
            ToStudyDate.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromStudyDate.ClientID, ToStudyDate.ClientID, ToStudyDate.ClientID, ToStudyDateCalendarExtender.ClientID, "To Date must be greater than From Date");
            FromStudyDate.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromStudyDate.ClientID, ToStudyDate.ClientID, FromStudyDate.ClientID, FromStudyDateCalendarExtender.ClientID, "From Date must be less than To Date");
            
            GridPagerTop.InitializeGridPager(SR.GridPagerStudySingleItem, SR.GridPagerStudyMultipleItems, StudyListGridView.TheGrid, delegate { return StudyListGridView.ResultCount; }, ImageServerConstants.GridViewPagerPosition.Top);
            StudyListGridView.Pager = GridPagerTop;

            ConfirmStudySearchMessageBox.Confirmed += delegate(object data) {
                                                                                StudyListGridView.DataBindOnPreRender =
                                                                                    true;  StudyListGridView.Refresh(); };
            ConfirmStudySearchMessageBox.Cancel += delegate()
            {
                StudyListGridView.DataBindOnPreRender =
                    false; 
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
                                                source.PatientId = SearchHelper.TrailingWildCard(PatientId.Text);
                                            if (!String.IsNullOrEmpty(PatientName.Text))
                                                source.PatientName = SearchHelper.NameWildCard(PatientName.Text);
                                            if (!String.IsNullOrEmpty(AccessionNumber.Text))
                                                source.AccessionNumber = SearchHelper.TrailingWildCard(AccessionNumber.Text);
                                            if (!String.IsNullOrEmpty(ToStudyDate.Text))
                                                source.ToStudyDate = ToStudyDate.Text;
                                            if (!String.IsNullOrEmpty(FromStudyDate.Text))
                                                source.FromStudyDate = FromStudyDate.Text;
                                            if (!String.IsNullOrEmpty(StudyDescription.Text))
                                                source.StudyDescription = SearchHelper.LeadingAndTrailingWildCard(StudyDescription.Text);
                                            if (!String.IsNullOrEmpty(ReferringPhysiciansName.Text))
                                                source.ReferringPhysiciansName = SearchHelper.NameWildCard(ReferringPhysiciansName.Text);

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
            ViewImagesButton.Roles = ImageServerConstants.WebViewerAuthorityToken;
            ViewStudyDetailsButton.Roles = AuthorityTokens.Study.View;
            MoveStudyButton.Roles = AuthorityTokens.Study.Move;
            DeleteStudyButton.Roles = AuthorityTokens.Study.Delete;
            RestoreStudyButton.Roles = AuthorityTokens.Study.Restore;
            AssignAuthorityGroupsButton.Roles = ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup;
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
            ReferringPhysiciansName.Text = string.Empty;
        }

        public void Refresh()
        {
            if (!StudyListGridView.IsDataSourceSet()) StudyListGridView.SetDataSource();
            StudyListGridView.RefreshCurrentPage();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SetupChildControls();
        }
        
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {   
            if(DisplaySearchWarning) {
                StudyListGridView.DataBindOnPreRender = false;
                ConfirmStudySearchMessageBox.Message = SR.NoFiltersSearchWarning;
                   ConfirmStudySearchMessageBox.MessageStyle = "font-weight: bold; color: #205F87;";
                ConfirmStudySearchMessageBox.Show();
            } else
            {
                StudyListGridView.Refresh();    
            }
        	StringBuilder sb = new StringBuilder();
			if(!String.IsNullOrEmpty(PatientId.Text))
				sb.AppendFormat("PatientId={0};", PatientId.Text);
            if(!String.IsNullOrEmpty(PatientName.Text))
				sb.AppendFormat("PatientsName={0};", PatientName.Text);
            if(!String.IsNullOrEmpty(AccessionNumber.Text))
				sb.AppendFormat("AccessionNumber={0};", AccessionNumber.Text);
            if(!String.IsNullOrEmpty(ToStudyDate.Text)||!String.IsNullOrEmpty(FromStudyDate.Text))
				sb.AppendFormat("StudyDate={0}-{1};", FromStudyDate.Text, ToStudyDate.Text);
            if(!String.IsNullOrEmpty(StudyDescription.Text))
				sb.AppendFormat("StudyDescription={0};", StudyDescription.Text);
			if (ModalityListBox.SelectedIndex < 0)
			{
				bool first = true;
				foreach (ListItem item in ModalityListBox.Items)
				{
					if (!item.Selected) continue;

					if (first)
					{
						sb.AppendFormat("ModalitiesInStudy={0}", item.Value);
						first = false;
					}
					else
					{
						sb.AppendFormat(",{0}", item.Value);
					}
				}
				if (!first)
					sb.Append(';');
			}

        	QueryAuditHelper helper = new QueryAuditHelper(ServerPlatform.AuditSource, EventIdentificationContentsEventOutcomeIndicator.Success,
				new AuditPersonActiveParticipant(SessionManager.Current.Credentials.UserName,
											 null,
											 SessionManager.Current.Credentials.DisplayName),
				ServerPartition.AeTitle,ServerPlatform.HostId,sb.ToString());
			ServerPlatform.LogAuditMessage(helper);
        }
        
		protected void RestoreStudyButton_Click(object sender, ImageClickEventArgs e)
		{
			IList<StudySummary> studies = StudyListGridView.SelectedStudies;

			if (studies != null && studies.Count > 0)
			{
			    string message = studies.Count > 1 ? string.Format(SR.MultipleStudyRestore):
				                                    string.Format(SR.SingleStudyRestore);

			    RestoreMessageBox.Message = DialogHelper.createConfirmationMessage(message);
                RestoreMessageBox.Message += DialogHelper.createStudyTable(studies);
				
			    RestoreMessageBox.Title = Titles.RestoreStudyConfirmation;
                RestoreMessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
				IList<Study> studyList = new List<Study>();
				foreach (StudySummary summary in studies)
					studyList.Add(summary.TheStudy);
				RestoreMessageBox.Data = studyList;
				RestoreMessageBox.Show();
			}
		}

        protected void DeleteStudyButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyListGridView.RefreshCurrentPage();
            SearchPanelButtonClickedEventArgs args = new SearchPanelButtonClickedEventArgs
                                                         {
                                                             SelectedStudies = StudyListGridView.SelectedStudies
                                                         };
            EventsHelper.Fire(_deleteButtonClickedHandler, this, args);
        }

        protected void AssignAuthorityGroupsButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyListGridView.RefreshCurrentPage();
            SearchPanelButtonClickedEventArgs args = new SearchPanelButtonClickedEventArgs
                                                         {
                                                             SelectedStudies = StudyListGridView.SelectedStudies
                                                         };
            EventsHelper.Fire(_assignAuthorityGroupsButtonClickedHandler, this, args);
        }

        protected void OpenStudyButton_Click(object sender, ImageClickEventArgs e)
        {
            foreach(StudySummary study in StudyListGridView.SelectedStudies)
            {
                String url = String.Format("{0}?study={1}", ImageServerConstants.PageURLs.StudyDetailsPage, study.StudyInstanceUid);

                string script = String.Format("window.open(\"{0}\", \"{1}\");", Page.ResolveClientUrl(url), "_blank");

                ScriptManager.RegisterStartupScript(Page, typeof(Page), "Redirect", script, true);
            }
        }

        #endregion Protected Methods

    }
}