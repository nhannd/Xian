using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Users
{
    public partial class UserPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerUserSingleItemFound,
                                             App_GlobalResources.SR.GridPagerUserMultipleItemsFound,
                                             UserGridPanel.UserGrid, 
                                             delegate
                                                 {
                                                     return UserGridPanel.ResultCount;
                                                 },
                                             ImageServerConstants.GridViewPagerPosition.top);
            GridPagerBottom.InitializeGridPager(App_GlobalResources.SR.GridPagerUserSingleItemFound,
                                                App_GlobalResources.SR.GridPagerUserMultipleItemsFound,
                                                UserGridPanel.UserGrid,
                                                delegate { return UserGridPanel.ResultCount; },
                                                ImageServerConstants.GridViewPagerPosition.bottom);

            UserGridPanel.DataSourceCreated += delegate(UserDataSource source)
                            {
                                //TODO: Search stuff here
                            };
        }
    }
}