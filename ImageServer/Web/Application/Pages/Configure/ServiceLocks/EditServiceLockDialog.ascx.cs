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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using MessageBox=ClearCanvas.ImageServer.Web.Application.Controls.MessageBox;
using ModalDialog=ClearCanvas.ImageServer.Web.Application.Controls.ModalDialog;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServiceLocks
{
    //
    // Dialog for editting an existing service lock.
    //
    public partial class EditServiceLockDialog : UserControl
    {
        #region Constants
        private const string TIME_FORMAT = "hh:mm tt";
        private const int DEFAULT_TIME_GAP_MINS = 15; // minutes
        private ServiceLock _serviceLock;

        #endregion Constants

        #region Private Members
        private List<ListItem> _defaultTimeList;

        #endregion Private Members

        #region Events

        public delegate void ServiceLockUpdatedListener(ServiceLock serviceLock);

        public event ServiceLockUpdatedListener ServiceLockUpdated;

        #endregion Events

        #region public members


        /// <summary>
        /// Gets the default list of schedule times available for user selection
        /// </summary>
        public List<ListItem> DefaultTimeListItems
        {
            get
            {
                if (_defaultTimeList != null)
                    return _defaultTimeList;

                _defaultTimeList = new List<ListItem>();
                DateTime dt = DateTime.Today;
                DateTime tomorrow = DateTime.Today.AddDays(1);
                double scheduleTimeWindow = DEFAULT_TIME_GAP_MINS;
                while (dt < tomorrow)
                {
                    _defaultTimeList.Add(new ListItem(dt.ToString(TIME_FORMAT)));
                    dt = dt.AddMinutes(scheduleTimeWindow);
                }

                return _defaultTimeList;
            }
        }


        /// <summary>
        /// Sets/Gets the current editing service.
        /// </summary>
        public ServiceLock ServiceLock
        {
            set
            {
                _serviceLock = value;
                // put into viewstate to retrieve later
                if(_serviceLock != null)
                    ViewState[ClientID + "_ServiceLock"] = _serviceLock.GetKey();
            }
            get
            {
                return _serviceLock;
            }
        }

        #endregion // public members

        #region Protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ScheduleTimeDropDownList.Items.Clear();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ViewState[ClientID + "_ServiceLock"] != null)
            {
                ServerEntityKey serviceLockKey = ViewState[ClientID + "_ServiceLock"] as ServerEntityKey;
                _serviceLock = ServiceLock.Load(serviceLockKey);
            }

            ScheduleDate.Text = Request[ScheduleDate.UniqueID] ;
        }

        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
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



        #endregion Protected methods

        #region Private Methods

        private void AddCustomTime(DateTime customTime)
        {
            ScheduleTimeDropDownList.Items.Clear();
            ScheduleTimeDropDownList.Items.AddRange(DefaultTimeListItems.ToArray());

            string customTimeValue = customTime.ToString(TIME_FORMAT);
            if (ScheduleTimeDropDownList.Items.FindByValue(customTimeValue)==null)
            {
                ScheduleTimeDropDownList.Items.Add(new ListItem(customTimeValue));
            }

            ScheduleTimeDropDownList.SelectedValue = customTimeValue;
        }

        private void SaveData()
        {
            if (ServiceLock != null)
            {
                ServiceLock.Enabled = Enabled.Checked;

                ServiceLockConfigurationController controller = new ServiceLockConfigurationController();
                DateTime scheduledDate = DateTime.ParseExact(ScheduleDate.Text, CalendarExtender.Format, null);
                DateTime scheduleTime = DateTime.ParseExact(ScheduleTimeDropDownList.SelectedValue, TIME_FORMAT, null);
                scheduledDate = scheduledDate.Add(scheduleTime.TimeOfDay);
                if (controller.UpdateServiceLock(ServiceLock.GetKey(), Enabled.Checked, scheduledDate))
                {
                    if (ServiceLockUpdated != null)
                        ServiceLockUpdated(ServiceLock);
                }
                else
                {
                    ErrorMessageBox.Message = App_GlobalResources.SR.ServiceLockUpdateFailed_ContactAdmin;
                    ErrorMessageBox.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    ErrorMessageBox.Show();
                }
            }

        }

        #endregion Private Methods

        #region Public methods



        public override void DataBind()
        {
            ServiceLock service = ServiceLock;

            if (service != null)
            {
                // only rebind the data if the dialog has been closed
                if (ModalDialog.State == ModalDialog.ShowState.Hide)
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
                        FileSystem.Text = string.Empty;

                    CalendarExtender.SelectedDate = service.ScheduledTime;
                    
                    AddCustomTime(service.ScheduledTime);

                }

            }

            base.DataBind();
        }


        /// <summary>
        /// Displays the add/edit service dialog box.
        /// </summary>
        public void Show()
        {
            DataBind();

            ModalDialog.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog.Hide();
        }

        #endregion Public methods
    }
}
