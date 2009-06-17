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
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.js")]
    public partial class MovePanel : AJAXScriptControl
    {
        [Serializable]
        private class MoveRequest
        {
            private IList<Study> _studies;
            private Device _device;

            public IList<Study> Studies
            {
                get { return _studies; }
                set { _studies = value; }
            }

            public Device DestinationDevice
            {
                get { return _device; }
                set { _device = value; }
            }
        }
        private ServerPartition _partition;
        private DeviceConfigurationController _theController;
        private IList<Study> _studies = new List<Study>();

        [ExtenderControlProperty]
        [ClientPropertyName("MoveButtonClientID")]
        public string MoveButtonClientID
        {
            get { return MoveButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeviceListClientID")]
        public string DeviceListClientID
        {
            get { return DeviceGridPanel.TheGrid.ClientID; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public StudyGridView StudyGridView
        {
            get { return this.StudyGridPanel; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateUI();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new DeviceConfigurationController();

            // setup child controls
            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerDeviceSingleItem, App_GlobalResources.SR.GridPagerDeviceMultipleItems, DeviceGridPanel.TheGrid, delegate { return DeviceGridPanel.Devices.Count; }, ImageServerConstants.GridViewPagerPosition.top);
            DeviceGridPanel.Pager = GridPagerTop;

            MoveConfirmation.Confirmed += delegate(object data)
                                              {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "alertScript", "self.close();", true);
                                              };

        }
        /// <summary>
        /// Updates the device list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadDevices()"/> instead.
        /// </remarks>
        public void UpdateUI()
        {
            LoadDevices();
            Device dev = this.DeviceGridPanel.SelectedDevice;

            if (dev == null)
            {
                // no device being selected
                MoveButton.Enabled = false;
            }
            else
            {
                MoveButton.Enabled = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
            ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
            criteria.AeTitle.EqualTo(Request.QueryString[ImageServerConstants.QueryStrings.ServerAE]);
            _partition = adaptor.GetFirst(criteria);

            for(int i=1;;i++)
            {
                string qs = string.Format(ImageServerConstants.QueryStrings.StudyUID + "{0}", i);
                string studyuid = Request.QueryString[qs];

                if(!string.IsNullOrEmpty(studyuid))
                {
                    _studies.Add(LoadStudy(studyuid));
                }
                else
                {
                    break;
                }
            }

            StudyGridPanel.StudyList = _studies;

            LoadDevices();            
        }

        protected Study LoadStudy(string studyInstanceUID)
        {
            if (String.IsNullOrEmpty(studyInstanceUID))
                return null;

            if (_partition == null)
                return null;

            StudyAdaptor studyAdaptor = new StudyAdaptor();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.StudyInstanceUid.EqualTo(studyInstanceUID);
            criteria.ServerPartitionKey.EqualTo(Partition.Key);
            return studyAdaptor.GetFirst(criteria);
        }

        /// <summary>
        /// Load the devices for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="UpdateUI()"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="UpdateUI()"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadDevices()
        {
            DeviceSelectCriteria criteria = new DeviceSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

            if (!String.IsNullOrEmpty(AETitleFilter.Text))
            {
                string key = AETitleFilter.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.AeTitle.Like(key);
            }

            // only enabled devices and devices that allow retrieves.
            criteria.Enabled.EqualTo(true);
            criteria.AllowRetrieve.EqualTo(true);

            DeviceGridPanel.Devices = _theController.GetDevices(criteria);
            DeviceGridPanel.Refresh();
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            LoadDevices();
        }

        protected void MoveButton_Click(object sender, EventArgs e)
        {
            if (DeviceGridPanel.SelectedDevice != null)
            {
                MoveConfirmation.Message = DialogHelper.createConfirmationMessage(string.Format(App_GlobalResources.SR.MoveStudyMessage, DeviceGridPanel.SelectedDevice.AeTitle));
                
                MoveConfirmation.Message += DialogHelper.createStudyTable(StudyGridView.StudyList);
                    
                // Create the move request, although it really isn't needed.
                MoveRequest data = new MoveRequest();
                data.Studies = StudyGridView.StudyList;
                data.DestinationDevice = DeviceGridPanel.SelectedDevice;
                MoveConfirmation.Data = data;

                StudyController studyController = new StudyController();
                foreach (Study study in data.Studies)
                {
                    studyController.MoveStudy(study, data.DestinationDevice);
                }

                MoveConfirmation.MessageType = MessageBox.MessageTypeEnum.INFORMATION;
                MoveConfirmation.Show();
            }
        }
    }
}