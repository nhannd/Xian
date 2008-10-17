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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{

    /// <summary>
    /// Work Queue Search Panel
    /// </summary>
    public partial class SearchPanel : UserControl
    {
        #region Private Members

        private ServerPartition _serverPartition;
        private Default _enclosingPage;
        #endregion Private Members

        #region Static Class Initializer


        #endregion Static Class Initializer

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        public Default EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);


            ClearScheduleDateButton.OnClientClick = ScriptHelper.ClearDate(ScheduleDate.ClientID, ScheduleCalendarExtender.ClientID);

            // setup child controls

            GridPagerTop.ItemName = App_GlobalResources.SR.GridPagerWorkQueueSingleItem;
            GridPagerTop.PuralItemName = App_GlobalResources.SR.GridPagerWorkQueueMultipleItems;
            GridPagerBottom.ItemName = App_GlobalResources.SR.GridPagerWorkQueueSingleItem;
            GridPagerBottom.PuralItemName = App_GlobalResources.SR.GridPagerWorkQueueMultipleItems;
            GridPagerTop.Target = workQueueItemListPanel.WorkQueueItemListControl;
            GridPagerTop.GetRecordCountMethod = delegate
                                                  {
                                                      return workQueueItemListPanel.ResultCount;
                                                  };
            GridPagerBottom.GetRecordCountMethod = delegate
                                      {
                                          return workQueueItemListPanel.ResultCount;
                                      };

            GridPagerBottom.Target = workQueueItemListPanel.WorkQueueItemListControl;

            workQueueItemListPanel.DataSourceCreated += delegate(WorkQueueDataSource source)
                                                            {
                                                                source.AccessionNumber = AccessionNumber.Text;
                                                                source.Partition = ServerPartition;
                                                                source.PatientId = PatientId.Text;

                                                                if(ScheduleCalendarExtender.SelectedDate != null)
                                                                {
                                                                    source.ScheduledDate = ScheduleCalendarExtender.SelectedDate.Value.ToString(ScheduleCalendarExtender.Format);                                                                    
                                                                } else
                                                                {
                                                                    source.ScheduledDate = string.Empty;
                                                                }                                   

                                                                source.StudyDescription = StudyDescription.Text;
                                                                source.DateFormats = ScheduleCalendarExtender.Format;
                                                                if (TypeDropDownList.SelectedValue != string.Empty)
                                                                    source.TypeEnum = WorkQueueTypeEnum.GetEnum(TypeDropDownList.SelectedValue);

                                                                if (StatusDropDownList.SelectedValue != string.Empty)
                                                                    source.StatusEnum = WorkQueueStatusEnum.GetEnum(StatusDropDownList.SelectedValue);

                                                                if (PriorityDropDownList.SelectedValue != string.Empty)
                                                                    source.PriorityEnum = WorkQueuePriorityEnum.GetEnum(PriorityDropDownList.SelectedValue);

                                                            };

        }

        /// <summary>
        /// Handle user clicking the "Apply Filter" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            workQueueItemListPanel.WorkQueueItemListControl.PageIndex = 0;
            DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Setup the calendar for schedule date
            if (!Page.IsPostBack)
            {
                // first time load
                ScheduleCalendarExtender.SelectedDate = DateTime.Today;
            }
            else
            {
                ScheduleDate.Text = Request[ScheduleDate.UniqueID];
                if (!String.IsNullOrEmpty(ScheduleDate.Text))
                    ScheduleCalendarExtender.SelectedDate =
                        DateTime.ParseExact(ScheduleDate.Text, ScheduleCalendarExtender.Format, null);
                else
                    ScheduleCalendarExtender.SelectedDate = null;
            }

            // re-populate the drop down lists and restore their states
            IList<WorkQueueTypeEnum> workQueueTypes = WorkQueueTypeEnum.GetAll();
            IList<WorkQueueStatusEnum> workQueueStatuses = WorkQueueStatusEnum.GetAll();
            IList<WorkQueuePriorityEnum> workQueuePriorities = WorkQueuePriorityEnum.GetAll();

            int prevSelectedIndex = TypeDropDownList.SelectedIndex;
            TypeDropDownList.Items.Clear();
            TypeDropDownList.Items.Add(new ListItem(App_GlobalResources.SR.Any, string.Empty));
            foreach (WorkQueueTypeEnum t in workQueueTypes)
                TypeDropDownList.Items.Add(new ListItem(t.Description, t.Lookup));
            TypeDropDownList.SelectedIndex = prevSelectedIndex;

            prevSelectedIndex = StatusDropDownList.SelectedIndex;
            StatusDropDownList.Items.Clear();
            StatusDropDownList.Items.Add(new ListItem(App_GlobalResources.SR.Any, string.Empty));
            foreach (WorkQueueStatusEnum s in workQueueStatuses)
                StatusDropDownList.Items.Add(new ListItem(s.Description, s.Lookup));
            StatusDropDownList.SelectedIndex = prevSelectedIndex;


            prevSelectedIndex = PriorityDropDownList.SelectedIndex;
            PriorityDropDownList.Items.Clear();
            PriorityDropDownList.Items.Add(new ListItem(App_GlobalResources.SR.Any, string.Empty));
            foreach (WorkQueuePriorityEnum p in workQueuePriorities)
                PriorityDropDownList.Items.Add(new ListItem(p.Description, p.Lookup));
            PriorityDropDownList.SelectedIndex = prevSelectedIndex;

        }

        protected void ViewItemButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem.TheWorkQueueItem;
            if (item != null)
            {
                EnclosingPage.ViewWorkQueueItem(item.Key);
            }
        }


        protected void ResetItemButton_Click(object sender, EventArgs arg)
        {
            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem.TheWorkQueueItem;
            if (item != null)
            {
                EnclosingPage.ResetWorkQueueItem(item.Key);
            }
        }

        protected void DeleteItemButton_Click(object sender, EventArgs arg)
        {
            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem.TheWorkQueueItem;
            if (item != null)
            {
                EnclosingPage.DeleteWorkQueueItem(item.Key);
            }
        }

        protected void ReprocessItemButton_Click(object sender, EventArgs arg)
        {
            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem.TheWorkQueueItem;
            if (item != null)
            {
                EnclosingPage.ReprocessWorkQueueItem(item.Key);
            }
        }


        protected void RescheduleItemButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem.TheWorkQueueItem;
            if (item != null)
            {
                EnclosingPage.RescheduleWorkQueueItem(item.Key);
            }
            else
            {
                // the item no longer exist on the list... either it is deleted or filtered
                MessageBox.Title = string.Empty;
                MessageBox.BackgroundCSS = string.Empty;
                MessageBox.Message = App_GlobalResources.SR.SelectedWorkQueueNoLongerOnTheList;
                MessageBox.MessageType =
                    Web.Application.Controls.MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Show();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateToolBarButtons();

            base.OnPreRender(e);
        }

        protected void UpdateToolBarButtons()
        {
            WorkQueueSummary selectedItem = workQueueItemListPanel.SelectedWorkQueueItem;

            ViewItemDetailsButton.Enabled = selectedItem != null;
            RescheduleItemButton.Enabled = selectedItem != null && WorkQueueController.CanReschedule(selectedItem.TheWorkQueueItem);
            DeleteItemButton.Enabled = selectedItem != null && WorkQueueController.CanDelete(selectedItem.TheWorkQueueItem);
            ResetItemButton.Enabled = selectedItem != null && WorkQueueController.CanReset(selectedItem.TheWorkQueueItem);
            ReprocessItemButton.Enabled = selectedItem != null && WorkQueueController.CanReprocess(selectedItem.TheWorkQueueItem);
        }

        #endregion Protected Methods

    }
}