using System;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class SearchResultGridView : System.Web.UI.UserControl
    {
        #region Private Fields
        private DeletedStudyDataSource _dataSource;
        #endregion

        #region Public Properties

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

        public DeletedStudyInfo SelectedItem
        {
            get
            {
                return _dataSource.Find(ListControl.SelectedValue);
            }
        }

        public ObjectDataSource DataSourceContainer
        {
            get
            {
                return DataSource;
            }
        }

        #endregion

        #region Overriden Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DataSourceContainer.ObjectCreated += DataSourceContainer_ObjectCreated;
        }

        #endregion

        #region Private Methods
        void DataSourceContainer_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            // keep a reference to the data source created, used for other purposes
            _dataSource = e.ObjectInstance as DeletedStudyDataSource;
        }

        #endregion

        #region Public Methods
        public void GotoPage(int pageIndex)
        {
            ListControl.PageIndex = pageIndex;
        }

        #endregion
    }
}