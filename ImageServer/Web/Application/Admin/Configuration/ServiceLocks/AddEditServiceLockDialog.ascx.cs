#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;


namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServiceLocks
{
    //
    // Dialog for adding a new service or editting an existing service.
    //
    public partial class AddEditServiceLockDialog : UserControl
    {
        #region Constants

        private const int DEFAULT_TIME_GAP_MINS = 15; // minutes

        #endregion Constants

        private List<DateTime> _defaultTimeList;

        #region Events

        public delegate void ServiceLockUpdatedListener(ServiceLock serviceLock);

        public event ServiceLockUpdatedListener ServiceLockUpdated;

        #endregion Events

        #region public members


        /// <summary>
        /// Gets the default list of schedule times available for user selection
        /// </summary>
        public List<DateTime> DefaultTimeList
        {
            get
            {
                if (_defaultTimeList != null)
                    return _defaultTimeList;

                _defaultTimeList = new List<DateTime>();
                DateTime dt = DateTime.Today;
                DateTime tomorrow = DateTime.Today.AddDays(1);
                double scheduleTimeWindow = DEFAULT_TIME_GAP_MINS;
                while (dt < tomorrow)
                {
                    _defaultTimeList.Add(dt);
                    dt = dt.AddMinutes(scheduleTimeWindow);
                }

                return _defaultTimeList;
            }
        }

        /// <summary>
        /// Sets or gets the value which indicates whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            get
            {
                if (ViewState[ClientID + "_EditMode"] == null)
                    return false;
                return (bool) ViewState[ClientID + "_EditMode"];
            }
            set
            {
                ViewState[ClientID + "_EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing service.
        /// </summary>
        public ServiceLock ServiceLock
        {
            set
            {
                // put into viewstate to retrieve later
                ViewState[ClientID + "_ServiceLock"] = value;
            }
            get
            {
                return ViewState[ClientID + "_ServiceLock"] as ServiceLock;
            }
        }

        #endregion // public members

        #region Events


        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (DateTime dt in DefaultTimeList)
            {
                ListItem item = new ListItem(dt.ToString("hh:mm tt"), dt.ToString("hh:mm tt"));
                ScheduleTimeDropDownList.Items.Add(item);
            }

        }

        public override void DataBind()
        {
            ServiceLock service = ServiceLock;

            if (service!=null)
            {
                // only rebind the data if the dialog has closed 
                if (ModalDialog1.State == ClearCanvas.ImageServer.Web.Application.Common.ModalDialog.ShowState.Hide)
                {
                    Type.Text = service.ServiceLockTypeEnum.Description;
                    Description.Text = service.ServiceLockTypeEnum.LongDescription;
                    Enabled.Checked = service.Enabled;

                    if (service.FilesystemKey != null)
                    {
                        FileSystemDataAdapter adaptor = new FileSystemDataAdapter();
                        Model.Filesystem fs = adaptor.Get(service.FilesystemKey);
                        FileSystem.Text = fs.Description;
                    }
                    else
                        FileSystem.Text = "";

                    CalendarExtender1.SelectedDate = service.ScheduledTime;
                    ListItem item = new ListItem(service.ScheduledTime.ToString("hh:mm tt"), service.ScheduledTime.ToString("hh:mm tt"));
                    ScheduleTimeDropDownList.Items.Add(item);
                    ScheduleTimeDropDownList.SelectedValue = item.Value;
                
                }
                
            }

            base.DataBind();
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplyButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveData();
                Close();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        
        private void SaveData()
        {
            if (ServiceLock != null)
            {
                ServiceLock.Enabled = Enabled.Checked;

                ServiceLockConfigurationController controller = new ServiceLockConfigurationController();
                DateTime scheduledDate = DateTime.ParseExact(ScheduleDate.Text, CalendarExtender1.Format, null);
                DateTime scheduleTime = DateTime.ParseExact(ScheduleTimeDropDownList.SelectedValue, "hh:mm tt", null);
                scheduledDate = scheduledDate.Add(scheduleTime.TimeOfDay);
                if (controller.UpdateServiceLock(ServiceLock.GetKey(), Enabled.Checked, scheduledDate))
                {
                    if (ServiceLockUpdated != null)
                        ServiceLockUpdated(ServiceLock);
                }
                else
                {
                    MessageBox.Message = SR.ServiceLockUpdateFailed_ContactAdmin;
                    MessageBox.MessageType =
                        ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.ERROR;
                    MessageBox.Show();
                }
            }

        }

        #endregion Protected methods

        #region Public methods


        /// <summary>
        /// Displays the add/edit service dialog box.
        /// </summary>
        public void Show()
        {
            DataBind();

            ModalDialog1.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog1.Hide();
        }

        #endregion Public methods
    }
}
