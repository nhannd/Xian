using System;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit
{
    public partial class SearchResultGridView : System.Web.UI.UserControl
    {
        private DeletedStudyDataSource _dataSource;

        public GridView GridViewControl
        {
            get
            {
                return ListControl;
            }
        }

        public int ResultCount
        {
            get
            {
                return _dataSource.SelectCount();
            }
        }

        public ObjectDataSource DataSourceContainer
        {
            get
            {
                return DataSource;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DataSourceContainer.ObjectCreated += DataSourceContainer_ObjectCreated;
        }

        void DataSourceContainer_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            // keep a reference to the data source created, used for other purposes
            _dataSource = e.ObjectInstance as DeletedStudyDataSource;
        }

        public void GotoPage(int pageIndex)
        {
            ListControl.PageIndex = pageIndex;
        }

    }
}