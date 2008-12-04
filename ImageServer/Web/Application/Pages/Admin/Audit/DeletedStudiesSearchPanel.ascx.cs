using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit
{
    public class DeletedStudyViewDetailsClickedEventArgs:EventArgs
    {
        private DeletedStudyInfo _deletedStudyInfo;
        public DeletedStudyInfo DeletedStudyInfo
        {
            get { return _deletedStudyInfo; }
            set { _deletedStudyInfo = value; }
        }
    }

    public partial class DeletedStudiesSearchPanel : UserControl
    {
        private EventHandler<DeletedStudyViewDetailsClickedEventArgs> _viewDetailsClicked;

        public event EventHandler<DeletedStudyViewDetailsClickedEventArgs> ViewDetailsClicked
        {
            add { _viewDetailsClicked += value; }
            remove { _viewDetailsClicked -= value; }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearStudyDateButton.OnClientClick = ScriptHelper.ClearDate(StudyDate.ClientID, StudyDateCalendarExtender.ClientID);
            
            GridPagerTop.InitializeGridPager(App_GlobalResources.Labels.GridPagerQueueSingleItem, App_GlobalResources.Labels.GridPagerQueueMultipleItems, SearchResultGridView1.GridViewControl);
            GridPagerBottom.InitializeGridPager(App_GlobalResources.Labels.GridPagerQueueSingleItem, App_GlobalResources.Labels.GridPagerQueueMultipleItems, SearchResultGridView1.GridViewControl);
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

        void DataSource_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            DeletedStudyDataSource dataSource = e.ObjectInstance as DeletedStudyDataSource;
            if (dataSource!=null)
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

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            SearchResultGridView1.GotoPage(0);
            DataBind();
        }

        protected void ViewDetails(object sender, ImageClickEventArgs e)
        {
            DeletedStudyViewDetailsClickedEventArgs args = new DeletedStudyViewDetailsClickedEventArgs();
            args.DeletedStudyInfo = SearchResultGridView1.SelectedItem;
            EventsHelper.Fire(_viewDetailsClicked, this, args);
        }
    }

}