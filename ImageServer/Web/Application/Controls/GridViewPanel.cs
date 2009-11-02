using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public class GridViewPanel : UserControl
    {
        private GridPager _gridPager;
        private GridView _theGrid;
        private bool _dataBindOnPreRender = true;
        
        public GridPager Pager
        {
            set { _gridPager = value; }
            get { return _gridPager;  }
        }

        public bool DataBindOnPreRender
        {
            set { _dataBindOnPreRender = value;  }
            get { return _dataBindOnPreRender;  }

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

        public void RefreshWithoutPagerUpdate()
        {
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

        protected override void OnPreRender(EventArgs e)
        {
        if(!_theGrid.IsDataBound && _dataBindOnPreRender) _theGrid.DataBind();
        }
    }
}
