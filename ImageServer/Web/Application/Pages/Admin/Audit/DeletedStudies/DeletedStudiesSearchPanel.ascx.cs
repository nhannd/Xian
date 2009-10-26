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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Data.Model;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    /// <summary>
    /// Represents an event fired when the View Details button is clicked
    /// </summary>
    public class DeletedStudyViewDetailsClickedEventArgs:EventArgs
    {
        private DeletedStudyInfo _deletedStudyInfo;
        public DeletedStudyInfo DeletedStudyInfo
        {
            get { return _deletedStudyInfo; }
            set { _deletedStudyInfo = value; }
        }
    }

    /// <summary>
    /// Represents an event fired when the Delete button is clicked
    /// </summary>
    public class DeletedStudyDeleteClickedEventArgs : EventArgs
    {
        private DeletedStudyInfo _selectedStudyInfo;
        public DeletedStudyInfo SelectedItem
        {
            get { return _selectedStudyInfo; }
            set { _selectedStudyInfo = value; }
        }
    }

    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.js")]
    public partial class DeletedStudiesSearchPanel : AJAXScriptControl
    {
        #region Private Fields
        private EventHandler<DeletedStudyViewDetailsClickedEventArgs> _viewDetailsClicked;
        private EventHandler<DeletedStudyDeleteClickedEventArgs> _deleteClicked;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when user clicks on the View Details button
        /// </summary>
        public event EventHandler<DeletedStudyViewDetailsClickedEventArgs> ViewDetailsClicked
        {
            add { _viewDetailsClicked += value; }
            remove { _viewDetailsClicked -= value; }
        }

        /// <summary>
        /// Occurs when user clicks on the Delete button
        /// </summary>
        public event EventHandler<DeletedStudyDeleteClickedEventArgs> DeleteClicked
        {
            add { _deleteClicked += value; }
            remove { _deleteClicked -= value; }
        }
        #endregion

        #region Ajax Properties
        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewDetailsButtonClientID")]
        public string ViewDetailsButtonClientID
        {
            get { return ViewStudyDetailsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ListClientID")]
        public string ListClientID
        {
            get { return SearchResultGridView1.GridViewControl.ClientID; }
        }

        #endregion 
        
        #region Private Methods
        void DataSource_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            DeletedStudyDataSource dataSource = e.ObjectInstance as DeletedStudyDataSource;
            if (dataSource != null)
            {
                dataSource.AccessionNumber = AccessionNumber.Text;
                dataSource.PatientsName = PatientName.Text;
                dataSource.PatientId = PatientId.Text;
                dataSource.StudyDescription = StudyDescription.Text;
                dataSource.DeletedBy = DeletedBy.Text;
                if (!String.IsNullOrEmpty(StudyDate.Text))
                {
                    DateTime value;
                    if (DateTime.TryParseExact(StudyDate.Text, StudyDateCalendarExtender.Format,
                                               CultureInfo.InvariantCulture, DateTimeStyles.None,
                                               out value))
                    {
                        dataSource.StudyDate = value;
                    }
                }
            }

        }
        #endregion

        #region Overridden Protected Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearStudyDateButton.OnClientClick = ScriptHelper.ClearDate(StudyDate.ClientID, StudyDateCalendarExtender.ClientID);
            
            GridPagerTop.InitializeGridPager(App_GlobalResources.Labels.GridPagerQueueSingleItem, App_GlobalResources.Labels.GridPagerQueueMultipleItems, SearchResultGridView1.GridViewControl, delegate { return SearchResultGridView1.ResultCount; }, ImageServerConstants.GridViewPagerPosition.Top);
            SearchResultGridView1.Pager = GridPagerTop;
            GridPagerTop.Reset();

            SearchResultGridView1.DataSourceContainer.ObjectCreated += DataSource_ObjectCreated;

            DeleteButton.Roles =
                AuthorityTokens.Admin.StudyDeleteHistory.Delete;
            ViewStudyDetailsButton.Roles =
                AuthorityTokens.Admin.StudyDeleteHistory.View;
        }
        #endregion

        #region Protected Methods
        
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            Refresh();
        }

        protected void ViewDetailsButtonClicked(object sender, ImageClickEventArgs e)
        {
            DeletedStudyViewDetailsClickedEventArgs args = new DeletedStudyViewDetailsClickedEventArgs();
            args.DeletedStudyInfo = SearchResultGridView1.SelectedItem;
            EventsHelper.Fire(_viewDetailsClicked, this, args);
        }

        protected void DeleteButtonClicked(object sender, ImageClickEventArgs e)
        {
            DeletedStudyDeleteClickedEventArgs args = new DeletedStudyDeleteClickedEventArgs();
            args.SelectedItem = SearchResultGridView1.SelectedItem;
            EventsHelper.Fire(_deleteClicked, this, args);
        }
        #endregion
        
        #region Public Methods

        public void Refresh()
        {
            SearchResultGridView1.GotoPage(0);
            DataBind();
            SearchUpdatePanel.Update();
        }

        #endregion
    }
}