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
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue
{

    /// <summary>
    /// Work Queue Search Panel
    /// </summary>
    public partial class SearchPanel : UserControl
    {
        #region Private Members

        private ServerPartition _serverPartition;
        private WorkQueueController _searchController;
        private SearchPage _enclosingPage;

        private WorkQueueItemCollection _workQueueItems;
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

        public SearchPage EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        /// <summary>
        /// Gets the <see cref="WorkQueueItemCollection"/> associated with this search panel.
        /// </summary>
        public WorkQueueItemCollection WorkQueueItems
        {
            get { return _workQueueItems; }
            set { _workQueueItems = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            
            base.OnInit(e);

            // initialize the controller
            _searchController = new WorkQueueController();

            CalendarExtender1.Format = DateTimeFormatter.DefaultDateFormat;

            ClearScheduleDateButton.OnClientClick = "document.getElementById('" + ScheduleDate.ClientID + "').value=''; return false;";

            // setup child controls
            GridPagerTop.PageCountVisible = false;
            GridPagerTop.ItemCountVisible = true;
            GridPagerTop.ItemName = "Work Item";
            GridPagerTop.PuralItemName = "Work Items";
            GridPagerTop.Target = workQueueItemListPanel.WorkQueueItemListControl;
            GridPagerTop.GetRecordCountMethod = delegate
                                                  {
                                                      return workQueueItemListPanel.WorkQueueItems == null ? 0 : workQueueItemListPanel.WorkQueueItems.Count;
                                                  };

            GridPagerBottom.PageCountVisible = true;
            GridPagerBottom.ItemCountVisible = false;
            GridPagerBottom.Target = workQueueItemListPanel.WorkQueueItemListControl;
            
        }

        /// <summary>
        /// Handle user clicking the "Apply Filter" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            workQueueItemListPanel.PageIndex = 0;
            DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // first time load
                CalendarExtender1.SelectedDate = DateTime.Today;
            }
            else
            {
                ScheduleDate.Text = Request[ScheduleDate.UniqueID];
                if (!String.IsNullOrEmpty(ScheduleDate.Text))
                    CalendarExtender1.SelectedDate = DateTime.ParseExact(ScheduleDate.Text, CalendarExtender1.Format, null);
                else
                    CalendarExtender1.SelectedDate = null;

            }

            //
            
            // re-populate the drop down lists and restore their states
            IList<WorkQueueTypeEnum> workQueueTypes = WorkQueueTypeEnum.GetAll();
            IList<WorkQueueStatusEnum> workQueueStatuses = WorkQueueStatusEnum.GetAll();
            IList<WorkQueuePriorityEnum> workQueuePriorities = WorkQueuePriorityEnum.GetAll();
            
            int prevSelectedIndex = TypeDropDownList.SelectedIndex;
            TypeDropDownList.Items.Clear();
            TypeDropDownList.Items.Add(new ListItem("-- Any --", ""));
            foreach (WorkQueueTypeEnum t in workQueueTypes)
                TypeDropDownList.Items.Add(new ListItem(t.Description, t.Lookup));
            TypeDropDownList.SelectedIndex = prevSelectedIndex;

            prevSelectedIndex = StatusDropDownList.SelectedIndex;
            StatusDropDownList.Items.Clear();
            StatusDropDownList.Items.Add(new ListItem("-- Any --", ""));
            foreach (WorkQueueStatusEnum s in workQueueStatuses)
                StatusDropDownList.Items.Add(new ListItem(s.Description, s.Lookup));
            StatusDropDownList.SelectedIndex = prevSelectedIndex;


            prevSelectedIndex = PriorityDropDownList.SelectedIndex;
            PriorityDropDownList.Items.Clear();
            PriorityDropDownList.Items.Add(new ListItem("-- Any --", ""));
            foreach (WorkQueuePriorityEnum p in workQueuePriorities)
                PriorityDropDownList.Items.Add(new ListItem(p.Description, p.Lookup));
            PriorityDropDownList.SelectedIndex = prevSelectedIndex;

            
           
        }


        protected void LoadWorkQueues()
        {
        
            if (ServerPartition != null)
            {
                WebWorkQueueQueryParameters parameters = new WebWorkQueueQueryParameters();
                parameters.ServerPartitionKey = ServerPartition.GetKey();
                parameters.Accession = AccessionNumber.Text;
                parameters.PatientID = PatientId.Text;
                if (String.IsNullOrEmpty(ScheduleDate.Text ))
                    parameters.ScheduledTime = null;
                else
                    parameters.ScheduledTime = DateTime.ParseExact(ScheduleDate.Text, CalendarExtender1.Format, null);// CalendarExtender1.SelectedDate;

                parameters.Accession = AccessionNumber.Text;
                parameters.StudyDescription = StudyDescription.Text;
                parameters.Type = (TypeDropDownList.SelectedValue == "")
                                    ? null
                                    : WorkQueueTypeEnum.GetEnum(TypeDropDownList.SelectedValue);
                parameters.Status = (StatusDropDownList.SelectedValue == "")
                                        ? null

                                        : WorkQueueStatusEnum.GetEnum(StatusDropDownList.SelectedValue);

                parameters.Priority = (PriorityDropDownList.SelectedValue == "")
                                        ? null
                                        : WorkQueuePriorityEnum.GetEnum(PriorityDropDownList.SelectedValue);

                IList<Model.WorkQueue> list = _searchController.FindWorkQueue(parameters);

                WorkQueueItems = new WorkQueueItemCollection(list);

                workQueueItemListPanel.WorkQueueItems = WorkQueueItems;
               
            }
            
        }

        protected void ViewButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem;
            if (item != null)
            {
               EnclosingPage.ViewWorkQueueItem(item.GetKey());
            }
        }


        protected void Reset_Click(object sender, EventArgs arg)
        {

            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem;
            if (item != null)
            {
                EnclosingPage.ResetWorkQueueItem(item.GetKey());
            }

        }

        protected void Delete_Click(object sender, EventArgs arg)
        {

            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem;
            if (item != null)
            {
                EnclosingPage.DeleteWorkQueueItem(item.GetKey());
            }

        }


        protected void RescheduleButton_Click(object sender, ImageClickEventArgs e)
        {
            
            Model.WorkQueue item = workQueueItemListPanel.SelectedWorkQueueItem;
            if (item != null)
            {
               EnclosingPage.RescheduleWorkQueueItem(item.GetKey());
            }
            else 
            {
                // the item no longer exist on the list... either it is deleted or filtered
                ConfirmationDialog.Title = "";
                ConfirmationDialog.BackgroundCSS = "";
                ConfirmationDialog.Message = App_GlobalResources.SR.SelectedWorkQueueNoLongerOnTheList;
                ConfirmationDialog.MessageType =
                    ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.ERROR;
                ConfirmationDialog.Show();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateToolBarButtons(); 
            
            base.OnPreRender(e);

        }

        protected void UpdateToolBarButtons()
        {
            Model.WorkQueue selectedItem = workQueueItemListPanel.SelectedWorkQueueItem;

            ViewItemDetailsButton.Enabled = selectedItem != null;
            RescheduleItemButton.Enabled = selectedItem != null && WorkQueueController.CanReschedule(selectedItem);
            DeleteItemButton.Enabled = selectedItem != null && WorkQueueController.CanDelete(selectedItem);
            ResetItemButton.Enabled = selectedItem != null && WorkQueueController.CanReset(selectedItem);
        }


            
        #endregion Protected Methods


        #region Public Methods

        public override void DataBind()
        {

            if (workQueueItemListPanel.IsPostBack)
                LoadWorkQueues();

            base.DataBind();
        }

        #endregion
    }
}