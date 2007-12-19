#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue
{
    /// <summary>
    /// WorkQueue filter panel
    /// </summary>
    public partial class SearchFilterPanel : UserControl
    {
        /// <summary>
        /// Used to store the filter settings.
        /// </summary>
        public class SearchFilterSettings
        {
            #region private members

            private string _patientId;
            private string _accessionNumber;
            private string _studyDescription;
            private DateTime _scheduledTime;
            private WorkQueueTypeEnum _type;
            private WorkQueueStatusEnum _status;

            #endregion

            #region public properties

            /// <summary>
            ///  The Patient Id filter
            /// </summary>
            public string PatientId
            {
                get { return _patientId; }
                set { _patientId = value; }
            }

            /// <summary>
            ///  The Patient Id filter
            /// </summary>
            public string AccessionNumber
            {
                get { return _accessionNumber; }
                set { _accessionNumber = value; }
            }

            public string StudyDescription
            {
                get { return _studyDescription; }
                set { _studyDescription = value; }
            }

            public DateTime ScheduledTime
            {
                get { return _scheduledTime; }
                set { _scheduledTime = value; }
            }

            public WorkQueueTypeEnum Type
            {
                get { return _type; }
                set { _type = value; }
            }

            public WorkQueueStatusEnum Status
            {
                get { return _status; }
                set { _status = value; }
            }

            #endregion
        }

        #region Static members

        private static readonly IList<WorkQueueTypeEnum> WorkQueueTypes;
        private static readonly IList<WorkQueueStatusEnum> WorkQueueStatuses;

        #endregion Static members

        #region Class initializer

        static SearchFilterPanel()
        {
            // cache these lists
            WorkQueueTypes = WorkQueueTypeEnum.GetAll();
            WorkQueueStatuses = WorkQueueStatusEnum.GetAll();
        }

        #endregion Class initializer

        #region public properties

        /// <summary>
        /// Retrieves the current filter settings.
        /// </summary>
        public SearchFilterSettings Filters
        {
            get
            {
                SearchFilterSettings settings = new SearchFilterSettings();
                settings.PatientId = PatientId.Text;
                settings.AccessionNumber = AccessionNumber.Text;
                settings.StudyDescription = StudyDescription.Text;
                settings.ScheduledTime = (ScheduleDate.Text == "")
                                             ? DateTime.MinValue
                                             : DateTime.Parse(ScheduleDate.Text);

                settings.Type = (TypeDropDownList.SelectedValue == "")
                                    ? null
                                    : WorkQueueTypeEnum.GetEnum(TypeDropDownList.SelectedValue);

                settings.Status = (StatusDropDownList.SelectedValue == "")
                                      ? null
                                      : WorkQueueStatusEnum.GetEnum(StatusDropDownList.SelectedValue);

                return settings;
            }
        }

        #endregion // public properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // first time load
                ScheduleDate.Text = DateTime.Today.ToString("MM/dd/yyyy");
            }

            // re-populate the drop down lists and restore their states
            int prevSelectedIndex = TypeDropDownList.SelectedIndex;
            TypeDropDownList.Items.Clear();
            TypeDropDownList.Items.Add(new ListItem("-- Any --", ""));
            foreach (WorkQueueTypeEnum t in WorkQueueTypes)
                TypeDropDownList.Items.Add(new ListItem(t.Description, t.Lookup));
            TypeDropDownList.SelectedIndex = prevSelectedIndex;

            prevSelectedIndex = StatusDropDownList.SelectedIndex;
            StatusDropDownList.Items.Clear();
            IList<WorkQueueStatusEnum> statuses = WorkQueueStatusEnum.GetAll();
            StatusDropDownList.Items.Add(new ListItem("-- Any --", ""));
            foreach (WorkQueueStatusEnum s in WorkQueueStatuses)
                StatusDropDownList.Items.Add(new ListItem(s.Description, s.Lookup));
            StatusDropDownList.SelectedIndex = prevSelectedIndex;
        }

        /// <summary>
        /// Handle user clicking the "Apply Filter" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            if (ApplyFiltersClicked != null)
                ApplyFiltersClicked(Filters);
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = "";
            AccessionNumber.Text = "";
        }

        #endregion Public Methods

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="ApplyFiltersClicked"/>
        /// </summary>
        /// <param name="filters">The current settings.</param>
        /// <remarks>
        /// </remarks>
        public delegate void OnApplyFilterSettingsClickedEventHandler(SearchFilterSettings filters);

        /// <summary>
        /// Occurs when the filter settings users click on "Apply" on the filter panel.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event OnApplyFilterSettingsClickedEventHandler ApplyFiltersClicked;

        #endregion // Events
    }
}