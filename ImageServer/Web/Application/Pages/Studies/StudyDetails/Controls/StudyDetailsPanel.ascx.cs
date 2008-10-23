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
using AjaxControlToolkit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Main panel within the <see cref="Default"/>
    /// </summary>
    public partial class StudyDetailsPanel : UserControl
    {
        #region Private Members
        private Study _study;
        private Default _enclosingPage;
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
            ConfirmDialog.Confirmed += ConfirmDialog_Confirmed;
        }

        public override void DataBind()
        {
            // setup the data for the child controls
            if (Study != null)
            {
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartition partition = adaptor.Get(Study.ServerPartitionKey);

                PatientSummaryPanel.PatientSummary = PatientSummaryAssembler.CreatePatientSummary(Study);
          
                StudyDetailsTabs.Partition = partition;
                StudyDetailsTabs.Study = Study;                
            } 
            
            base.DataBind();

        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateUI(); 
            
            base.OnPreRender(e);
        }

        protected void UpdateUI()
        {
            if (Study!=null)
            {
                StudyController controller = new StudyController();
                bool scheduledForDelete = controller.IsScheduledForDelete(Study);
                bool scheduledForEdit = controller.IsScheduledForEdit(Study);
                bool scheduledForReconcile = Study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ReconcileScheduled);

                StudySummaryAssembler assembler = new StudySummaryAssembler();
                StudySummary studySummary = assembler.CreateStudySummary(Study);
                DeleteStudyButton.Enabled = controller.CanScheduleDelete(studySummary);
                EditStudyButton.Enabled = controller.CanScheduleEdit(studySummary);

                if (scheduledForDelete)
                {
                    ShowAlert(App_GlobalResources.SR.StudyScheduledForDeletion);
                }
                else if (scheduledForEdit)
                {
                    ShowAlert(App_GlobalResources.SR.StudyScheduledForEdit);
                }
                else if(scheduledForReconcile)
                {
                    ShowAlert(App_GlobalResources.SR.StudyScheduledForReconcile);
                }
                else
                {
                    MessagePanel.Visible = false;
                }
            }
           
        }

        protected void DeleteStudyButton_Click(object sender, EventArgs e)
        {
            ConfirmDialog.MessageType = MessageBox.MessageTypeEnum.YESNO;
            ConfirmDialog.Message = App_GlobalResources.SR.SingleStudyDelete;
            ConfirmDialog.Data = Study;

            ConfirmDialog.Show();
        }

        protected void EditStudyButton_Click(object sender, EventArgs e)
        {
            EnclosingPage.EditStudy();
        }

        #endregion Protected Methods

        #region Private Methods

        private void ConfirmDialog_Confirmed(object data)
        {
            StudyController controller = new StudyController();

            Study study = ConfirmDialog.Data as Study;
            if (controller.DeleteStudy(study))
            {
                DeleteStudyButton.Enabled = false;
                EditStudyButton.Enabled = false;

                ShowAlert(App_GlobalResources.SR.StudyScheduledForDeletion);
                
            }
            else
            {
                throw new Exception(App_GlobalResources.ErrorMessages.DeleteStudyError);

            } 
        }

        private void ShowAlert(string message)
        {
            MessagePanel.Visible = true;
            ConfirmationMessage.Text = message;
            UpdatePanel1.Update();
        }
        
        #endregion Private Methods
    }
}