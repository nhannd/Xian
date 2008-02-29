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
using System.Drawing;
using System.Web.UI;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    /// <summary>
    /// Main panel within the <see cref="StudyDetailsPage"/>
    /// </summary>
    public partial class StudyDetailsPanel : UserControl
    {
        #region Private Members
        private Study _study;
        private ServerPartition _partition;
        #endregion Private Members


        #region Public Properties

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

       
        #endregion Public Properties


        #region Protected Methods


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ConfirmDialog1.Confirmed += new ConfirmDialog.ConfirmedEventHandler(ConfirmDialog1_Confirmed);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Study!=null)
            {
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartition partition = adaptor.Get(Study.ServerPartitionKey);

                PatientSummaryPanel.PatientSummary = PatientSummaryAssembler.CreatePatientSummary(Study);
                StudyDetailsView1.Studies.Add(Study);
                SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
                SeriesSelectCriteria criteria = new SeriesSelectCriteria();
                criteria.StudyKey.EqualTo(Study.GetKey());
                criteria.ServerPartitionKey.EqualTo(partition.GetKey());
                SeriesGridView1.Series = seriesAdaptor.Get(criteria); 
            }
           
            
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateUI();
        }

        protected void UpdateUI()
        {
           
            StudyController controller = new StudyController();
            bool scheduledForDelete = controller.IsScheduledForDelete(Study);

            //TODO: make Delete button enabled/disabled based on user permission too
            DeleteToolbarButton.Enabled = !scheduledForDelete;

            if (scheduledForDelete)
            {
                ShowScheduledForDeleteAlert();
            }
            else
            {
                MessagePanel.Visible = false;
            }
        }

        protected void DeleteToolbarButton_Click(object sender, ImageClickEventArgs e)
        {
            ConfirmDialog1.MessageType = ConfirmDialog.MessageTypeEnum.WARNING;
            ConfirmDialog1.Message = "Are you sure to delete this study?";
            ConfirmDialog1.Data = Study;

            ConfirmDialog1.Show();
        }

        #endregion Protected Methods

        #region Private Methods

        private void ConfirmDialog1_Confirmed(object data)
        {
            StudyController controller = new StudyController();

            Study study = ConfirmDialog1.Data as Study;
            if (controller.DeleteStudy(study))
            {
                DeleteToolbarButton.Enabled = false;

                ShowScheduledForDeleteAlert();
                
            }
            else
            {
                throw new Exception("Unable to delete the study. See server log for more details");

            }

            
        }

        private void ShowScheduledForDeleteAlert()
        {
            MessagePanel.Visible = true;
            ConfirmationMessage.Text = "This study has been scheduled for delete !";
            ConfirmationMessage.ForeColor = Color.Red;
                
            UpdatePanel1.Update();
        }


        #endregion Private Methods
    }
}