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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue
{
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

        private ArchiveQueueController _controller = new ArchiveQueueController();

    	#endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return ArchiveQueueItemList.ArchiveQueueGrid.ClientID; }
        }

        #endregion Public Properties  

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            ScheduleDate.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
        }

        public override void DataBind()
        {
            ArchiveQueueItemList.Partition = ((Default)Page).ServerPartition;
            base.DataBind();
            ArchiveQueueItemList.DataBind();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearScheduleDateButton.OnClientClick = "document.getElementById('" + ScheduleDate.ClientID + "').value=''; return false;";
            
            // setup child controls
            GridPagerBottom.Target = ArchiveQueueItemList.ArchiveQueueGrid;

            GridPagerTop.ItemName = App_GlobalResources.Labels.GridPagerQueueSingleItem;
            GridPagerTop.PuralItemName = App_GlobalResources.Labels.GridPagerQueueMultipleItems;
            GridPagerTop.Target = ArchiveQueueItemList.ArchiveQueueGrid;
            GridPagerTop.GetRecordCountMethod = delegate
                              {
								  return ArchiveQueueItemList.ResultCount;
                              };

            GridPagerBottom.ItemName = App_GlobalResources.Labels.GridPagerQueueSingleItem;
            GridPagerBottom.PuralItemName = App_GlobalResources.Labels.GridPagerQueueMultipleItems;
            GridPagerBottom.Target = ArchiveQueueItemList.ArchiveQueueGrid;
            GridPagerBottom.GetRecordCountMethod = delegate
                              {
                                  return ArchiveQueueItemList.ResultCount;
                              };


            MessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Model.ArchiveQueue>)
                                {
                                    IList<Model.ArchiveQueue> items = data as IList<Model.ArchiveQueue>;
                                    foreach (Model.ArchiveQueue item in items)
                                    {
                                        _controller.DeleteArchiveQueueItem(item);
                                    }
                                }
                                else if (data is Model.ArchiveQueue)
                                {
                                    Model.ArchiveQueue item = data as Model.ArchiveQueue;
                                    _controller.DeleteArchiveQueueItem(item);
                                }

                                DataBind();
                                UpdatePanel.Update(); // force refresh

                            };

			ArchiveQueueItemList.DataSourceCreated += delegate(ArchiveQueueDataSource source)
										{
											source.Partition = ((Default)Page).ServerPartition;
                                            source.DateFormats = ScheduleDateCalendarExtender.Format;

											if (!String.IsNullOrEmpty(PatientId.Text))
												source.PatientId = PatientId.Text;
											if (!String.IsNullOrEmpty(PatientName.Text))
												source.PatientName = PatientName.Text;
											if (!String.IsNullOrEmpty(ScheduleDate.Text))
												source.ScheduledDate = ScheduleDate.Text;
										};
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScheduleDate.Text = Request[ScheduleDate.UniqueID];
            if (!String.IsNullOrEmpty(ScheduleDate.Text))
                ScheduleDateCalendarExtender.SelectedDate = DateTime.ParseExact(ScheduleDate.Text, ScheduleDateCalendarExtender.Format, null);
            else
                ScheduleDateCalendarExtender.SelectedDate = null;

            IList<ArchiveQueueStatusEnum> statusItems = ArchiveQueueStatusEnum.GetAll();

            int prevSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem("All", "All"));
            foreach (ArchiveQueueStatusEnum s in statusItems)
                StatusFilter.Items.Add(new ListItem(s.Description, s.Lookup));
            StatusFilter.SelectedIndex = prevSelectedIndex;

			if (ArchiveQueueItemList.IsPostBack)
			{
				DataBind();
			} 
        }

        protected override void OnPreRender(EventArgs e)
        {

			UpdateUI();
			base.OnPreRender(e);
        }

        protected void UpdateUI()
        {
            UpdateToolbarButtonState();
        }
        
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            ArchiveQueueItemList.ArchiveQueueGrid.ClearSelections();
        	ArchiveQueueItemList.ArchiveQueueGrid.PageIndex = 0;
			DataBind();
        }

        protected void DeleteItemButton_Click(object sender, EventArgs e)
        {
            IList<Model.ArchiveQueue> items = ArchiveQueueItemList.SelectedItems;

            if (items != null && items.Count>0)
            {
                if (items.Count > 1) MessageBox.Message = string.Format(App_GlobalResources.SR.MultipleArchiveQueueDelete);
                else MessageBox.Message = string.Format(App_GlobalResources.SR.SingleArchiveQueueDelete);

                MessageBox.Message += "<table>";
                foreach (Model.ArchiveQueue item in items)
                {
                    String text = "";
                    //String.Format("<tr align='left'><td>Patient:{0}&nbsp;&nbsp;</td><td>Accession:{1}&nbsp;&nbsp;</td><td>Description:{2}</td></tr>", 
                                    //item.PatientsName, item.AccessionNumber, item.StudyDescription);
                    MessageBox.Message += text;
                }
                MessageBox.Message += "</table>";

                MessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
                MessageBox.Data = items;
                MessageBox.Show();
            }
        }

        protected void UpdateToolbarButtonState()
        {
            IList<Model.ArchiveQueue> items = ArchiveQueueItemList.SelectedItems;
            if (items != null)
            {
				DeleteItemButton.Enabled = true;
            }
            else
            {
                DeleteItemButton.Enabled = false;
            }
        }

        #endregion Protected Methods
    }
}