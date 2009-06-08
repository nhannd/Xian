using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public class GridViewPanel : UserControl
    {
        private GridPager _gridPager;
        private GridView _theGrid;
        
        public GridPager Pager
        {
            set { _gridPager = value; }
            get { return _gridPager;  }
        }

        public GridView TheGrid
        {
            set { _theGrid = value; }
            get { return _theGrid; }
        }

        public void Refresh()
        {
            if(_gridPager != null) _gridPager.Reset();
            _theGrid.ClearSelections();
            _theGrid.PageIndex = 0;
            _theGrid.DataBind();
        }

        public void RefreshAndKeepSelections()
        {
            if (_gridPager != null) _gridPager.Reset();
            _theGrid.PageIndex = 0;
            _theGrid.DataBind();
        }

        public void RefreshCurrentPage()
        {
            if(_gridPager != null) _gridPager.Reset();
            _theGrid.DataBind();
        }

        protected void DisposeDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }


    }
}
