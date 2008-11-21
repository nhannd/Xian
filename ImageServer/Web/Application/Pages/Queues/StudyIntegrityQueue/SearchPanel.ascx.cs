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
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using ClearCanvas.Common.Utilities;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

    	private ServerPartition _serverPartition;

    	#endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("ReconcileButtonClientID")]
        public string ReconcileButtonClientID
        {
            get { return ReconcileButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return StudyIntegrityQueueItemList.StudyIntegrityQueueGrid.ClientID; }
        }

		/// <summary>
		/// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
		/// </summary>
		public ServerPartition ServerPartition
		{
			get { return _serverPartition; }
			set { _serverPartition = value; }
		}
        #endregion Public Properties  

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientName.Text = string.Empty;
        }

        public override void DataBind()
        {
            StudyIntegrityQueueItemList.Partition = ServerPartition;
            base.DataBind();
            StudyIntegrityQueueItemList.DataBind();
        }

        public void UpdateUI()
        {
            StudyIntegrityQueueItemList.DataBind();
            SearchPanelUpdatePanel.Update();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearReceivedDateButton.OnClientClick = ScriptHelper.ClearDate(ReceivedDate.ClientID, ReceivedDateCalendarExtender.ClientID);
                         
            GridPagerTop.InitializeGridPager(App_GlobalResources.Labels.GridPagerQueueSingleItem, 
                                             App_GlobalResources.Labels.GridPagerQueueMultipleItems, 
                                             StudyIntegrityQueueItemList.StudyIntegrityQueueGrid);
            GridPagerTop.GetRecordCountMethod = delegate
                              {
								  return StudyIntegrityQueueItemList.ResultCount;
                              };

            GridPagerBottom.InitializeGridPager(App_GlobalResources.Labels.GridPagerQueueSingleItem, 
                                                App_GlobalResources.Labels.GridPagerQueueMultipleItems, 
                                                StudyIntegrityQueueItemList.StudyIntegrityQueueGrid);
            GridPagerBottom.GetRecordCountMethod = delegate
                              {
                                  return StudyIntegrityQueueItemList.ResultCount;
                              };

			StudyIntegrityQueueItemList.DataSourceCreated += delegate(StudyIntegrityQueueDataSource source)
										{
											source.Partition = ServerPartition;

											if (!String.IsNullOrEmpty(PatientName.Text))
												source.PatientName = "*" + PatientName.Text + "*";
                                            if (!String.IsNullOrEmpty(AccessionNumber.Text))
										        source.AccessionNumber = "*" + AccessionNumber.Text + "*";
                                            if (!String.IsNullOrEmpty(ReceivedDate.Text))
                                                source.InsertTime =ReceivedDate.Text;
										};
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			if (StudyIntegrityQueueItemList.IsPostBack)
			{
				DataBind();
			} 
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateToolbarButtons();
			base.OnPreRender(e);
        }

        protected void UpdateToolbarButtons()
        {
            ReconcileButton.Enabled = (StudyIntegrityQueueItemList.SelectedItems != null)
                && CollectionUtils.TrueForAll<StudyIntegrityQueueSummary>(StudyIntegrityQueueItemList.SelectedItems,
                delegate(StudyIntegrityQueueSummary item)
                {
                    return item.CanReconcile;
                });
        }
       
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyIntegrityQueueItemList.StudyIntegrityQueueGrid.ClearSelections();
        	StudyIntegrityQueueItemList.StudyIntegrityQueueGrid.PageIndex = 0;
			DataBind();
        }

        protected void ReconcileButton_Click(object sender, EventArgs e)
        {
            ReconcileDetails details = ReconcileDetailsAssembler.CreateReconcileDetails(StudyIntegrityQueueItemList.SelectedItems[0]);

            ((Default)Page).OnReconcileItem(details);
        }

        #endregion Protected Methods
    }
}