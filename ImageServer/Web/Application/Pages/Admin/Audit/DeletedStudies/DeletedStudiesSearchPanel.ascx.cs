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
            
            GridPagerTop.InitializeGridPager(App_GlobalResources.Labels.GridPagerQueueSingleItem, App_GlobalResources.Labels.GridPagerQueueMultipleItems, SearchResultGridView1.GridViewControl, ImageServerConstants.GridViewPagerPosition.top);
            GridPagerBottom.InitializeGridPager(App_GlobalResources.Labels.GridPagerQueueSingleItem, App_GlobalResources.Labels.GridPagerQueueMultipleItems, SearchResultGridView1.GridViewControl, ImageServerConstants.GridViewPagerPosition.bottom);
            GridPagerTop.GetRecordCountMethod = delegate
                                                    {
                                                        return SearchResultGridView1.ResultCount;
                                                    };
            GridPagerBottom.GetRecordCountMethod = delegate
                                                       {
                                                           return SearchResultGridView1.ResultCount;
                                                       };

            SearchResultGridView1.DataSourceContainer.ObjectCreated += DataSource_ObjectCreated;
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