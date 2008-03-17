using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit
{
    public partial class WorkQueueSettingsPanel : System.Web.UI.UserControl
    {

        public WorkQueuePriorityEnum SelectedPriority
        {
            get
            {
                return WorkQueuePriorityEnum.GetEnum(PriorityDropDownList.SelectedValue);
            }
            set
            {
                if (value != null)
                    PriorityDropDownList.SelectedValue = value.Lookup;
                else
                    PriorityDropDownList.SelectedIndex = -1;
            }
        }


        public DateTime? NewScheduledDateTime
        {
            get
            {
                return (DateTime?) ViewState[ClientID + "_NewScheduledDateTime"];
                //return CalendarExtender1.SelectedDate;
            }
            set
            {
                ViewState[ClientID + "_NewScheduledDateTime"] = value;
                CalendarExtender1.SelectedDate = value;
                NewScheduleDate.Text = value == null ? "" : value.Value.ToString(CalendarExtender1.Format);
                if (value != null)
                {
                    AddCustomTime(value.Value);
                }
            }
        }

        public void AddCustomTime(DateTime time)
        {
            string timeValue = time.ToString("hh:mm tt");
            if (DefaultTimeList.FindByValue(timeValue)==null)
            {
                NewScheduleTimeDropDownList.Items.Clear();
                foreach (ListItem item in DefaultTimeList)
                    NewScheduleTimeDropDownList.Items.Add(item);

                ListItem newItem = new ListItem(timeValue, timeValue);
                NewScheduleTimeDropDownList.Items.Add(newItem);
            }

            NewScheduleTimeDropDownList.SelectedValue = timeValue;
        }

        private ListItemCollection _defaultTimeList;
        public ListItemCollection DefaultTimeList
        {
            get
            {
                if (_defaultTimeList != null)
                    return _defaultTimeList;

                _defaultTimeList = new ListItemCollection();
                DateTime dt = DateTime.Today;
                DateTime tomorrow = DateTime.Today.AddDays(1);
                double scheduleTimeWindow = 15;
                while (dt < tomorrow)
                {
                    _defaultTimeList.Add(dt.ToString("hh:mm tt"));
                    dt = dt.AddMinutes(scheduleTimeWindow);
                }

                return _defaultTimeList;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ListItemCollection defaultList = DefaultTimeList;
            foreach(ListItem item in defaultList)
                NewScheduleTimeDropDownList.Items.Add(item);

            CalendarExtender1.Format = DateTimeFormatter.DefaultDateFormat;

            PriorityDropDownList.Items.Clear();
            IList<WorkQueuePriorityEnum> priorities = WorkQueuePriorityEnum.GetAll();
            foreach (WorkQueuePriorityEnum priority in priorities)
            {
                PriorityDropDownList.Items.Add(new ListItem(priority.Description, priority.Lookup));
            }

        }

        public override void DataBind()
        {

            if (!Page.IsPostBack)
            {

            }
            else
            {
                if (!String.IsNullOrEmpty(NewScheduleDate.Text))
                {
                    DateTime dt = DateTime.ParseExact(NewScheduleDate.Text, CalendarExtender1.Format, null);

                    DateTime time = DateTime.ParseExact(NewScheduleTimeDropDownList.Text, "hh:mm tt", null);

                    NewScheduledDateTime = dt.Add(time.TimeOfDay);

                    CalendarExtender1.SelectedDate = NewScheduledDateTime;
                }
                else
                {
                    NewScheduledDateTime = null;
                }
            }

            base.DataBind();
        }
    }
    
}