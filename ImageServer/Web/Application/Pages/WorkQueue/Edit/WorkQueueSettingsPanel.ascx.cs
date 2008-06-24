using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit
{
    /// <summary>
    /// Represents a panel containing the different settings users can specify for a work queue item.
    /// </summary>
    public partial class WorkQueueSettingsPanel : System.Web.UI.UserControl
    {
        #region Constants

        private const int DEFAULT_TIME_GAP_MINS = 15; // minutes

        #endregion Constants


        #region Private members
        private ListItemCollection _defaultTimeList;

        #endregion 

        #region Public Properties

        /// <summary>
        /// Gets/Sets the current selected work queue priority  displayed on the UI
        /// </summary>
        public WorkQueuePriorityEnum SelectedPriority
        {
            get
            {
                return WorkQueuePriorityEnum.GetEnum(PriorityDropDownList.SelectedValue);
            }
            set
            {
                PriorityDropDownList.SelectedValue = value.Lookup;
            }
        }

        /// <summary>
        /// Gets/Sets the new scheduled date/time displayed on the UI
        /// </summary>
        public DateTime? NewScheduledDateTime
        {
            get
            {
                return (DateTime?) ViewState[ClientID + "_NewScheduledDateTime"];
                //return CalendarExtender.SelectedDate;
            }
            set
            {
                ViewState[ClientID + "_NewScheduledDateTime"] = value;
                CalendarExtender.SelectedDate = value;
                NewScheduleDate.Text = value == null ? string.Empty : value.Value.ToString(CalendarExtender.Format);
                if (value != null)
                {
                    AddCustomTime(value.Value);
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Appends a specified time to the default list of schedule time.
        /// </summary>
        /// <param name="time"></param>
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

        /// <summary>
        /// Gets the default list of schedule times available for user selection
        /// </summary>
        public ListItemCollection DefaultTimeList
        {
            get
            {
                if (_defaultTimeList != null)
                    return _defaultTimeList;

                _defaultTimeList = new ListItemCollection();
                DateTime dt = DateTime.Today;
                DateTime tomorrow = DateTime.Today.AddDays(1);
                double scheduleTimeWindow = DEFAULT_TIME_GAP_MINS;
                while (dt < tomorrow)
                {
                    _defaultTimeList.Add(dt.ToString("hh:mm tt"));
                    dt = dt.AddMinutes(scheduleTimeWindow);
                }

                return _defaultTimeList;
            }
        }


        #endregion Public Methods

        #region Protected Methods

        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ListItemCollection defaultList = DefaultTimeList;
            foreach(ListItem item in defaultList)
                NewScheduleTimeDropDownList.Items.Add(item);

            CalendarExtender.Format = DateTimeFormatter.DefaultDateFormat;

            PriorityDropDownList.Items.Clear();
            IList<WorkQueuePriorityEnum> priorities = WorkQueuePriorityEnum.GetAll();
            foreach (WorkQueuePriorityEnum priority in priorities)
            {
                PriorityDropDownList.Items.Add(new ListItem(priority.Description, priority.Lookup));
            }

        }

        #endregion Protected Methods


        #region Public Methods

        public override void DataBind()
        {

            if (!Page.IsPostBack)
            {

            }
            else
            {
                if (!String.IsNullOrEmpty(NewScheduleDate.Text))
                {
                    DateTime dt = DateTime.ParseExact(NewScheduleDate.Text, CalendarExtender.Format, null);

                    DateTime time = DateTime.ParseExact(NewScheduleTimeDropDownList.Text, "hh:mm tt", null);

                    NewScheduledDateTime = dt.Add(time.TimeOfDay);

                    CalendarExtender.SelectedDate = NewScheduledDateTime;
                }
                else
                {
                    NewScheduledDateTime = null;
                }
            }

            base.DataBind();
        }

        #endregion Public Methods
    }
    
}