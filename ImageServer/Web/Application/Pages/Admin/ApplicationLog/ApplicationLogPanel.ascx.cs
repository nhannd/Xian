using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog
{
	public partial class ApplicationLogSearchPanel : System.Web.UI.UserControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerApplicationLogSingleItem, App_GlobalResources.SR.GridPagerApplicationLogMultipleItems, ApplicationLogGridView.ApplicationLogListGrid, delegate { return ApplicationLogGridView.ResultCount; }, ImageServerConstants.GridViewPagerPosition.top);
			GridPagerBottom.InitializeGridPager(App_GlobalResources.SR.GridPagerApplicationLogSingleItem, App_GlobalResources.SR.GridPagerApplicationLogMultipleItems, ApplicationLogGridView.ApplicationLogListGrid, delegate { return ApplicationLogGridView.ResultCount; }, ImageServerConstants.GridViewPagerPosition.bottom);

			ApplicationLogGridView.DataSourceCreated += delegate(ApplicationLogDataSource source)
			                                       	{
														if (!String.IsNullOrEmpty(HostFilter.Text))
															source.Host = HostFilter.Text;
														if (!String.IsNullOrEmpty(ThreadFilter.Text))
															source.Thread = ThreadFilter.Text;
														if (!String.IsNullOrEmpty(MessageFilter.Text))
															source.Message = MessageFilter.Text;
														if (!String.IsNullOrEmpty(LogLevelListBox.SelectedValue))
															if (!LogLevelListBox.SelectedValue.Equals("ANY"))
																source.LogLevel = LogLevelListBox.SelectedValue;
														if (!String.IsNullOrEmpty(FromFilter.Text))
														{
															DateTime val;
															if (DateTime.TryParseExact(FromFilter.Text,DateTimeFormatter.DefaultTimestampFormat,CultureInfo.CurrentCulture,DateTimeStyles.None, out val))
																source.StartDate = val;
															else if (DateTime.TryParse(FromFilter.Text, out val))
																source.StartDate = val;
														}

			                                       		if (!String.IsNullOrEmpty(ToFilter.Text))
														{
															DateTime val;
															if (DateTime.TryParseExact(ToFilter.Text, DateTimeFormatter.DefaultTimestampFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out val))
																source.EndDate = val;
															else if (DateTime.TryParse(ToFilter.Text, out val))
																source.EndDate = val;
														}
														
													};
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void SearchButton_Click(object sender, ImageClickEventArgs e)
		{
			ApplicationLogGridView.ApplicationLogListGrid.ClearSelections();
			ApplicationLogGridView.ApplicationLogListGrid.PageIndex = 0;
			DataBind();
		}
	}
}