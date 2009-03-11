#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{

    /// <summary>
    /// Work Queue Search Panel
    /// </summary>

    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
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

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return workQueueItemList.WorkQueueItemGridView.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewDetailsButtonClientID")]
        public string ViewDetailsButtonClientID
        {
            get { return ViewItemDetailsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("RescheduleButtonClientID")]
        public string RescheduleButtonClientID
        {
            get { return RescheduleItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ResetButtonClientID")]
        public string ResetButtonClientID
        {
            get { return ResetItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ReprocessButtonClientID")]
        public string ReprocessButtonClientID
        {
            get { return ReprocessItemButton.ClientID; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);

            ClearScheduleDateButton.OnClientClick = ScriptHelper.ClearDate(ScheduleDate.ClientID, ScheduleCalendarExtender.ClientID);

            // setup child controls
            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerWorkQueueSingleItem, App_GlobalResources.SR.GridPagerWorkQueueMultipleItems, workQueueItemList.WorkQueueItemGridView, delegate { return workQueueItemList.ResultCount; }, ImageServerConstants.GridViewPagerPosition.top);

            workQueueItemList.DataSourceCreated += delegate(WorkQueueDataSource source)
                                                            {
                                                                source.PatientsName = PatientName.Text;
                                                                source.Partition = ServerPartition;
                                                                source.PatientId = PatientId.Text;

                                                                if(ScheduleCalendarExtender.SelectedDate != null)
                                                                {
                                                                    source.ScheduledDate = ScheduleCalendarExtender.SelectedDate.Value.ToString(ScheduleCalendarExtender.Format);                                                                    
                                                                } else
                                                                {
                                                                    source.ScheduledDate = string.Empty;
                                                                }                                   

                                                                source.DateFormats = ScheduleCalendarExtender.Format;

                                                                if (TypeListBox.SelectedIndex > -1)
                                                                {
                                                                    List<WorkQueueTypeEnum> types = new List<WorkQueueTypeEnum>();
                                                                    foreach (ListItem item in TypeListBox.Items)
                                                                    {
                                                                        if (item.Selected)
                                                                        {
                                                                            types.Add(WorkQueueTypeEnum.GetEnum(item.Value));
                                                                        }
                                                                    }
                                                                    source.TypeEnums = types.ToArray();
                                                                }

                                                                if (StatusListBox.SelectedIndex > -1)
                                                                {
                                                                    List<WorkQueueStatusEnum> statuses = new List<WorkQueueStatusEnum>();
                                                                    foreach (ListItem item in StatusListBox.Items)
                                                                    {
                                                                        if (item.Selected)
                                                                        {
                                                                            statuses.Add(WorkQueueStatusEnum.GetEnum(item.Value));
                                                                        }
                                                                    }
                                                                    source.StatusEnums = statuses.ToArray();
                                                                }

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
            workQueueItemList.Refresh();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Setup the calendar for schedule date
            if (Page.IsPostBack)
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

            if (TypeListBox.Items.Count == 0)
            {
                foreach (WorkQueueTypeEnum t in workQueueTypes)
                {
                    TypeListBox.Items.Add(new ListItem(t.Description, t.Lookup));
                }
            }
            else
            {
                ListItem[] typeItems = new ListItem[TypeListBox.Items.Count];
                TypeListBox.Items.CopyTo(typeItems, 0);
                TypeListBox.Items.Clear();
                int count = 0;
                foreach (WorkQueueTypeEnum t in workQueueTypes)
                {
                    TypeListBox.Items.Add(new ListItem(t.Description, t.Lookup));
                    TypeListBox.Items[count].Selected = typeItems[count].Selected;
                    count++;
                }
            }

            if (StatusListBox.Items.Count == 0)
            {
                foreach (WorkQueueStatusEnum s in workQueueStatuses)
                {
                    StatusListBox.Items.Add(new ListItem(s.Description, s.Lookup));
                }
            }
            else
            {
                ListItem[] statusItems = new ListItem[StatusListBox.Items.Count];
                StatusListBox.Items.CopyTo(statusItems, 0);
                StatusListBox.Items.Clear();
                int count = 0;
                foreach (WorkQueueStatusEnum s in workQueueStatuses)
                {
                    StatusListBox.Items.Add(new ListItem(s.Description, s.Lookup));
                    StatusListBox.Items[count].Selected = statusItems[count].Selected;
                    count++;
                }
            }
            int prevSelectedIndex = PriorityDropDownList.SelectedIndex;
            PriorityDropDownList.Items.Clear();
            PriorityDropDownList.Items.Add(new ListItem(App_GlobalResources.SR.Any, string.Empty));
            foreach (WorkQueuePriorityEnum p in workQueuePriorities)
                PriorityDropDownList.Items.Add(new ListItem(p.Description, p.Lookup));
            PriorityDropDownList.SelectedIndex = prevSelectedIndex;

        }

        protected void ViewItemButton_Click(object sender, ImageClickEventArgs e)
        {
			if (workQueueItemList.SelectedItems == null)
				DataBind();

			if (workQueueItemList.SelectedItems[0] != null)
            {
                EnclosingPage.ViewWorkQueueItem(workQueueItemList.SelectedItems[0].Key);
            }
        }


        protected void ResetItemButton_Click(object sender, EventArgs arg)
        {
			if (workQueueItemList.SelectedItems == null)
				DataBind();

            if (workQueueItemList.SelectedItems[0] != null)
            {
                EnclosingPage.ResetWorkQueueItem(workQueueItemList.SelectedItems[0].Key);
                workQueueItemList.RefreshCurrentPage();
            }
        }

        protected void DeleteItemButton_Click(object sender, EventArgs arg)
        {
			if (workQueueItemList.SelectedItems == null)
				DataBind();

			if (workQueueItemList.SelectedItems[0] != null)
            {
                EnclosingPage.DeleteWorkQueueItem(workQueueItemList.SelectedItems[0].Key);
                workQueueItemList.RefreshCurrentPage();
            }
        }

        protected void ReprocessItemButton_Click(object sender, EventArgs arg)
        {
			if (workQueueItemList.SelectedItems == null)
				DataBind();

			if (workQueueItemList.SelectedItems[0] != null)
            {
                EnclosingPage.ReprocessWorkQueueItem(workQueueItemList.SelectedItems[0].Key);
                workQueueItemList.RefreshCurrentPage();
            }
        }


        protected void RescheduleItemButton_Click(object sender, ImageClickEventArgs e)
        {
			if (workQueueItemList.SelectedItems == null)
				DataBind();

            if (workQueueItemList.SelectedItems[0] != null)
            {
                EnclosingPage.RescheduleWorkQueueItem(workQueueItemList.SelectedItems[0].Key);
                workQueueItemList.RefreshCurrentPage();
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

        #endregion Protected Methods

    }
}