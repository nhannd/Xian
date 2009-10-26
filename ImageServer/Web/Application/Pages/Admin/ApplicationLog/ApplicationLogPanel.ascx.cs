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
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog
{
	public partial class ApplicationLogSearchPanel : System.Web.UI.UserControl
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !Page.IsAsync)
            {
                string startTime = Request["From"];
                string endTime = Request["To"];
                string hostname = Request["HostName"];

                if (startTime != null && endTime != null)
                {
                    string[] start = startTime.Split(' ');
                    string[] end = endTime.Split(' ');

                    FromDateFilter.Text = start[0];
                    ToDateFilter.Text = end[0];
                    FromDateCalendarExtender.SelectedDate = DateTime.Parse(start[0]);
                    ToDateCalendarExtender.SelectedDate = DateTime.Parse(end[0]);
                    FromTimeFilter.Text = start[1] + ".000";
                    ToTimeFilter.Text = end[1] + ".000";
                    HostFilter.Text = hostname;
                    ApplicationLogGridView.SetDataSource();
                    ApplicationLogGridView.Refresh();
                }
            }
        }

	    protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            ClearToDateFilterButton.Attributes["onclick"] = ScriptHelper.ClearDate(ToDateFilter.ClientID, ToDateCalendarExtender.ClientID);
            ClearFromDateFilterButton.Attributes["onclick"] = ScriptHelper.ClearDate(FromDateFilter.ClientID, FromDateCalendarExtender.ClientID);
            ToDateFilter.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromDateFilter.ClientID, ToDateFilter.ClientID, ToDateFilter.ClientID, ToDateCalendarExtender.ClientID, "To Date must be greater than From Date") + " " + ScriptHelper.PopulateDefaultToTime(ToTimeFilter.ClientID) + " return false;";
            FromDateFilter.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromDateFilter.ClientID, ToDateFilter.ClientID, FromDateFilter.ClientID, FromDateCalendarExtender.ClientID, "From Date must be less than To Date") + " " + ScriptHelper.PopulateDefaultFromTime(FromTimeFilter.ClientID) + " return false;";

			GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerApplicationLogSingleItem, App_GlobalResources.SR.GridPagerApplicationLogMultipleItems, ApplicationLogGridView.ApplicationLogListGrid, delegate { return ApplicationLogGridView.ResultCount; }, ImageServerConstants.GridViewPagerPosition.Top);
		    ApplicationLogGridView.Pager = GridPagerTop;

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
														if (!String.IsNullOrEmpty(FromDateFilter.Text) || !String.IsNullOrEmpty(FromTimeFilter.Text))
														{
															DateTime val;

															if (DateTime.TryParseExact(FromDateFilter.Text + " " + FromTimeFilter.Text,DateTimeFormatter.DefaultTimestampFormat,CultureInfo.CurrentCulture,DateTimeStyles.None, out val))
																source.StartDate = val;
															else if (DateTime.TryParse(FromDateFilter.Text + " " + FromTimeFilter.Text, out val))
																source.StartDate = val;
														}

			                                       		if (!String.IsNullOrEmpty(ToDateFilter.Text) || !String.IsNullOrEmpty(ToTimeFilter.Text))
														{
															DateTime val;
															if (DateTime.TryParseExact(ToDateFilter.Text + " " + ToTimeFilter.Text, DateTimeFormatter.DefaultTimestampFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out val))
																source.EndDate = val;
															else if (DateTime.TryParse(ToDateFilter.Text + " " + ToTimeFilter.Text, out val))
																source.EndDate = val;
														}
														
													};

		}

		protected void SearchButton_Click(object sender, ImageClickEventArgs e)
		{
            ApplicationLogGridView.Refresh();
		}
	}
}