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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
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
                return (DateTime?) ViewState[ "NewScheduledDateTime"];
            }
            set
            {
                ViewState[ "NewScheduledDateTime"] = value;
                CalendarExtender.SelectedDate = value;
                NewScheduleDate.Text = value == null ? string.Empty : value.Value.ToString(CalendarExtender.Format);
                if (value != null && ScheduleNowCheckBox.Checked == false)
                {
                    AddCustomTime(value.Value);
                }
            }
        }

        public bool ScheduleNow
        {
            get { return (Boolean)ViewState[ "_ScheduleNow"]; }
            set { ViewState[ "_ScheduleNow"] = value;
                ScheduleNowCheckBox.Checked = value;
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

            NewScheduleTime.Text = timeValue;
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

            CalendarExtender.Format = DateTimeFormatter.DefaultDateFormat;

            PriorityDropDownList.Items.Clear();
            IList<WorkQueuePriorityEnum> priorities = WorkQueuePriorityEnum.GetAll();
            foreach (WorkQueuePriorityEnum priority in priorities)
            {
                PriorityDropDownList.Items.Add(new ListItem(priority.Description, priority.Lookup));
            }

            ScheduleNowCheckBox.Checked = false;
            NewScheduleDate.Enabled = true;
            NewScheduleTime.Enabled = true;

//            WorkQueueSettingsUpdatePanel.Update();

        }

        protected void ScheduleNow_CheckChanged(object sender, EventArgs arg)
        {
            ScheduleNow = ScheduleNowCheckBox.Checked;

            if(ScheduleNow)
            {
                NewScheduleDate.Enabled = false;
                NewScheduleTime.Enabled = false;
            } else
            {
                NewScheduleDate.Enabled = true;
                NewScheduleTime.Enabled = true;
            }

//           WorkQueueSettingsUpdatePanel.Update();

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

                    DateTime time = Platform.Time; 
                    
                    if(NewScheduleTime.Text.Contains("_") == false)
                    {
                        try
                        {
                            time = DateTime.ParseExact(NewScheduleTime.Text, "hh:mm tt", null);
                        }
						catch(Exception)
                        {
                            //Ignore this exception since the time is not fully typed in or in an incorrect format,
                            //that will be validated when the user presses apply.
                        }
                    }

                    NewScheduledDateTime = dt.Add(time.TimeOfDay);

                    CalendarExtender.SelectedDate = NewScheduledDateTime;
                }
                else
                {
                    NewScheduledDateTime = null;
                }                
            }

            NewScheduleDate.Enabled = true;
            NewScheduleTime.Enabled = true;

            base.DataBind();
        }

        public void UpdateScheduleDateTime()
        {
            DateTime newDate = DateTime.Parse(NewScheduleDate.Text + " " + NewScheduleTime.Text);
            NewScheduledDateTime = newDate;
        }

        #endregion Public Methods
    }
    
}